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
        var gradski = new Stadium { Id = 4, Name = "Opus Arena", City = "Osijek", Capacity = 13_005, YearBuilt = 2023 };
        var lokomotiva = new Stadium { Id = 5, Name = "Stadion Kranjčevićeva", City = "Zagreb", Capacity = 8_850, YearBuilt = 1911 };
        var slaven = new Stadium { Id = 6, Name = "Gradski stadion Ivan Kušek Apaš", City = "Koprivnica", Capacity = 3_134, YearBuilt = 1912 };
        var vukovarSt = new Stadium { Id = 7, Name = "Stadion Cibalia", City = "Vinkovci", Capacity = 10_110, YearBuilt = 1982 };
        var varazdin = new Stadium { Id = 8, Name = "Stadion Varteks", City = "Varaždin", Capacity = 8_800, YearBuilt = 1978 };
        var istra = new Stadium { Id = 9, Name = "Aldo Drosina", City = "Pula", Capacity = 8_500, YearBuilt = 1948 };
        var goricaSt = new Stadium { Id = 10, Name = "Gradski stadion Velika Gorica", City = "Velika Gorica", Capacity = 8_000, YearBuilt = 2009 };
        Stadiums = new List<Stadium> { maksimir, poljud, rijekaSt, gradski, lokomotiva, slaven, vukovarSt, varazdin, istra, goricaSt };

        // --- KLUBOVI ---
        var dinamo = new Club { Id = 1, Name = "GNK Dinamo Zagreb", City = "Zagreb", FoundedYear = 1945, Budget = 45.5m, LeagueName = "HNL" };
        var hajduk = new Club { Id = 2, Name = "HNK Hajduk Split", City = "Split", FoundedYear = 1911, Budget = 28.3m, LeagueName = "HNL" };
        var rijeka = new Club { Id = 3, Name = "HNK Rijeka", City = "Rijeka", FoundedYear = 1946, Budget = 15.7m, LeagueName = "HNL" };
        var osijek = new Club { Id = 4, Name = "NK Osijek", City = "Osijek", FoundedYear = 1945, Budget = 12.5m, LeagueName = "HNL" };
        var lok = new Club { Id = 5, Name = "NK Lokomotiva Zagreb", City = "Zagreb", FoundedYear = 1914, Budget = 8.3m, LeagueName = "HNL" };
        var slaven_belupo = new Club { Id = 6, Name = "NK Slaven Belupo", City = "Koprivnica", FoundedYear = 1919, Budget = 5.2m, LeagueName = "HNL" };
        var vukovar = new Club { Id = 7, Name = "HNK Vukovar 1991", City = "Vinkovci", FoundedYear = 1991, Budget = 3.6m, LeagueName = "HNL" };
        var varazdin_club = new Club { Id = 8, Name = "NK Varaždin", City = "Varaždin", FoundedYear = 1990, Budget = 4.1m, LeagueName = "HNL" };
        var istra_1961 = new Club { Id = 9, Name = "NK Istra 1961", City = "Pula", FoundedYear = 1961, Budget = 6.2m, LeagueName = "HNL" };
        var gorica = new Club { Id = 10, Name = "HNK Gorica", City = "Velika Gorica", FoundedYear = 2009, Budget = 4.4m, LeagueName = "HNL" };

        dinamo.HomeStadium = maksimir;
        hajduk.HomeStadium = poljud;
        rijeka.HomeStadium = rijekaSt;
        osijek.HomeStadium = gradski;
        lok.HomeStadium = lokomotiva;
        slaven_belupo.HomeStadium = slaven;
        vukovar.HomeStadium = vukovarSt;
        varazdin_club.HomeStadium = varazdin;
        istra_1961.HomeStadium = istra;
        gorica.HomeStadium = goricaSt;
        
        Clubs = new List<Club> { dinamo, hajduk, rijeka, osijek, lok, slaven_belupo, vukovar, varazdin_club, istra_1961, gorica };

        // --- TRENERI (Past and Current) ---
        // Dinamo coaches
        var trDinamo = new Coach { Id = 1, FirstName = "Sergej", LastName = "Jakirović", Nationality = "Hrvatska", DateOfBirth = new DateTime(1978, 3, 15), ContractUntil = new DateTime(2026, 6, 30) };
        var trDinamoPrev1 = new Coach { Id = 11, FirstName = "Igor", LastName = "Jovičević", Nationality = "Hrvatska", DateOfBirth = new DateTime(1970, 5, 22), ContractUntil = new DateTime(2023, 12, 31) };
        var trDinamoPrev2 = new Coach { Id = 21, FirstName = "Damir", LastName = "Burić", Nationality = "Hrvatska", DateOfBirth = new DateTime(1962, 8, 12), ContractUntil = new DateTime(2022, 6, 30) };
        
        var trHajduk = new Coach { Id = 2, FirstName = "Gennaro", LastName = "Gattuso", Nationality = "Italija", DateOfBirth = new DateTime(1978, 1, 9), ContractUntil = new DateTime(2026, 6, 30) };
        var trRijeka = new Coach { Id = 3, FirstName = "Željko", LastName = "Sopić", Nationality = "Hrvatska", DateOfBirth = new DateTime(1972, 7, 20), ContractUntil = new DateTime(2025, 12, 31) };
        var trOsijek = new Coach { Id = 4, FirstName = "Andrej", LastName = "Kramarić", Nationality = "Hrvatska", DateOfBirth = new DateTime(1977, 11, 10), ContractUntil = new DateTime(2025, 6, 30) };
        var trLok = new Coach { Id = 5, FirstName = "Mario", LastName = "Cvitković", Nationality = "Hrvatska", DateOfBirth = new DateTime(1975, 2, 28), ContractUntil = new DateTime(2025, 12, 31) };
        var trSlaven = new Coach { Id = 6, FirstName = "Petar", LastName = "Krpan", Nationality = "Hrvatska", DateOfBirth = new DateTime(1968, 6, 3), ContractUntil = new DateTime(2025, 6, 30) };
        var trVukovar = new Coach { Id = 7, FirstName = "Silvijo", LastName = "Čabraja", Nationality = "Hrvatska", DateOfBirth = new DateTime(1970, 8, 11), ContractUntil = new DateTime(2026, 6, 30) };
        var trVarazdin = new Coach { Id = 8, FirstName = "Nenad", LastName = "Bjelica", Nationality = "Hrvatska", DateOfBirth = new DateTime(1973, 4, 19), ContractUntil = new DateTime(2026, 6, 30) };
        var trIstra = new Coach { Id = 9, FirstName = "Safet", LastName = "Hadžić", Nationality = "Bosna i Hercegovina", DateOfBirth = new DateTime(1971, 12, 5), ContractUntil = new DateTime(2025, 6, 30) };
        var trGorica = new Coach { Id = 10, FirstName = "Mario", LastName = "Carević", Nationality = "Hrvatska", DateOfBirth = new DateTime(1975, 11, 12), ContractUntil = new DateTime(2026, 6, 30) };
        
        dinamo.Coach = trDinamo; hajduk.Coach = trHajduk; rijeka.Coach = trRijeka; osijek.Coach = trOsijek;
        lok.Coach = trLok; slaven_belupo.Coach = trSlaven; vukovar.Coach = trVukovar;
        varazdin_club.Coach = trVarazdin; istra_1961.Coach = trIstra; gorica.Coach = trGorica;
        
        Coaches = new List<Coach> { trDinamo, trDinamoPrev1, trDinamoPrev2, trHajduk, trRijeka, trOsijek, trLok, trSlaven, trVukovar, trVarazdin, trIstra, trGorica };

        // --- IGRAČI ---
        // Dinamo players
        var livakovic = new Player { Id = 1, FirstName = "Dominik", LastName = "Livaković", DateOfBirth = new DateTime(1995, 1, 9), Nationality = "Hrvatska", Position = PlayerPosition.Goalkeeper, JerseyNumber = 1, MarketValue = 20.0m, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = dinamo };
        var sutalo = new Player { Id = 2, FirstName = "Josip", LastName = "Šutalo", DateOfBirth = new DateTime(2000, 2, 28), Nationality = "Hrvatska", Position = PlayerPosition.Defender, JerseyNumber = 5, MarketValue = 18.0m, ContractUntil = new DateTime(2028, 6, 30), IsInjured = false, Club = dinamo };
        var ivanusec = new Player { Id = 3, FirstName = "Luka", LastName = "Ivanušec", DateOfBirth = new DateTime(1998, 11, 26), Nationality = "Hrvatska", Position = PlayerPosition.Midfielder, JerseyNumber = 10, MarketValue = 12.0m, ContractUntil = new DateTime(2026, 6, 30), IsInjured = false, Club = dinamo };
        var petkovic = new Player { Id = 4, FirstName = "Bruno", LastName = "Petković", DateOfBirth = new DateTime(1994, 9, 16), Nationality = "Hrvatska", Position = PlayerPosition.Forward, JerseyNumber = 9, MarketValue = 10.0m, ContractUntil = new DateTime(2026, 6, 30), IsInjured = true, Club = dinamo };
        var moro = new Player { Id = 41, FirstName = "Mislav", LastName = "Oršić", DateOfBirth = new DateTime(1992, 4, 24), Nationality = "Hrvatska", Position = PlayerPosition.Midfielder, JerseyNumber = 11, MarketValue = 15.0m, ContractUntil = new DateTime(2027, 12, 31), IsInjured = false, Club = dinamo };
        var bradaric = new Player { Id = 42, FirstName = "Borna", LastName = "Barišić", DateOfBirth = new DateTime(1996, 11, 10), Nationality = "Hrvatska", Position = PlayerPosition.Defender, JerseyNumber = 3, MarketValue = 16.0m, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = dinamo };
        
        // Hajduk players
        var posavec = new Player { Id = 5, FirstName = "Lovre", LastName = "Posavec", DateOfBirth = new DateTime(1994, 4, 4), Nationality = "Hrvatska", Position = PlayerPosition.Goalkeeper, JerseyNumber = 1, MarketValue = 3.5m, ContractUntil = new DateTime(2026, 6, 30), IsInjured = false, Club = hajduk };
        var kalik = new Player { Id = 6, FirstName = "Stipe", LastName = "Kalik", DateOfBirth = new DateTime(1997, 5, 12), Nationality = "Hrvatska", Position = PlayerPosition.Defender, JerseyNumber = 4, MarketValue = 2.0m, ContractUntil = new DateTime(2025, 12, 31), IsInjured = false, Club = hajduk };
        var durdov = new Player { Id = 7, FirstName = "Ivan", LastName = "Đurđev", DateOfBirth = new DateTime(2003, 8, 15), Nationality = "Hrvatska", Position = PlayerPosition.Forward, JerseyNumber = 11, MarketValue = 8.0m, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = hajduk };
        var mlakar = new Player { Id = 43, FirstName = "Haris", LastName = "Haračić", DateOfBirth = new DateTime(2000, 3, 17), Nationality = "Bosna i Hercegovina", Position = PlayerPosition.Forward, JerseyNumber = 9, MarketValue = 9.5m, ContractUntil = new DateTime(2026, 12, 31), IsInjured = false, Club = hajduk };
        
        // Rijeka players
        var nevistic = new Player { Id = 8, FirstName = "Rauno", LastName = "Nevistić", DateOfBirth = new DateTime(1997, 3, 23), Nationality = "Hrvatska", Position = PlayerPosition.Goalkeeper, JerseyNumber = 1, MarketValue = 1.5m, ContractUntil = new DateTime(2026, 6, 30), IsInjured = false, Club = rijeka };
        var tomecak = new Player { Id = 9, FirstName = "Nino", LastName = "Tomečak", DateOfBirth = new DateTime(1999, 6, 30), Nationality = "Hrvatska", Position = PlayerPosition.Midfielder, JerseyNumber = 8, MarketValue = 2.5m, ContractUntil = new DateTime(2026, 12, 31), IsInjured = false, Club = rijeka };
        var muric = new Player { Id = 10, FirstName = "Stjepan", LastName = "Murić", DateOfBirth = new DateTime(2001, 11, 5), Nationality = "Hrvatska", Position = PlayerPosition.Forward, JerseyNumber = 19, MarketValue = 1.8m, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = rijeka };
        var cvetkovic = new Player { Id = 44, FirstName = "Robert", LastName = "Muniž", DateOfBirth = new DateTime(1998, 8, 7), Nationality = "Hrvatska", Position = PlayerPosition.Defender, JerseyNumber = 2, MarketValue = 3.2m, ContractUntil = new DateTime(2025, 12, 31), IsInjured = false, Club = rijeka };
        
        // Osijek players
        var gelezniak = new Player { Id = 12, FirstName = "Andrej", LastName = "Kramarić", DateOfBirth = new DateTime(1991, 6, 19), Nationality = "Hrvatska", Position = PlayerPosition.Forward, JerseyNumber = 9, MarketValue = 7.5m, ContractUntil = new DateTime(2026, 6, 30), IsInjured = false, Club = osijek };
        
        // Lokomotiva players
        var stojanovic = new Player { Id = 13, FirstName = "Savo", LastName = "Milošević", DateOfBirth = new DateTime(2000, 5, 12), Nationality = "Srbija", Position = PlayerPosition.Midfielder, JerseyNumber = 7, MarketValue = 4.2m, ContractUntil = new DateTime(2026, 12, 31), IsInjured = false, Club = lok };
        
        // Slaven players
        var soric = new Player { Id = 14, FirstName = "Marijo", LastName = "Ćuže", DateOfBirth = new DateTime(1999, 4, 22), Nationality = "Hrvatska", Position = PlayerPosition.Forward, JerseyNumber = 10, MarketValue = 3.1m, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = slaven_belupo };
        
        // Vukovar players
        var vukovarPlayer = new Player { Id = 15, FirstName = "Dario", LastName = "Mandić", DateOfBirth = new DateTime(2000, 9, 11), Nationality = "Hrvatska", Position = PlayerPosition.Midfielder, JerseyNumber = 8, MarketValue = 2.3m, ContractUntil = new DateTime(2026, 6, 30), IsInjured = false, Club = vukovar };
        
        // Varaždin players
        var svidercic = new Player { Id = 16, FirstName = "Loren", LastName = "Lovreković", DateOfBirth = new DateTime(2000, 11, 3), Nationality = "Hrvatska", Position = PlayerPosition.Forward, JerseyNumber = 11, MarketValue = 2.8m, ContractUntil = new DateTime(2026, 12, 31), IsInjured = false, Club = varazdin_club };
        
        // Istra players
        var mikulic = new Player { Id = 17, FirstName = "Damir", LastName = "Peretin", DateOfBirth = new DateTime(2000, 2, 14), Nationality = "Hrvatska", Position = PlayerPosition.Defender, JerseyNumber = 4, MarketValue = 3.5m, ContractUntil = new DateTime(2025, 12, 31), IsInjured = false, Club = istra_1961 };
        
        // Gorica players
        var goricaPlayer = new Player { Id = 18, FirstName = "Ante", LastName = "Čuić", DateOfBirth = new DateTime(2001, 7, 8), Nationality = "Hrvatska", Position = PlayerPosition.Forward, JerseyNumber = 9, MarketValue = 1.5m, ContractUntil = new DateTime(2027, 6, 30), IsInjured = false, Club = gorica };

        dinamo.Players.AddRange(new[] { livakovic, sutalo, ivanusec, petkovic, moro, bradaric });
        hajduk.Players.AddRange(new[] { posavec, kalik, durdov, mlakar });
        rijeka.Players.AddRange(new[] { nevistic, tomecak, muric, cvetkovic });
        osijek.Players.Add(gelezniak);
        lok.Players.Add(stojanovic);
        slaven_belupo.Players.Add(soric);
        vukovar.Players.Add(vukovarPlayer);
        varazdin_club.Players.Add(svidercic);
        istra_1961.Players.Add(mikulic);
        gorica.Players.Add(goricaPlayer);
        
        Players = new List<Player> { livakovic, sutalo, ivanusec, petkovic, moro, bradaric, posavec, kalik, durdov, mlakar, nevistic, tomecak, muric, cvetkovic, gelezniak, stojanovic, vukovarPlayer, svidercic, mikulic, goricaPlayer };

        // --- UTAKMICE (HNL 2025/26: actual season clubs) ---
        var roundRobinTeams = new List<Club> { dinamo, hajduk, rijeka, osijek, lok, slaven_belupo, vukovar, varazdin_club, istra_1961, gorica };
        var rotation = roundRobinTeams.Skip(1).ToList();
        var baseRounds = new List<List<(Club A, Club B)>>();

        for (int round = 0; round < 9; round++)
        {
            var left = new List<Club> { roundRobinTeams[0] };
            left.AddRange(rotation.Take(4));
            var right = rotation.Skip(4).Reverse().ToList();

            var pairs = new List<(Club A, Club B)>();
            for (int i = 0; i < 5; i++)
            {
                pairs.Add((left[i], right[i]));
            }

            baseRounds.Add(pairs);

            var last = rotation[^1];
            rotation.RemoveAt(rotation.Count - 1);
            rotation.Insert(0, last);
        }

        var schedule = new List<List<(Club Home, Club Away)>>();
        for (int cycle = 0; cycle < 2; cycle++)
        {
            for (int i = 0; i < baseRounds.Count; i++)
            {
                var roundMatches = new List<(Club Home, Club Away)>();
                foreach (var pair in baseRounds[i])
                {
                    var flip = (i + cycle) % 2 == 0;
                    roundMatches.Add(flip ? (pair.A, pair.B) : (pair.B, pair.A));
                }

                schedule.Add(roundMatches);
            }
        }

        var firstHalf = schedule.ToList();
        foreach (var round in firstHalf)
        {
            schedule.Add(round.Select(m => (m.Away, m.Home)).ToList());
        }

        var seasonStart = new DateTime(2025, 8, 1, 21, 0, 0);
        var generatedMatches = new List<Match>();
        int matchId = 1;

        for (int round = 0; round < schedule.Count; round++)
        {
            for (int slot = 0; slot < schedule[round].Count; slot++)
            {
                var fixture = schedule[round][slot];
                var kickOff = seasonStart.AddDays(round * 7).AddHours(slot * 2);
                var isFinished = round < 30;
                var homeGoals = isFinished ? (round + fixture.Home.Id + fixture.Away.Id + slot) % 4 : 0;
                var awayGoals = isFinished ? (round * 2 + fixture.Away.Id + slot) % 3 : 0;

                var match = new Match
                {
                    Id = matchId++,
                    Date = kickOff,
                    HomeClub = fixture.Home,
                    AwayClub = fixture.Away,
                    HomeScore = homeGoals,
                    AwayScore = awayGoals,
                    Stadium = fixture.Home.HomeStadium,
                    Status = isFinished ? MatchStatus.Finished : MatchStatus.Scheduled,
                    Attendance = isFinished ? 6000 + ((fixture.Home.Id * fixture.Away.Id * (round + 1)) % 22000) : 0,
                    Referee = isFinished ? $"HNL Referee {(round + slot) % 12 + 1}" : "TBD",
                    Round = $"Kolo {round + 1}"
                };

                fixture.Home.HomeMatches.Add(match);
                fixture.Away.AwayMatches.Add(match);
                generatedMatches.Add(match);
            }
        }

        Matches = generatedMatches;

        // --- STATISTIKE (Dinamo fokus) ---
        var dinamoFinished = Matches
            .Where(m => m.Status == MatchStatus.Finished && (m.HomeClub.Id == dinamo.Id || m.AwayClub.Id == dinamo.Id))
            .OrderBy(m => m.Date)
            .Take(3)
            .ToList();

        var stats = new List<PlayerStat>();
        int statId = 1;

        foreach (var match in dinamoFinished)
        {
            var ivanusecStat = new PlayerStat
            {
                Id = statId++,
                Player = ivanusec,
                Match = match,
                Goals = 1,
                Assists = 1,
                MinutesPlayed = 90,
                YellowCards = 0,
                RedCard = false,
                Rating = 8.1
            };

            var petkovicStat = new PlayerStat
            {
                Id = statId++,
                Player = petkovic,
                Match = match,
                Goals = 1,
                Assists = 0,
                MinutesPlayed = 84,
                YellowCards = 1,
                RedCard = false,
                Rating = 7.5
            };

            var moroStat = new PlayerStat
            {
                Id = statId++,
                Player = moro,
                Match = match,
                Goals = 0,
                Assists = 1,
                MinutesPlayed = 90,
                YellowCards = 0,
                RedCard = false,
                Rating = 7.8
            };

            match.PlayerStats.AddRange(new[] { ivanusecStat, petkovicStat, moroStat });
            ivanusec.Stats.Add(ivanusecStat);
            petkovic.Stats.Add(petkovicStat);
            moro.Stats.Add(moroStat);
            stats.AddRange(new[] { ivanusecStat, petkovicStat, moroStat });
        }

        var livakovicStat = new PlayerStat
        {
            Id = statId++,
            Player = livakovic,
            Match = dinamoFinished.First(),
            Goals = 0,
            Assists = 0,
            MinutesPlayed = 90,
            YellowCards = 0,
            RedCard = false,
            Rating = 7.2
        };
        dinamoFinished.First().PlayerStats.Add(livakovicStat);
        livakovic.Stats.Add(livakovicStat);
        stats.Add(livakovicStat);

        PlayerStats = stats;

        // --- TRANSFERI ---
        var t1 = new Transfer { Id = 1, Player = livakovic, FromClub = dinamo, ToClub = dinamo, TransferDate = new DateTime(2022, 7, 1), Fee = 0m };
        var t2 = new Transfer { Id = 2, Player = petkovic, FromClub = hajduk, ToClub = dinamo, TransferDate = new DateTime(2023, 1, 31), Fee = 4.5m };
        livakovic.Transfers.Add(t1); 
        petkovic.Transfers.Add(t2);
        
        Transfers = new List<Transfer> { t1, t2 };
    }
}