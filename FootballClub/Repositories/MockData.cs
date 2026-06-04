using FootballClub.Models;
using FootballClub.Models.Enums;
namespace FootballClub.Repositories;

/// <summary>
/// Centralna klasa sa statičkim seed podacima za bazu.
/// Koristi se od strane DataSeeder-a.
/// </summary>
public static class MockData
{
    public static readonly List<Stadium> Stadiums;
    public static readonly List<Club> Clubs;
    public static readonly List<Staff> StaffMembers;
    public static readonly List<Player> Players;
    public static readonly List<Match> Matches;
    public static readonly List<PlayerStat> PlayerStats;
    public static readonly List<Transfer> Transfers;
    public static readonly List<TrainingSession> TrainingSessions;
    public static readonly List<PlayerScheduleItem> PlayerScheduleItems;
    public static readonly List<LeagueStanding> LeagueStandings;

    static MockData()
    {
        // Inicijalizacija stadiona
        Stadiums = new List<Stadium>
        {
            new Stadium { Name = "Maksimir", City = "Zagreb", Capacity = 37000, YearBuilt = 1912 },
            new Stadium { Name = "Poljud", City = "Split", Capacity = 35000, YearBuilt = 1979 },
            new Stadium { Name = "Stadion nt Kardinala Alojzija Stepinca", City = "Zagreb", Capacity = 19000, YearBuilt = 2016 },
            new Stadium { Name = "Stadion Rujevica", City = "Rijeka", Capacity = 16000, YearBuilt = 1947 },
            new Stadium { Name = "Gradski vrt", City = "Osijek", Capacity = 23000, YearBuilt = 1963 }
        };

        // Inicijalizacija klubova
        Clubs = new List<Club>
        {
            new Club { Name = "Dinamo Zagreb", City = "Zagreb", FoundedYear = 1945, Budget = 150, LeagueName = "HNL", HomeStadium = Stadiums[0] },
            new Club { Name = "HNK Hajduk Split", City = "Split", FoundedYear = 1911, Budget = 80, LeagueName = "HNL", HomeStadium = Stadiums[1] },
            new Club { Name = "NK Slaven Belupo", City = "Koprivnica", FoundedYear = 1916, Budget = 30, LeagueName = "HNL", HomeStadium = Stadiums[2] },
            new Club { Name = "HNK Rijeka", City = "Rijeka", FoundedYear = 1899, Budget = 45, LeagueName = "HNL", HomeStadium = Stadiums[3] },
            new Club { Name = "NK Osijek", City = "Osijek", FoundedYear = 1945, Budget = 35, LeagueName = "HNL", HomeStadium = Stadiums[4] }
        };

        // Inicijalizacija trenera (sa istorijom za Dinamo)
        StaffMembers = new List<Staff>
        {
            // Dinamo Zagreb - trenutni
            new Staff { FirstName = "Miodrag", LastName = "Radulović", Nationality = "Serbian", DateOfBirth = new DateTime(1965, 3, 15), ContractUntil = new DateTime(2027, 6, 30), Role = "Head Coach", Club = Clubs[0] },
            // Dinamo Zagreb - prošli treneri
            new Staff { FirstName = "Damir", LastName = "Krznar", Nationality = "Croatian", DateOfBirth = new DateTime(1973, 8, 22), ContractUntil = new DateTime(2024, 6, 30), Role = "Former Head Coach", Club = Clubs[0] },
            new Staff { FirstName = "Igor", LastName = "Ček", Nationality = "Croatian", DateOfBirth = new DateTime(1968, 5, 10), ContractUntil = new DateTime(2022, 12, 31), Role = "Former Head Coach", Club = Clubs[0] },
            new Staff { FirstName = "Zoran", LastName = "Mamić", Nationality = "Croatian", DateOfBirth = new DateTime(1966, 1, 15), ContractUntil = new DateTime(2020, 6, 30), Role = "Former Head Coach", Club = Clubs[0] },
            
            // Ostali klubovi
            new Staff { FirstName = "Nenad", LastName = "Čancar", Nationality = "Croatian", DateOfBirth = new DateTime(1970, 7, 22), ContractUntil = new DateTime(2027, 12, 31), Role = "Head Coach", Club = Clubs[1] },
            new Staff { FirstName = "Mario", LastName = "Carević", Nationality = "Croatian", DateOfBirth = new DateTime(1975, 5, 10), ContractUntil = new DateTime(2026, 12, 31), Role = "Head Coach", Club = Clubs[2] },
            new Staff { FirstName = "Gennaro", LastName = "Gattuso", Nationality = "Italian", DateOfBirth = new DateTime(1978, 2, 9), ContractUntil = new DateTime(2027, 6, 30), Role = "Head Coach", Club = Clubs[3] },
            new Staff { FirstName = "Nenad", LastName = "Bjelica", Nationality = "Serbian", DateOfBirth = new DateTime(1972, 11, 30), ContractUntil = new DateTime(2027, 3, 31), Role = "Head Coach", Club = Clubs[4] }
        };

        // Inicijalizacija igrača
        Players = new List<Player>
        {
            // Dinamo Zagreb igrači
            new Player { FirstName = "Dominik", LastName = "Livaković", Nationality = "Croatian", DateOfBirth = new DateTime(1997, 1, 6), Position = PlayerPosition.Goalkeeper, JerseyNumber = 1, MarketValue = 35, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = Clubs[0] },
            new Player { FirstName = "Arijan", LastName = "Ademi", Nationality = "Macedonian", DateOfBirth = new DateTime(1994, 11, 2), Position = PlayerPosition.Midfielder, JerseyNumber = 8, MarketValue = 12, ContractUntil = new DateTime(2026, 12, 31), IsInjured = false, Club = Clubs[0] },
            new Player { FirstName = "Stefan", LastName = "Ristovski", Nationality = "Macedonian", DateOfBirth = new DateTime(1993, 2, 1), Position = PlayerPosition.Defender, JerseyNumber = 2, MarketValue = 8, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = Clubs[0] },
            new Player { FirstName = "Bruno", LastName = "Petković", Nationality = "Croatian", DateOfBirth = new DateTime(1996, 10, 24), Position = PlayerPosition.Forward, JerseyNumber = 9, MarketValue = 28, ContractUntil = new DateTime(2027, 12, 31), IsInjured = false, Club = Clubs[0] },
            new Player { FirstName = "Serhii", LastName = "Sydorchuk", Nationality = "Ukrainian", DateOfBirth = new DateTime(1992, 10, 31), Position = PlayerPosition.Midfielder, JerseyNumber = 16, MarketValue = 9, ContractUntil = new DateTime(2026, 6, 30), IsInjured = false, Club = Clubs[0] },
            new Player { FirstName = "Petar", LastName = "Sarlija", Nationality = "Croatian", DateOfBirth = new DateTime(1998, 7, 12), Position = PlayerPosition.Defender, JerseyNumber = 5, MarketValue = 10, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = Clubs[0] },
            new Player { FirstName = "Mislav", LastName = "Oršić", Nationality = "Croatian", DateOfBirth = new DateTime(1992, 2, 16), Position = PlayerPosition.Forward, JerseyNumber = 17, MarketValue = 18, ContractUntil = new DateTime(2027, 12, 31), IsInjured = true, Club = Clubs[0] },
            
            // Hajduk Split igrači
            new Player { FirstName = "Amir", LastName = "Nikolić", Nationality = "Bosnian", DateOfBirth = new DateTime(1995, 4, 20), Position = PlayerPosition.Forward, JerseyNumber = 10, MarketValue = 22, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = Clubs[1] },
            new Player { FirstName = "Mario", LastName = "Ćuže", Nationality = "Croatian", DateOfBirth = new DateTime(1999, 6, 10), Position = PlayerPosition.Midfielder, JerseyNumber = 7, MarketValue = 16, ContractUntil = new DateTime(2027, 12, 31), IsInjured = false, Club = Clubs[1] },
            new Player { FirstName = "Josip", LastName = "Stanišić", Nationality = "Croatian", DateOfBirth = new DateTime(1998, 12, 9), Position = PlayerPosition.Defender, JerseyNumber = 23, MarketValue = 18, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = Clubs[1] },
            
            // Slaven Belupo igrači
            new Player { FirstName = "Lovre", LastName = "Kalinić", Nationality = "Croatian", DateOfBirth = new DateTime(2000, 3, 5), Position = PlayerPosition.Forward, JerseyNumber = 9, MarketValue = 8, ContractUntil = new DateTime(2026, 6, 30), IsInjured = false, Club = Clubs[2] },
            
            // Rijeka igrači
            new Player { FirstName = "Marko", LastName = "Bulj", Nationality = "Croatian", DateOfBirth = new DateTime(1992, 11, 26), Position = PlayerPosition.Forward, JerseyNumber = 19, MarketValue = 9, ContractUntil = new DateTime(2026, 12, 31), IsInjured = false, Club = Clubs[3] }
        };

        // Inicijalizacija utakmica
        Matches = new List<Match>
        {
            new Match { Date = new DateTime(2026, 4, 15, 19, 0, 0), HomeClub = Clubs[0], AwayClub = Clubs[1], HomeScore = 3, AwayScore = 2, Stadium = Stadiums[0], Status = MatchStatus.Finished, Attendance = 35000, Referee = "Damir Skomina", Round = "Kolo 28" },
            new Match { Date = new DateTime(2026, 4, 20, 19, 0, 0), HomeClub = Clubs[1], AwayClub = Clubs[0], HomeScore = 1, AwayScore = 2, Stadium = Stadiums[1], Status = MatchStatus.Finished, Attendance = 33000, Referee = "Marko Nikolić", Round = "Kolo 29" },
            new Match { Date = new DateTime(2026, 5, 10, 19, 0, 0), HomeClub = Clubs[0], AwayClub = Clubs[2], HomeScore = 4, AwayScore = 1, Stadium = Stadiums[0], Status = MatchStatus.Finished, Attendance = 32000, Referee = "Slavko Vinčić", Round = "Kolo 30" },
            new Match { Date = new DateTime(2026, 5, 18, 19, 0, 0), HomeClub = Clubs[3], AwayClub = Clubs[0], HomeScore = 0, AwayScore = 2, Stadium = Stadiums[3], Status = MatchStatus.Finished, Attendance = 15000, Referee = "Nenad Mišković", Round = "Kolo 31" }
        };

        // Inicijalizacija igračkih statistika
        PlayerStats = new List<PlayerStat>
        {
            new PlayerStat { Player = Players[3], Match = Matches[0], MinutesPlayed = 90, Goals = 2, Assists = 0, YellowCards = 0, RedCard = false, Rating = 8.5 },
            new PlayerStat { Player = Players[5], Match = Matches[0], MinutesPlayed = 85, Goals = 2, Assists = 0, YellowCards = 1, RedCard = false, Rating = 7.8 },
            new PlayerStat { Player = Players[1], Match = Matches[1], MinutesPlayed = 90, Goals = 1, Assists = 1, YellowCards = 0, RedCard = false, Rating = 7.5 },
            new PlayerStat { Player = Players[3], Match = Matches[2], MinutesPlayed = 70, Goals = 2, Assists = 1, YellowCards = 1, RedCard = false, Rating = 8.2 },
            new PlayerStat { Player = Players[3], Match = Matches[3], MinutesPlayed = 90, Goals = 1, Assists = 0, YellowCards = 0, RedCard = false, Rating = 7.9 }
        };

        // Inicijalizacija transfera
        Transfers = new List<Transfer>
        {
            new Transfer { Player = Players[3], FromClub = Clubs[1], ToClub = Clubs[0], TransferDate = new DateTime(2023, 7, 15), Fee = 18 },
            new Transfer { Player = Players[6], FromClub = Clubs[0], ToClub = Clubs[1], TransferDate = new DateTime(2024, 1, 20), Fee = 12 }
        };

        // Inicijalizacija treninških sesija
        TrainingSessions = new List<TrainingSession>
        {
            new TrainingSession
            {
                Club = Clubs[0],
                Title = "Taktička prijava",
                FocusArea = "Pozicijska igra",
                StartTime = new DateTime(2026, 5, 8, 10, 0, 0),
                EndTime = new DateTime(2026, 5, 8, 12, 0, 0),
                Location = "Maksimir",
                Intensity = TrainingIntensity.High,
                LeadStaff = StaffMembers[0],
                Participants = new List<Player> { Players[0], Players[1], Players[2], Players[3], Players[4] },
                Notes = "Priprema za nadolazeću utakmicu"
            },
            new TrainingSession
            {
                Club = Clubs[0],
                Title = "Opuštajući trening",
                FocusArea = "Oporavak i fleksibilnost",
                StartTime = new DateTime(2026, 5, 9, 15, 0, 0),
                EndTime = new DateTime(2026, 5, 9, 16, 30, 0),
                Location = "Maksimir",
                Intensity = TrainingIntensity.Recovery,
                LeadStaff = StaffMembers[0],
                Participants = new List<Player> { Players[5], Players[6] },
                Notes = "Oporavak unatoč ozljedi igrača 6"
            },
            new TrainingSession
            {
                Club = Clubs[1],
                Title = "Offensivni set-pieces",
                FocusArea = "Korneri i slobodni udarci",
                StartTime = new DateTime(2026, 5, 8, 11, 0, 0),
                EndTime = new DateTime(2026, 5, 8, 12, 30, 0),
                Location = "Poljud",
                Intensity = TrainingIntensity.Moderate,
                LeadStaff = StaffMembers[4],
                Participants = new List<Player> { Players[8], Players[9], Players[10] },
                Notes = "Fokus na kombinacijske igrače"
            }
        };

        // Inicijalizacija igrača raspored
        PlayerScheduleItems = new List<PlayerScheduleItem>
        {
            new PlayerScheduleItem
            {
                Player = Players[3],
                StartTime = new DateTime(2026, 5, 10, 18, 0, 0),
                EndTime = new DateTime(2026, 5, 10, 21, 0, 0),
                ResponsibilityType = ScheduleResponsibilityType.PerformanceReview,
                Title = "Utakmica Dinamo - Slaven Belupo",
                Location = "Maksimir",
                AssignedBy = "Miodrag Radulović",
                Notes = "Važna utakmica za ligu"
            },
            new PlayerScheduleItem
            {
                Player = Players[6],
                StartTime = new DateTime(2026, 5, 7, 14, 0, 0),
                EndTime = new DateTime(2026, 5, 7, 16, 0, 0),
                ResponsibilityType = ScheduleResponsibilityType.PhysicalTherapy,
                Title = "Rehabilitacija ozljede",
                Location = "Klinika Dinamo",
                AssignedBy = "Medicinski tim",
                Notes = "Ozljeda mišića, limitiran angažman"
            },
            new PlayerScheduleItem
            {
                Player = Players[0],
                StartTime = new DateTime(2026, 5, 8, 10, 0, 0),
                EndTime = new DateTime(2026, 5, 8, 12, 0, 0),
                ResponsibilityType = ScheduleResponsibilityType.RegularTraining,
                Title = "Taktička prijava",
                Location = "Maksimir",
                AssignedBy = "Miodrag Radulović",
                Notes = "Obavezno prisustvo"
            }
        };

        // Inicijalizacija ligaške tabele
        LeagueStandings = new List<LeagueStanding>
        {
            new LeagueStanding { Club = Clubs[0], Played = 31, Wins = 22, Draws = 5, Losses = 4, GoalsFor = 68, GoalsAgainst = 28, Points = 71 },
            new LeagueStanding { Club = Clubs[1], Played = 31, Wins = 19, Draws = 4, Losses = 8, GoalsFor = 59, GoalsAgainst = 32, Points = 61 },
            new LeagueStanding { Club = Clubs[3], Played = 31, Wins = 16, Draws = 7, Losses = 8, GoalsFor = 51, GoalsAgainst = 38, Points = 55 },
            new LeagueStanding { Club = Clubs[4], Played = 31, Wins = 14, Draws = 6, Losses = 11, GoalsFor = 45, GoalsAgainst = 42, Points = 48 },
            new LeagueStanding { Club = Clubs[2], Played = 31, Wins = 10, Draws = 5, Losses = 16, GoalsFor = 38, GoalsAgainst = 51, Points = 35 }
        };
    }
}
