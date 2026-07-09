using FootballClub.Models;
using FootballClub.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Data;

public static class DataSeeder
{
    public static void SeedDatabase(ApplicationDbContext context)
    {
        // If players exist we assume database already seeded; checking Players is safer
        // than checking Stadiums because partial seeding could leave stadiums present
        // while other tables remain empty. This full seed only ever runs once per database.
        if (!context.Players.Any())
        {
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

        // Runs every time (independent of the one-shot seed above) so an already-seeded
        // database still picks up fresh training data as "today" moves forward.
        SeedJulyTrainingSessions(context);
    }

    // The original seed's TrainingSessions were all dated May 2026; once "today" passes
    // that, Training/Schedules pages look stale (no upcoming sessions, most players with
    // no weekly obligations). This adds a July 2026 batch with broad squad coverage,
    // idempotent so it only inserts once even though it runs on every startup.
    private static void SeedJulyTrainingSessions(ApplicationDbContext context)
    {
        var club = context.Clubs.FirstOrDefault(c => c.Name.Contains("Dinamo"));
        if (club == null) return;

        if (context.TrainingSessions.Any(ts => ts.ClubId == club.Id && ts.StartTime.Year == 2026 && ts.StartTime.Month == 7))
        {
            return;
        }

        var squad = context.Players.Where(p => p.ClubId == club.Id && !p.IsDeleted).OrderBy(p => p.JerseyNumber).ToList();
        if (squad.Count == 0) return;

        var staff = context.StaffMembers.Where(s => s.ClubId == club.Id && !s.IsDeleted).ToList();
        var headCoach = staff.FirstOrDefault(s => s.Role == "Head Coach") ?? staff.FirstOrDefault();
        var assistantCoach = staff.FirstOrDefault(s => s.Role == "Assistant Coach") ?? headCoach;
        var fitnessCoach = staff.FirstOrDefault(s => s.Role == "Fitness Coach") ?? headCoach;
        var physio = staff.FirstOrDefault(s => s.Role == "Physio") ?? headCoach;

        var goalkeepers = squad.Where(p => p.Position == PlayerPosition.Goalkeeper).ToList();
        var defenders = squad.Where(p => p.Position == PlayerPosition.Defender).ToList();
        var midfielders = squad.Where(p => p.Position == PlayerPosition.Midfielder).ToList();
        var forwards = squad.Where(p => p.Position == PlayerPosition.Forward).ToList();

        // TrainingSession.Participants maps through Player.TrainingSessionId — a single FK on
        // Player, not a real many-to-many join table — so a given player can only ever be the
        // "current" participant of ONE session. Split each position group into two disjoint
        // halves so every player ends up with exactly one real July session (no player is
        // silently reassigned/overwritten by a later session claiming the same person).
        var defendersA = defenders.Take((defenders.Count + 1) / 2).ToList();
        var defendersB = defenders.Skip(defendersA.Count).ToList();
        var midfieldersA = midfielders.Take((midfielders.Count + 1) / 2).ToList();
        var midfieldersB = midfielders.Skip(midfieldersA.Count).ToList();
        var forwardsA = forwards.Take((forwards.Count + 1) / 2).ToList();
        var forwardsB = forwards.Skip(forwardsA.Count).ToList();

        var sessions = new List<TrainingSession>
        {
            // Club-wide calendar entries: no fixed roster, so they don't compete with the
            // unit-specific sessions below for any player's single TrainingSessionId slot.
            new()
            {
                Club = club, Title = "Pre-Season Fitness Testing", FocusArea = "Aerobic conditioning",
                StartTime = new DateTime(2026, 7, 1, 9, 0, 0), EndTime = new DateTime(2026, 7, 1, 10, 30, 0),
                Location = "Maksimir", Intensity = TrainingIntensity.High, LeadStaff = fitnessCoach,
                Participants = new List<Player>(), Notes = "Full-squad fitness testing to open the month."
            },
            new()
            {
                Club = club, Title = "Recovery Session", FocusArea = "Active recovery",
                StartTime = new DateTime(2026, 7, 4, 9, 0, 0), EndTime = new DateTime(2026, 7, 4, 10, 0, 0),
                Location = "Klinika Dinamo", Intensity = TrainingIntensity.Recovery, LeadStaff = physio,
                Participants = new List<Player>(), Notes = "Pool session and stretching after the fitness block."
            },
            new()
            {
                Club = club, Title = "Set-Piece Rehearsal", FocusArea = "Corners and free kicks",
                StartTime = new DateTime(2026, 7, 6, 16, 0, 0), EndTime = new DateTime(2026, 7, 6, 17, 30, 0),
                Location = "Maksimir", Intensity = TrainingIntensity.Moderate, LeadStaff = assistantCoach,
                Participants = new List<Player>(), Notes = "Dead-ball routines for both boxes."
            },
            new()
            {
                Club = club, Title = "Tactical Shape - Matchday Prep", FocusArea = "Positional play",
                StartTime = new DateTime(2026, 7, 8, 10, 0, 0), EndTime = new DateTime(2026, 7, 8, 12, 0, 0),
                Location = "Maksimir", Intensity = TrainingIntensity.High, LeadStaff = headCoach,
                Participants = new List<Player>(), Notes = "Full-squad walkthrough of the matchday XI shape."
            },
            new()
            {
                Club = club, Title = "Media & Sponsor Day", FocusArea = "Media training",
                StartTime = new DateTime(2026, 7, 14, 11, 0, 0), EndTime = new DateTime(2026, 7, 14, 13, 0, 0),
                Location = "Maksimir Media Centre", Intensity = TrainingIntensity.Light, LeadStaff = headCoach,
                Participants = new List<Player>(), Notes = "Season-opener press day."
            },
            new()
            {
                Club = club, Title = "Recovery & Regeneration", FocusArea = "Active recovery",
                StartTime = new DateTime(2026, 7, 18, 9, 0, 0), EndTime = new DateTime(2026, 7, 18, 10, 0, 0),
                Location = "Klinika Dinamo", Intensity = TrainingIntensity.Recovery, LeadStaff = physio,
                Participants = new List<Player>(), Notes = "Ice baths, massage rotation and light mobility."
            },
            new()
            {
                Club = club, Title = "Matchday Simulation", FocusArea = "11v11 friendly",
                StartTime = new DateTime(2026, 7, 21, 17, 0, 0), EndTime = new DateTime(2026, 7, 21, 18, 45, 0),
                Location = "Maksimir", Intensity = TrainingIntensity.High, LeadStaff = headCoach,
                Participants = new List<Player>(), Notes = "Full 90-minute intra-squad friendly."
            },
            new()
            {
                Club = club, Title = "Final Prep - League Opener", FocusArea = "Tactical walkthrough",
                StartTime = new DateTime(2026, 7, 29, 10, 0, 0), EndTime = new DateTime(2026, 7, 29, 11, 30, 0),
                Location = "Maksimir", Intensity = TrainingIntensity.High, LeadStaff = headCoach,
                Participants = new List<Player>(), Notes = "Final training session ahead of the league opener."
            },

            // Unit-specific sessions: every player appears in exactly one of these.
            new()
            {
                Club = club, Title = "Goalkeeper Specific Training", FocusArea = "Shot-stopping and distribution",
                StartTime = new DateTime(2026, 7, 9, 9, 0, 0), EndTime = new DateTime(2026, 7, 9, 10, 0, 0),
                Location = "Maksimir", Intensity = TrainingIntensity.Moderate, LeadStaff = assistantCoach,
                Participants = goalkeepers, Notes = "One-on-one goalkeeping unit work."
            },
            new()
            {
                Club = club, Title = "Defensive Shape Work", FocusArea = "Back-line organisation",
                StartTime = new DateTime(2026, 7, 2, 10, 0, 0), EndTime = new DateTime(2026, 7, 2, 11, 30, 0),
                Location = "Maksimir", Intensity = TrainingIntensity.Moderate, LeadStaff = assistantCoach,
                Participants = defendersA, Notes = "Offside trap and pressing triggers."
            },
            new()
            {
                Club = club, Title = "Defensive Transition Work", FocusArea = "Counter-press triggers",
                StartTime = new DateTime(2026, 7, 23, 10, 0, 0), EndTime = new DateTime(2026, 7, 23, 11, 30, 0),
                Location = "Maksimir", Intensity = TrainingIntensity.Moderate, LeadStaff = assistantCoach,
                Participants = defendersB, Notes = "Reaction to turnovers in midfield."
            },
            new()
            {
                Club = club, Title = "Midfield Possession Play", FocusArea = "Ball retention under pressure",
                StartTime = new DateTime(2026, 7, 11, 10, 0, 0), EndTime = new DateTime(2026, 7, 11, 11, 0, 0),
                Location = "Maksimir", Intensity = TrainingIntensity.High, LeadStaff = headCoach,
                Participants = midfieldersA, Notes = "Rondo and possession games."
            },
            new()
            {
                Club = club, Title = "Speed & Agility", FocusArea = "Sprint mechanics",
                StartTime = new DateTime(2026, 7, 16, 9, 0, 0), EndTime = new DateTime(2026, 7, 16, 10, 0, 0),
                Location = "Maksimir", Intensity = TrainingIntensity.High, LeadStaff = fitnessCoach,
                Participants = midfieldersB, Notes = "Sprint testing and agility ladder work."
            },
            new()
            {
                Club = club, Title = "Attacking Combination Play", FocusArea = "Final third movement",
                StartTime = new DateTime(2026, 7, 3, 10, 0, 0), EndTime = new DateTime(2026, 7, 3, 11, 30, 0),
                Location = "Maksimir", Intensity = TrainingIntensity.Moderate, LeadStaff = headCoach,
                Participants = forwardsA, Notes = "Combination play and finishing drills."
            },
            new()
            {
                Club = club, Title = "Technical Finishing Clinic", FocusArea = "Shooting technique",
                StartTime = new DateTime(2026, 7, 25, 10, 0, 0), EndTime = new DateTime(2026, 7, 25, 11, 0, 0),
                Location = "Maksimir", Intensity = TrainingIntensity.Moderate, LeadStaff = headCoach,
                Participants = forwardsB, Notes = "One-touch finishing and volley work."
            },
        };

        context.TrainingSessions.AddRange(sessions);
        context.SaveChanges();
    }

    public static async Task SeedIdentityDataAsync(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager, CancellationToken cancellationToken = default)
    {
        foreach (var roleName in new[] { Role.Admin.ToString(), Role.User.ToString() })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            }
        }

        if (await userManager.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var adminUser = new AppUser
        {
            UserName = "admin",
            Email = "admin@footballclub.local",
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var regularUser = new AppUser
        {
            UserName = "user",
            Email = "user@footballclub.local",
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var adminCreate = await userManager.CreateAsync(adminUser, "Admin123!");
        if (adminCreate.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, Role.Admin.ToString());
        }

        var userCreate = await userManager.CreateAsync(regularUser, "User123!");
        if (userCreate.Succeeded)
        {
            await userManager.AddToRoleAsync(regularUser, Role.User.ToString());
        }
    }
}
