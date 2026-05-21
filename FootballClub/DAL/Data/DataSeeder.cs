using FootballClub.Models;
using FootballClub.Models.Enums;

namespace FootballClub.Data;

public static class DataSeeder
{
    public static void SeedDatabase(ApplicationDbContext context)
    {
        // If players exist we assume database already seeded; checking Players is safer
        // than checking Stadiums because partial seeding could leave stadiums present
        // while other tables remain empty.
        if (context.Players.Any()) return;

        try
        {
            // Clear any tracking to avoid ID assignment conflicts
            context.ChangeTracker.Clear();

            // Create and seed stadiums first (no relationships)
            var stadiums = new[]
            {
                new Stadium { Name = "Maksimir", City = "Zagreb", Capacity = 37000, YearBuilt = 1912 },
                new Stadium { Name = "Poljud", City = "Split", Capacity = 35000, YearBuilt = 1979 },
                new Stadium { Name = "Stadion nt Kardinala Alojzija Stepinca", City = "Zagreb", Capacity = 19000, YearBuilt = 2016 },
                new Stadium { Name = "Stadion Rujevica", City = "Rijeka", Capacity = 16000, YearBuilt = 1947 },
                new Stadium { Name = "Gradski vrt", City = "Osijek", Capacity = 23000, YearBuilt = 1963 }
            };
            context.Stadiums.AddRange(stadiums);
            context.SaveChanges();

            // Reload stadiums to get their IDs
            var savedStadiums = context.Stadiums.ToList();

            // Create and seed clubs (with loaded stadium references)
            var clubs = new[]
            {
                new Club { Name = "Dinamo Zagreb", City = "Zagreb", FoundedYear = 1945, Budget = 150, LeagueName = "HNL", HomeStadium = savedStadiums[0] },
                new Club { Name = "HNK Hajduk Split", City = "Split", FoundedYear = 1911, Budget = 80, LeagueName = "HNL", HomeStadium = savedStadiums[1] },
                new Club { Name = "NK Slaven Belupo", City = "Koprivnica", FoundedYear = 1916, Budget = 30, LeagueName = "HNL", HomeStadium = savedStadiums[2] },
                new Club { Name = "HNK Rijeka", City = "Rijeka", FoundedYear = 1899, Budget = 45, LeagueName = "HNL", HomeStadium = savedStadiums[3] },
                new Club { Name = "NK Osijek", City = "Osijek", FoundedYear = 1945, Budget = 35, LeagueName = "HNL", HomeStadium = savedStadiums[4] }
            };
            context.Clubs.AddRange(clubs);
            context.SaveChanges();

            // Reload clubs to get their IDs
            var savedClubs = context.Clubs.ToList();

            // Create and seed staff members (allow multiple per club)
            var staffRaw = new[]
            {
                new Staff { FirstName = "Miodrag", LastName = "Radulović", Nationality = "Serbian", DateOfBirth = new DateTime(1965, 3, 15), ContractUntil = new DateTime(2027, 6, 30), Role = "Head Coach", Club = savedClubs[0] },
                new Staff { FirstName = "Damir", LastName = "Krznar", Nationality = "Croatian", DateOfBirth = new DateTime(1973, 8, 22), ContractUntil = new DateTime(2027, 6, 30), Role = "Assistant Coach", Club = savedClubs[0] },
                new Staff { FirstName = "Ivana", LastName = "Horvat", Nationality = "Croatian", DateOfBirth = new DateTime(1982, 4, 12), ContractUntil = new DateTime(2026, 6, 30), Role = "Physio", Club = savedClubs[0] },
                new Staff { FirstName = "Stjepan", LastName = "Rogić", Nationality = "Croatian", DateOfBirth = new DateTime(1978, 11, 4), ContractUntil = new DateTime(2027, 6, 30), Role = "Goalkeeping Coach", Club = savedClubs[0] },
                new Staff { FirstName = "Petra", LastName = "Kovač", Nationality = "Croatian", DateOfBirth = new DateTime(1989, 2, 18), ContractUntil = new DateTime(2026, 12, 31), Role = "Fitness Coach", Club = savedClubs[0] },
                new Staff { FirstName = "Luka", LastName = "Babić", Nationality = "Croatian", DateOfBirth = new DateTime(1991, 9, 6), ContractUntil = new DateTime(2027, 6, 30), Role = "Performance Analyst", Club = savedClubs[0] },
                new Staff { FirstName = "Maja", LastName = "Mlinar", Nationality = "Croatian", DateOfBirth = new DateTime(1984, 1, 29), ContractUntil = new DateTime(2026, 12, 31), Role = "Team Doctor", Club = savedClubs[0] },
                new Staff { FirstName = "Tomislav", LastName = "Vukelić", Nationality = "Croatian", DateOfBirth = new DateTime(1976, 5, 17), ContractUntil = new DateTime(2027, 6, 30), Role = "Scout", Club = savedClubs[0] },
                // Other clubs - add multiple staff to reach ~10 new staff with different roles
                new Staff { FirstName = "Nenad", LastName = "Čancar", Nationality = "Croatian", DateOfBirth = new DateTime(1970, 7, 22), ContractUntil = new DateTime(2027, 12, 31), Role = "Head Coach", Club = savedClubs[1] },
                new Staff { FirstName = "Mate", LastName = "Kovač", Nationality = "Croatian", DateOfBirth = new DateTime(1980, 3, 1), ContractUntil = new DateTime(2026, 12, 31), Role = "Assistant Coach", Club = savedClubs[1] },
                new Staff { FirstName = "Ana", LastName = "Barić", Nationality = "Croatian", DateOfBirth = new DateTime(1987, 9, 10), ContractUntil = new DateTime(2026, 6, 30), Role = "Physio", Club = savedClubs[1] },
                new Staff { FirstName = "Mario", LastName = "Carević", Nationality = "Croatian", DateOfBirth = new DateTime(1975, 5, 10), ContractUntil = new DateTime(2026, 12, 31), Role = "Head Coach", Club = savedClubs[2] },
                new Staff { FirstName = "Luka", LastName = "Perić", Nationality = "Croatian", DateOfBirth = new DateTime(1985, 2, 20), ContractUntil = new DateTime(2027, 6, 30), Role = "Assistant Coach", Club = savedClubs[2] },
                new Staff { FirstName = "Gennaro", LastName = "Gattuso", Nationality = "Italian", DateOfBirth = new DateTime(1978, 2, 9), ContractUntil = new DateTime(2027, 6, 30), Role = "Head Coach", Club = savedClubs[3] },
                new Staff { FirstName = "Ivica", LastName = "Radetić", Nationality = "Croatian", DateOfBirth = new DateTime(1982, 6, 6), ContractUntil = new DateTime(2025, 6, 30), Role = "Physio", Club = savedClubs[3] },
                new Staff { FirstName = "Nenad", LastName = "Bjelica", Nationality = "Serbian", DateOfBirth = new DateTime(1972, 11, 30), ContractUntil = new DateTime(2027, 3, 31), Role = "Head Coach", Club = savedClubs[4] },
                new Staff { FirstName = "Marko", LastName = "Jurić", Nationality = "Croatian", DateOfBirth = new DateTime(1990, 12, 11), ContractUntil = new DateTime(2026, 12, 31), Role = "Assistant Coach", Club = savedClubs[4] }
            };

            // Insert staff members if they don't already exist (idempotent by name)
            var staffToInsert = staffRaw
                .Where(c => !context.StaffMembers.Any(existing => existing.FirstName == c.FirstName && existing.LastName == c.LastName))
                .ToList();

            if (staffToInsert.Any())
            {
                context.StaffMembers.AddRange(staffToInsert);
                context.SaveChanges();
            }

            // Reload staff to get their IDs
            var savedStaff = context.StaffMembers.ToList();

            // Create and seed players
            var players = new[]
            {
                // Dinamo Zagreb - 25 players
                new Player { FirstName = "Dominik", LastName = "Livaković", Nationality = "Croatian", DateOfBirth = new DateTime(1997, 1, 6), Position = PlayerPosition.Goalkeeper, JerseyNumber = 1, MarketValue = 35, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Dani", LastName = "Olmo", Nationality = "Spanish", DateOfBirth = new DateTime(1998, 5, 7), Position = PlayerPosition.Midfielder, JerseyNumber = 20, MarketValue = 32, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Stefan", LastName = "Ristovski", Nationality = "Macedonian", DateOfBirth = new DateTime(1993, 2, 1), Position = PlayerPosition.Defender, JerseyNumber = 2, MarketValue = 8, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Bruno", LastName = "Petković", Nationality = "Croatian", DateOfBirth = new DateTime(1996, 10, 24), Position = PlayerPosition.Forward, JerseyNumber = 9, MarketValue = 28, ContractUntil = new DateTime(2027, 12, 31), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Serhii", LastName = "Sydorchuk", Nationality = "Ukrainian", DateOfBirth = new DateTime(1992, 10, 31), Position = PlayerPosition.Midfielder, JerseyNumber = 16, MarketValue = 9, ContractUntil = new DateTime(2026, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Petar", LastName = "Sarlija", Nationality = "Croatian", DateOfBirth = new DateTime(1998, 7, 12), Position = PlayerPosition.Defender, JerseyNumber = 5, MarketValue = 10, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Mislav", LastName = "Oršić", Nationality = "Croatian", DateOfBirth = new DateTime(1992, 2, 16), Position = PlayerPosition.Forward, JerseyNumber = 17, MarketValue = 18, ContractUntil = new DateTime(2027, 12, 31), IsInjured = true, Club = savedClubs[0] },
                new Player { FirstName = "Arijan", LastName = "Ademi", Nationality = "Macedonian", DateOfBirth = new DateTime(1994, 11, 2), Position = PlayerPosition.Midfielder, JerseyNumber = 8, MarketValue = 12, ContractUntil = new DateTime(2026, 12, 31), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Josip", LastName = "Pralija", Nationality = "Croatian", DateOfBirth = new DateTime(1999, 3, 18), Position = PlayerPosition.Defender, JerseyNumber = 3, MarketValue = 11, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Marko", LastName = "Čeljević", Nationality = "Croatian", DateOfBirth = new DateTime(2000, 8, 25), Position = PlayerPosition.Midfielder, JerseyNumber = 6, MarketValue = 9, ContractUntil = new DateTime(2027, 12, 31), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Mahir", LastName = "Emreli", Nationality = "Azerbaijani", DateOfBirth = new DateTime(1996, 6, 13), Position = PlayerPosition.Forward, JerseyNumber = 11, MarketValue = 15, ContractUntil = new DateTime(2026, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Emir", LastName = "Dilaver", Nationality = "Bosnian", DateOfBirth = new DateTime(1998, 11, 7), Position = PlayerPosition.Defender, JerseyNumber = 4, MarketValue = 12, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Kristijan", LastName = "Jakić", Nationality = "Croatian", DateOfBirth = new DateTime(1995, 12, 20), Position = PlayerPosition.Midfielder, JerseyNumber = 14, MarketValue = 14, ContractUntil = new DateTime(2026, 12, 31), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Josip", LastName = "Juranović", Nationality = "Croatian", DateOfBirth = new DateTime(1995, 5, 22), Position = PlayerPosition.Defender, JerseyNumber = 7, MarketValue = 13, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Stefan", LastName = "Ristovski", Nationality = "Macedonian", DateOfBirth = new DateTime(1996, 7, 14), Position = PlayerPosition.Defender, JerseyNumber = 22, MarketValue = 11, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Gianluigi", LastName = "Buffon", Nationality = "Italian", DateOfBirth = new DateTime(1978, 1, 28), Position = PlayerPosition.Goalkeeper, JerseyNumber = 77, MarketValue = 2, ContractUntil = new DateTime(2025, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Lovro", LastName = "Majer", Nationality = "Croatian", DateOfBirth = new DateTime(1998, 11, 6), Position = PlayerPosition.Midfielder, JerseyNumber = 10, MarketValue = 24, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Mihajlo", LastName = "Ristić", Nationality = "Serbian", DateOfBirth = new DateTime(1994, 2, 10), Position = PlayerPosition.Defender, JerseyNumber = 23, MarketValue = 7, ContractUntil = new DateTime(2026, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Roko", LastName = "Stiperski", Nationality = "Croatian", DateOfBirth = new DateTime(2001, 6, 15), Position = PlayerPosition.Forward, JerseyNumber = 13, MarketValue = 8, ContractUntil = new DateTime(2027, 12, 31), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Luka", LastName = "Ivanović", Nationality = "Croatian", DateOfBirth = new DateTime(1999, 9, 22), Position = PlayerPosition.Midfielder, JerseyNumber = 18, MarketValue = 10, ContractUntil = new DateTime(2026, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Zvjezdan", LastName = "Misimović", Nationality = "Bosnian", DateOfBirth = new DateTime(1985, 4, 8), Position = PlayerPosition.Midfielder, JerseyNumber = 25, MarketValue = 3, ContractUntil = new DateTime(2025, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Robert", LastName = "Kovač", Nationality = "Croatian", DateOfBirth = new DateTime(1997, 11, 11), Position = PlayerPosition.Defender, JerseyNumber = 26, MarketValue = 9, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Mesut", LastName = "Özil", Nationality = "German", DateOfBirth = new DateTime(1990, 10, 15), Position = PlayerPosition.Midfielder, JerseyNumber = 12, MarketValue = 8, ContractUntil = new DateTime(2025, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Nemanja", LastName = "Miletić", Nationality = "Serbian", DateOfBirth = new DateTime(2001, 8, 3), Position = PlayerPosition.Forward, JerseyNumber = 27, MarketValue = 6, ContractUntil = new DateTime(2028, 6, 30), IsInjured = false, Club = savedClubs[0] },
                new Player { FirstName = "Dino", LastName = "Hasanović", Nationality = "Bosnian", DateOfBirth = new DateTime(1999, 5, 20), Position = PlayerPosition.Defender, JerseyNumber = 28, MarketValue = 7, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = savedClubs[0] },
                // Hajduk Split
                new Player { FirstName = "Amir", LastName = "Nikolić", Nationality = "Bosnian", DateOfBirth = new DateTime(1995, 4, 20), Position = PlayerPosition.Forward, JerseyNumber = 10, MarketValue = 22, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = savedClubs[1] },
                new Player { FirstName = "Mario", LastName = "Ćuže", Nationality = "Croatian", DateOfBirth = new DateTime(1999, 6, 10), Position = PlayerPosition.Midfielder, JerseyNumber = 7, MarketValue = 16, ContractUntil = new DateTime(2027, 12, 31), IsInjured = false, Club = savedClubs[1] },
                new Player { FirstName = "Josip", LastName = "Stanišić", Nationality = "Croatian", DateOfBirth = new DateTime(1998, 12, 9), Position = PlayerPosition.Defender, JerseyNumber = 23, MarketValue = 18, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = savedClubs[1] },
                // Slaven Belupo
                new Player { FirstName = "Lovre", LastName = "Kalinić", Nationality = "Croatian", DateOfBirth = new DateTime(2000, 3, 5), Position = PlayerPosition.Forward, JerseyNumber = 9, MarketValue = 8, ContractUntil = new DateTime(2026, 6, 30), IsInjured = false, Club = savedClubs[2] },
                // Rijeka
                new Player { FirstName = "Marko", LastName = "Bulj", Nationality = "Croatian", DateOfBirth = new DateTime(1992, 11, 26), Position = PlayerPosition.Forward, JerseyNumber = 19, MarketValue = 9, ContractUntil = new DateTime(2026, 12, 31), IsInjured = false, Club = savedClubs[3] }
            };
            context.Players.AddRange(players);
            context.SaveChanges();

            // Reload players to get their IDs
            var reloadedPlayers = context.Players.ToList();

            // Create and seed matches
            var matches = new[]
            {
                new Match { Date = new DateTime(2026, 4, 15, 19, 0, 0), HomeClub = savedClubs[0], AwayClub = savedClubs[1], HomeScore = 3, AwayScore = 2, Stadium = savedStadiums[0], Status = MatchStatus.Finished, Attendance = 35000, Referee = "Damir Skomina", Round = "Kolo 28" },
                new Match { Date = new DateTime(2026, 4, 20, 19, 0, 0), HomeClub = savedClubs[1], AwayClub = savedClubs[0], HomeScore = 1, AwayScore = 2, Stadium = savedStadiums[1], Status = MatchStatus.Finished, Attendance = 33000, Referee = "Marko Nikolić", Round = "Kolo 29" },
                new Match { Date = new DateTime(2026, 5, 10, 19, 0, 0), HomeClub = savedClubs[0], AwayClub = savedClubs[2], HomeScore = 4, AwayScore = 1, Stadium = savedStadiums[0], Status = MatchStatus.Finished, Attendance = 32000, Referee = "Slavko Vinčić", Round = "Kolo 30" },
                new Match { Date = new DateTime(2026, 5, 18, 19, 0, 0), HomeClub = savedClubs[3], AwayClub = savedClubs[0], HomeScore = 0, AwayScore = 2, Stadium = savedStadiums[3], Status = MatchStatus.Finished, Attendance = 15000, Referee = "Nenad Mišković", Round = "Kolo 31" },
                new Match { Date = new DateTime(2026, 6, 5, 19, 0, 0), HomeClub = savedClubs[0], AwayClub = savedClubs[3], HomeScore = 0, AwayScore = 0, Stadium = savedStadiums[0], Status = MatchStatus.Scheduled, Attendance = 0, Referee = "TBD", Round = "Kolo 32" }
            };
            context.Matches.AddRange(matches);
            context.SaveChanges();

            // Reload matches to get their IDs
            var reloadedMatches = context.Matches.ToList();

            // Create and seed player stats
            var playerStats = new[]
            {
                new PlayerStat { Player = reloadedPlayers[3], Match = reloadedMatches[0], MinutesPlayed = 90, Goals = 2, Assists = 0, YellowCards = 0, RedCard = false, Rating = 8.5 },
                new PlayerStat { Player = reloadedPlayers[5], Match = reloadedMatches[0], MinutesPlayed = 85, Goals = 2, Assists = 0, YellowCards = 1, RedCard = false, Rating = 7.8 },
                new PlayerStat { Player = reloadedPlayers[1], Match = reloadedMatches[1], MinutesPlayed = 90, Goals = 1, Assists = 1, YellowCards = 0, RedCard = false, Rating = 7.5 },
                new PlayerStat { Player = reloadedPlayers[3], Match = reloadedMatches[2], MinutesPlayed = 70, Goals = 2, Assists = 1, YellowCards = 1, RedCard = false, Rating = 8.2 },
                new PlayerStat { Player = reloadedPlayers[3], Match = reloadedMatches[3], MinutesPlayed = 90, Goals = 1, Assists = 0, YellowCards = 0, RedCard = false, Rating = 7.9 }
            };
            context.PlayerStats.AddRange(playerStats);
            context.SaveChanges();

            // Create and seed transfers
            var transfers = new[]
            {
                new Transfer { Player = reloadedPlayers[3], FromClub = savedClubs[1], ToClub = savedClubs[0], TransferDate = new DateTime(2023, 7, 15), Fee = 18 },
                new Transfer { Player = reloadedPlayers[6], FromClub = savedClubs[0], ToClub = savedClubs[1], TransferDate = new DateTime(2024, 1, 20), Fee = 12 }
            };
            context.Transfers.AddRange(transfers);
            context.SaveChanges();

            // Create and seed training sessions
            var trainingSessions = new[]
            {
                new TrainingSession
                {
                    Club = savedClubs[0],
                    Title = "Taktička prijava",
                    FocusArea = "Pozicijska igra",
                    StartTime = new DateTime(2026, 5, 8, 10, 0, 0),
                    EndTime = new DateTime(2026, 5, 8, 12, 0, 0),
                    Location = "Maksimir",
                    Intensity = TrainingIntensity.High,
                    LeadStaff = savedStaff[0],
                    Participants = new List<Player> { reloadedPlayers[0], reloadedPlayers[1], reloadedPlayers[2], reloadedPlayers[3], reloadedPlayers[4] },
                    Notes = "Priprema za nadolazeću utakmicu"
                },
                new TrainingSession
                {
                    Club = savedClubs[0],
                    Title = "Opuštajući trening",
                    FocusArea = "Oporavak i fleksibilnost",
                    StartTime = new DateTime(2026, 5, 9, 15, 0, 0),
                    EndTime = new DateTime(2026, 5, 9, 16, 30, 0),
                    Location = "Maksimir",
                    Intensity = TrainingIntensity.Recovery,
                    LeadStaff = savedStaff[0],
                    Participants = new List<Player> { reloadedPlayers[5], reloadedPlayers[6] },
                    Notes = "Oporavak unatoč ozljedi igrača 6"
                },
                new TrainingSession
                {
                    Club = savedClubs[1],
                    Title = "Offensivni set-pieces",
                    FocusArea = "Korneri i slobodni udarci",
                    StartTime = new DateTime(2026, 5, 8, 11, 0, 0),
                    EndTime = new DateTime(2026, 5, 8, 12, 30, 0),
                    Location = "Poljud",
                    Intensity = TrainingIntensity.Moderate,
                    LeadStaff = savedStaff[4],
                    Participants = new List<Player> { reloadedPlayers[8], reloadedPlayers[9], reloadedPlayers[10] },
                    Notes = "Fokus na kombinacijske igrače"
                }
            };
            context.TrainingSessions.AddRange(trainingSessions);
            context.SaveChanges();

            // Create and seed player schedule items
            var playerScheduleItems = new[]
            {
                new PlayerScheduleItem
                {
                    Player = reloadedPlayers[3],
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
                    Player = reloadedPlayers[6],
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
                    Player = reloadedPlayers[0],
                    StartTime = new DateTime(2026, 5, 8, 10, 0, 0),
                    EndTime = new DateTime(2026, 5, 8, 12, 0, 0),
                    ResponsibilityType = ScheduleResponsibilityType.RegularTraining,
                    Title = "Taktička prijava",
                    Location = "Maksimir",
                    AssignedBy = "Miodrag Radulović",
                    Notes = "Obavezno prisustvo"
                }
            };
            context.PlayerScheduleItems.AddRange(playerScheduleItems);
            context.SaveChanges();

            // Create and seed league standings
            var leagueStandings = new[]
            {
                new LeagueStanding { Club = savedClubs[0], Played = 31, Wins = 22, Draws = 5, Losses = 4, GoalsFor = 68, GoalsAgainst = 28, Points = 71 },
                new LeagueStanding { Club = savedClubs[1], Played = 31, Wins = 19, Draws = 4, Losses = 8, GoalsFor = 59, GoalsAgainst = 32, Points = 61 },
                new LeagueStanding { Club = savedClubs[3], Played = 31, Wins = 16, Draws = 7, Losses = 8, GoalsFor = 51, GoalsAgainst = 38, Points = 55 },
                new LeagueStanding { Club = savedClubs[4], Played = 31, Wins = 14, Draws = 6, Losses = 11, GoalsFor = 45, GoalsAgainst = 42, Points = 48 },
                new LeagueStanding { Club = savedClubs[2], Played = 31, Wins = 10, Draws = 5, Losses = 16, GoalsFor = 38, GoalsAgainst = 51, Points = 35 }
            };
            context.LeagueStandings.AddRange(leagueStandings);
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Seeding error: {ex.Message}");
            throw;
        }
    }
}
