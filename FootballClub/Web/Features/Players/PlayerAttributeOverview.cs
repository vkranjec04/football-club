using FootballClub.Models;
using FootballClub.Models.Enums;

namespace FootballClub.Web.Features.Players;

// Five-axis attribute summary (0-100) shown as a radar chart on the player profile.
// Derived from the player's position (a rough archetype bias) plus their real match
// stats (rating, goals/assists/cards per match), so it reflects the actual player
// instead of a fixed placeholder shape.
public class PlayerAttributeOverview
{
    public int Attacking { get; set; }
    public int Creativity { get; set; }
    public int Technical { get; set; }
    public int Defending { get; set; }
    public int Tackling { get; set; }

    private static readonly Dictionary<PlayerPosition, (int Att, int Cre, int Tec, int Def, int Tac)> PositionBias = new()
    {
        [PlayerPosition.Goalkeeper] = (-20, -15, -8, 10, 2),
        [PlayerPosition.Defender] = (-8, -4, 0, 16, 14),
        [PlayerPosition.Midfielder] = (4, 10, 9, 2, 9),
        [PlayerPosition.Forward] = (14, 5, 6, -12, -10),
    };

    public static PlayerAttributeOverview From(Player player, IReadOnlyCollection<PlayerStat> stats)
    {
        var bias = PositionBias[player.Position];
        var matchesPlayed = stats.Count;

        var avgRating = matchesPlayed > 0 ? stats.Average(s => s.Rating) : 6.0;
        var goalsPerMatch = matchesPlayed > 0 ? stats.Sum(s => s.Goals) / (double)matchesPlayed : 0;
        var assistsPerMatch = matchesPlayed > 0 ? stats.Sum(s => s.Assists) / (double)matchesPlayed : 0;
        var cardsPerMatch = matchesPlayed > 0 ? stats.Sum(s => s.YellowCards + (s.RedCard ? 2 : 0)) / (double)matchesPlayed : 0;

        var baseline = Clamp(avgRating * 10, 35, 95);

        return new PlayerAttributeOverview
        {
            Attacking = Clamp(baseline + bias.Att + goalsPerMatch * 35),
            Creativity = Clamp(baseline + bias.Cre + assistsPerMatch * 35),
            Technical = Clamp(baseline + bias.Tec),
            Defending = Clamp(baseline + bias.Def - cardsPerMatch * 6),
            Tackling = Clamp(baseline + bias.Tac - cardsPerMatch * 4),
        };
    }

    private static int Clamp(double value, double min = 20, double max = 99) =>
        (int)Math.Round(Math.Clamp(value, min, max));
}
