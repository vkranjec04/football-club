using FootballClub.Models;
using FootballClub.Models.Enums;
namespace FootballClub.Repositories;

/// <summary>
/// Centralna klasa sa statičkim seed podacima (preneseno iz Lab 1).
/// Koristi se od strane svih mock repozitorija.
/// </summary>
public static class MockData
{
    public static readonly List<Stadium> Stadiums;
    public static readonly List<Club> Clubs;
    public static readonly List<Coach> Coaches;
    public static readonly List<Player> Players;
    public static readonly List<Match> Matches;
    public static readonly List<PlayerStat> PlayerStats;
    public static readonly List<Transfer> Transfers;

    static MockData()
    {
        // --- STADIONI ---
        var maksimir = new Stadium { Id = 1, Name = "Stadion Maksimir", City = "Zagreb", Capacity = 35_123, YearBuilt = 1912 };
        var poljud = new Stadium { Id = 2, Name = "Stadion Poljud", City = "Split", Capacity = 34_198, YearBuilt = 1979 };
        var rijekaSt = new Stadium { Id = 3, Name = "HNK Rijeka stadion", City = "Rijeka", Capacity = 8_279, YearBuilt = 1946 };
        Stadiums = new List<Stadium> { maksimir, poljud, rijekaSt };

        // --- KLUBOVI ---
        var dinamo = new Club { Id = 1, Name = "GNK Dinamo Zagreb", City = "Zagreb", FoundedYear = 1945, Budget = 45.5m, LeagueName = "Supersport HNL"};
        var hajduk = new Club { Id = 2, Name = "HNK Hajduk Split", City = "Split", FoundedYear = 1911, Budget = 28.3m, LeagueName = "Supersport HNL"};
        var rijeka = new Club { Id = 3, Name = "HNK Rijeka", City = "Rijeka", FoundedYear = 1946, Budget = 15.7m, LeagueName = "Supersport HNL"};
        Clubs = new List<Club> { dinamo, hajduk, rijeka };

        // --- TRENERI ---
        var trDinamo = new Coach { Id = 1, FirstName = "Sergej", LastName = "Jakirović", Nationality = "Hrvatska", DateOfBirth = new DateTime(1978, 3, 15), ContractUntil = new DateTime(2026, 6, 30)};
        var trHajduk = new Coach { Id = 2, FirstName = "Gennaro", LastName = "Gattuso", Nationality = "Italija", DateOfBirth = new DateTime(1978, 1, 9), ContractUntil = new DateTime(2026, 6, 30)};
        var trRijeka = new Coach { Id = 3, FirstName = "Željko", LastName = "Sopić", Nationality = "Hrvatska", DateOfBirth = new DateTime(1972, 7, 20), ContractUntil = new DateTime(2025, 12, 31)};
        dinamo.Coach = trDinamo; hajduk.Coach = trHajduk; rijeka.Coach = trRijeka;
        Coaches = new List<Coach> { trDinamo, trHajduk, trRijeka };

        // --- IGRAČI ---
        var livakovic = new Player { Id = 1, FirstName = "Dominik", LastName = "Livaković", DateOfBirth = new DateTime(1995, 1, 9), Nationality = "Hrvatska", Position = PlayerPosition.Goalkeeper, JerseyNumber = 1, MarketValue = 20.0m, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = dinamo };
        var sutalo = new Player { Id = 2, FirstName = "Josip", LastName = "Šutalo", DateOfBirth = new DateTime(2000, 2, 28), Nationality = "Hrvatska", Position = PlayerPosition.Defender, JerseyNumber = 5, MarketValue = 18.0m, ContractUntil = new DateTime(2028, 6, 30), IsInjured = false, Club = dinamo };
        var ivanusec = new Player { Id = 3, FirstName = "Luka", LastName = "Ivanušec", DateOfBirth = new DateTime(1998, 11, 26), Nationality = "Hrvatska", Position = PlayerPosition.Midfielder, JerseyNumber = 10, MarketValue = 12.0m, ContractUntil = new DateTime(2026, 6, 30), IsInjured = false, Club = dinamo };
        var petkovic = new Player { Id = 4, FirstName = "Bruno", LastName = "Petković", DateOfBirth = new DateTime(1994, 9, 16), Nationality = "Hrvatska", Position = PlayerPosition.Forward, JerseyNumber = 9, MarketValue = 10.0m, ContractUntil = new DateTime(2026, 6, 30), IsInjured = true, Club = dinamo };
        var posavec = new Player { Id = 5, FirstName = "Lovre", LastName = "Posavec", DateOfBirth = new DateTime(1994, 4, 4), Nationality = "Hrvatska", Position = PlayerPosition.Goalkeeper, JerseyNumber = 1, MarketValue = 3.5m, ContractUntil = new DateTime(2026, 6, 30), IsInjured = false, Club = hajduk };
        var kalik = new Player { Id = 6, FirstName = "Stipe", LastName = "Kalik", DateOfBirth = new DateTime(1997, 5, 12), Nationality = "Hrvatska", Position = PlayerPosition.Defender, JerseyNumber = 4, MarketValue = 2.0m, ContractUntil = new DateTime(2025, 12, 31), IsInjured = false, Club = hajduk };
        var durdov = new Player { Id = 7, FirstName = "Ivan", LastName = "Đurđev", DateOfBirth = new DateTime(2003, 8, 15), Nationality = "Hrvatska", Position = PlayerPosition.Forward, JerseyNumber = 11, MarketValue = 8.0m, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = hajduk };
        var nevistic = new Player { Id = 8, FirstName = "Rauno", LastName = "Nevistić", DateOfBirth = new DateTime(1997, 3, 23), Nationality = "Hrvatska", Position = PlayerPosition.Goalkeeper, JerseyNumber = 1, MarketValue = 1.5m, ContractUntil = new DateTime(2026, 6, 30), IsInjured = false, Club = rijeka };
        var tomecak = new Player { Id = 9, FirstName = "Nino", LastName = "Tomečak", DateOfBirth = new DateTime(1999, 6, 30), Nationality = "Hrvatska", Position = PlayerPosition.Midfielder, JerseyNumber = 8, MarketValue = 2.5m, ContractUntil = new DateTime(2026, 12, 31), IsInjured = false, Club = rijeka };
        var muric = new Player { Id = 10, FirstName = "Stjepan", LastName = "Murić", DateOfBirth = new DateTime(2001, 11, 5), Nationality = "Hrvatska", Position = PlayerPosition.Forward, JerseyNumber = 19, MarketValue = 1.8m, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = rijeka };

        dinamo.Players.AddRange(new[] { livakovic, sutalo, ivanusec, petkovic });
        hajduk.Players.AddRange(new[] { posavec, kalik, durdov });
        rijeka.Players.AddRange(new[] { nevistic, tomecak, muric });
        Players = new List<Player> { livakovic, sutalo, ivanusec, petkovic, posavec, kalik, durdov, nevistic, tomecak, muric };

        // --- UTAKMICE ---
        var m1 = new Match { Id = 1, Date = new DateTime(2025, 9, 14, 20, 0, 0), HomeClub = dinamo, AwayClub = hajduk, HomeScore = 2, AwayScore = 1, Stadium = maksimir, Status = MatchStatus.Finished, Attendance = 30_000, Referee = "Fran Jović", Round = "Kolo 5" };
        var m2 = new Match { Id = 2, Date = new DateTime(2025, 10, 5, 18, 0, 0), HomeClub = hajduk, AwayClub = rijeka, HomeScore = 3, AwayScore = 0, Stadium = poljud, Status = MatchStatus.Finished, Attendance = 25_000, Referee = "Mario Zebić", Round = "Kolo 8" };
        var m3 = new Match { Id = 3, Date = new DateTime(2025, 11, 22, 17, 0, 0), HomeClub = rijeka, AwayClub = dinamo, HomeScore = 1, AwayScore = 1, Stadium = rijekaSt, Status = MatchStatus.Finished, Attendance = 7_500, Referee = "Tomislav Šuperina", Round = "Kolo 12" };
        var m4 = new Match { Id = 4, Date = new DateTime(2026, 4, 20, 20, 0, 0), HomeClub = dinamo, AwayClub = rijeka, HomeScore = 0, AwayScore = 0, Stadium = maksimir, Status = MatchStatus.Scheduled, Attendance = 0, Referee = "TBD", Round = "Kolo 25" };

        dinamo.HomeMatches.AddRange(new[] { m1, m4 }); dinamo.AwayMatches.Add(m3);
        hajduk.HomeMatches.Add(m2); hajduk.AwayMatches.Add(m1);
        rijeka.HomeMatches.Add(m3); rijeka.AwayMatches.AddRange(new[] { m2, m4 });
        Matches = new List<Match> { m1, m2, m3, m4 };

        // --- STATISTIKE ---
        var s1 = new PlayerStat { Id = 1, Player = ivanusec, Match = m1, Goals = 1, Assists = 1, MinutesPlayed = 90, YellowCards = 0, RedCard = false, Rating = 8.5 };
        var s2 = new PlayerStat { Id = 2, Player = petkovic, Match = m1, Goals = 1, Assists = 0, MinutesPlayed = 80, YellowCards = 1, RedCard = false, Rating = 7.8 };
        var s3 = new PlayerStat { Id = 3, Player = durdov, Match = m1, Goals = 1, Assists = 0, MinutesPlayed = 90, YellowCards = 0, RedCard = false, Rating = 7.2 };
        var s4 = new PlayerStat { Id = 4, Player = durdov, Match = m2, Goals = 2, Assists = 1, MinutesPlayed = 90, YellowCards = 0, RedCard = false, Rating = 9.1 };
        var s5 = new PlayerStat { Id = 5, Player = kalik, Match = m2, Goals = 1, Assists = 0, MinutesPlayed = 90, YellowCards = 1, RedCard = false, Rating = 7.5 };
        var s6 = new PlayerStat { Id = 6, Player = tomecak, Match = m2, Goals = 0, Assists = 0, MinutesPlayed = 60, YellowCards = 2, RedCard = true, Rating = 4.5 };
        var s7 = new PlayerStat { Id = 7, Player = ivanusec, Match = m3, Goals = 0, Assists = 1, MinutesPlayed = 90, YellowCards = 0, RedCard = false, Rating = 7.0 };
        var s8 = new PlayerStat { Id = 8, Player = muric, Match = m3, Goals = 1, Assists = 0, MinutesPlayed = 85, YellowCards = 0, RedCard = false, Rating = 7.9 };

        m1.PlayerStats.AddRange(new[] { s1, s2, s3 }); m2.PlayerStats.AddRange(new[] { s4, s5, s6 }); m3.PlayerStats.AddRange(new[] { s7, s8 });
        ivanusec.Stats.AddRange(new[] { s1, s7 }); petkovic.Stats.Add(s2); durdov.Stats.AddRange(new[] { s3, s4 });
        kalik.Stats.Add(s5); tomecak.Stats.Add(s6); muric.Stats.Add(s8);
        PlayerStats = new List<PlayerStat> { s1, s2, s3, s4, s5, s6, s7, s8 };

        // --- TRANSFERI ---
        var t1 = new Transfer { Id = 1, Player = livakovic, FromClub = dinamo, ToClub = dinamo, TransferDate = new DateTime(2022, 7, 1), Fee = 0m };
        var t2 = new Transfer { Id = 2, Player = petkovic, FromClub = hajduk, ToClub = dinamo, TransferDate = new DateTime(2023, 1, 31), Fee = 4.5m };
        livakovic.Transfers.Add(t1); petkovic.Transfers.Add(t2);
        Transfers = new List<Transfer> { t1, t2 };
    }
}