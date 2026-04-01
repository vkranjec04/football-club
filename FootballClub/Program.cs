using FootballClub.Models;
using FootballClub.Models.Enums;

// ============================================================
//  STADIONI
// ============================================================
var stadionMaximir = new Stadium
{
    Id = 1,
    Name = "Stadion Maksimir",
    City = "Zagreb",
    Capacity = 35_123,
    YearBuilt = 1912
};
var stadionPolyud = new Stadium
{
    Id = 2,
    Name = "Stadion Poljud",
    City = "Split",
    Capacity = 34_198,
    YearBuilt = 1979
};
var stadionRijeka = new Stadium
{
    Id = 3,
    Name = "HNK Rijeka stadion",
    City = "Rijeka",
    Capacity = 8_279,
    YearBuilt = 1946
};

// ============================================================
//  KLUBOVI
// ============================================================
var dinamo = new Club
{
    Id = 1,
    Name = "GNK Dinamo Zagreb",
    City = "Zagreb",
    FoundedYear = 1945,
    Budget = 45.5m,
    LeagueName = "Supersport HNL",
    HomeStadium = stadionMaximir
};

var hajduk = new Club
{
    Id = 2,
    Name = "HNK Hajduk Split",
    City = "Split",
    FoundedYear = 1911,
    Budget = 28.3m,
    LeagueName = "Supersport HNL",
    HomeStadium = stadionPolyud
};

var rijeka = new Club
{
    Id = 3,
    Name = "HNK Rijeka",
    City = "Rijeka",
    FoundedYear = 1946,
    Budget = 15.7m,
    LeagueName = "Supersport HNL",
    HomeStadium = stadionRijeka
};

// ============================================================
//  TRENERI
// ============================================================
var trenerDinamo = new Coach
{
    Id = 1,
    FirstName = "Sergej",
    LastName = "Jakirović",
    Nationality = "Hrvatska",
    DateOfBirth = new DateTime(1978, 3, 15),
    ContractUntil = new DateTime(2026, 6, 30),
    Club = dinamo
};
dinamo.Coach = trenerDinamo;

var trenerHajduk = new Coach
{
    Id = 2,
    FirstName = "Gennaro",
    LastName = "Gattuso",
    Nationality = "Italija",
    DateOfBirth = new DateTime(1978, 1, 9),
    ContractUntil = new DateTime(2026, 6, 30),
    Club = hajduk
};
hajduk.Coach = trenerHajduk;

var trenerRijeka = new Coach
{
    Id = 3,
    FirstName = "Željko",
    LastName = "Sopić",
    Nationality = "Hrvatska",
    DateOfBirth = new DateTime(1972, 7, 20),
    ContractUntil = new DateTime(2025, 12, 31),
    Club = rijeka
};
rijeka.Coach = trenerRijeka;

// ============================================================
//  IGRAČI - DINAMO
// ============================================================
var livakovic = new Player
{
    Id = 1,
    FirstName = "Dominik",
    LastName = "Livaković",
    DateOfBirth = new DateTime(1995, 1, 9),
    Nationality = "Hrvatska",
    Position = PlayerPosition.Goalkeeper,
    JerseyNumber = 1,
    MarketValue = 20.0m,
    ContractUntil = new DateTime(2027, 6, 30),
    IsInjured = false,
    Club = dinamo
};

var sutalo = new Player
{
    Id = 2,
    FirstName = "Josip",
    LastName = "Šutalo",
    DateOfBirth = new DateTime(2000, 2, 28),
    Nationality = "Hrvatska",
    Position = PlayerPosition.Defender,
    JerseyNumber = 5,
    MarketValue = 18.0m,
    ContractUntil = new DateTime(2028, 6, 30),
    IsInjured = false,
    Club = dinamo
};

var ivanusec = new Player
{
    Id = 3,
    FirstName = "Luka",
    LastName = "Ivanušec",
    DateOfBirth = new DateTime(1998, 11, 26),
    Nationality = "Hrvatska",
    Position = PlayerPosition.Midfielder,
    JerseyNumber = 10,
    MarketValue = 12.0m,
    ContractUntil = new DateTime(2026, 6, 30),
    IsInjured = false,
    Club = dinamo
};

var petković = new Player
{
    Id = 4,
    FirstName = "Bruno",
    LastName = "Petković",
    DateOfBirth = new DateTime(1994, 9, 16),
    Nationality = "Hrvatska",
    Position = PlayerPosition.Forward,
    JerseyNumber = 9,
    MarketValue = 10.0m,
    ContractUntil = new DateTime(2026, 6, 30),
    IsInjured = true,
    Club = dinamo
};

dinamo.Players.AddRange(new[] { livakovic, sutalo, ivanusec, petković });

// ============================================================
//  IGRAČI - HAJDUK
// ============================================================
var posavec = new Player
{
    Id = 5,
    FirstName = "Lovre",
    LastName = "Posavec",
    DateOfBirth = new DateTime(1994, 4, 4),
    Nationality = "Hrvatska",
    Position = PlayerPosition.Goalkeeper,
    JerseyNumber = 1,
    MarketValue = 3.5m,
    ContractUntil = new DateTime(2026, 6, 30),
    IsInjured = false,
    Club = hajduk
};

var kalik = new Player
{
    Id = 6,
    FirstName = "Stipe",
    LastName = "Kalik",
    DateOfBirth = new DateTime(1997, 5, 12),
    Nationality = "Hrvatska",
    Position = PlayerPosition.Defender,
    JerseyNumber = 4,
    MarketValue = 2.0m,
    ContractUntil = new DateTime(2025, 12, 31),
    IsInjured = false,
    Club = hajduk
};

var durdov = new Player
{
    Id = 7,
    FirstName = "Ivan",
    LastName = "Đurđev",
    DateOfBirth = new DateTime(2003, 8, 15),
    Nationality = "Hrvatska",
    Position = PlayerPosition.Forward,
    JerseyNumber = 11,
    MarketValue = 8.0m,
    ContractUntil = new DateTime(2027, 6, 30),
    IsInjured = false,
    Club = hajduk
};

hajduk.Players.AddRange(new[] { posavec, kalik, durdov });

// ============================================================
//  IGRAČI - RIJEKA
// ============================================================
var nevistic = new Player
{
    Id = 8,
    FirstName = "Rauno",
    LastName = "Nevistić",
    DateOfBirth = new DateTime(1997, 3, 23),
    Nationality = "Hrvatska",
    Position = PlayerPosition.Goalkeeper,
    JerseyNumber = 1,
    MarketValue = 1.5m,
    ContractUntil = new DateTime(2026, 6, 30),
    IsInjured = false,
    Club = rijeka
};

var tomecak = new Player
{
    Id = 9,
    FirstName = "Nino",
    LastName = "Tomečak",
    DateOfBirth = new DateTime(1999, 6, 30),
    Nationality = "Hrvatska",
    Position = PlayerPosition.Midfielder,
    JerseyNumber = 8,
    MarketValue = 2.5m,
    ContractUntil = new DateTime(2026, 12, 31),
    IsInjured = false,
    Club = rijeka
};

var murić = new Player
{
    Id = 10,
    FirstName = "Stjepan",
    LastName = "Murić",
    DateOfBirth = new DateTime(2001, 11, 5),
    Nationality = "Hrvatska",
    Position = PlayerPosition.Forward,
    JerseyNumber = 19,
    MarketValue = 1.8m,
    ContractUntil = new DateTime(2027, 6, 30),
    IsInjured = false,
    Club = rijeka
};

rijeka.Players.AddRange(new[] { nevistic, tomecak, murić });

// ============================================================
//  UTAKMICE
// ============================================================
var utakmica1 = new Match
{
    Id = 1,
    Date = new DateTime(2025, 9, 14, 20, 0, 0),
    HomeClub = dinamo,
    AwayClub = hajduk,
    HomeScore = 2,
    AwayScore = 1,
    Stadium = stadionMaximir,
    Status = MatchStatus.Finished,
    Attendance = 30_000,
    Referee = "Fran Jović",
    Round = "Kolo 5"
};

var utakmica2 = new Match
{
    Id = 2,
    Date = new DateTime(2025, 10, 5, 18, 0, 0),
    HomeClub = hajduk,
    AwayClub = rijeka,
    HomeScore = 3,
    AwayScore = 0,
    Stadium = stadionPolyud,
    Status = MatchStatus.Finished,
    Attendance = 25_000,
    Referee = "Mario Zebić",
    Round = "Kolo 8"
};

var utakmica3 = new Match
{
    Id = 3,
    Date = new DateTime(2025, 11, 22, 17, 0, 0),
    HomeClub = rijeka,
    AwayClub = dinamo,
    HomeScore = 1,
    AwayScore = 1,
    Stadium = stadionRijeka,
    Status = MatchStatus.Finished,
    Attendance = 7_500,
    Referee = "Tomislav Šuperina",
    Round = "Kolo 12"
};

var utakmica4 = new Match
{
    Id = 4,
    Date = new DateTime(2026, 4, 20, 20, 0, 0),
    HomeClub = dinamo,
    AwayClub = rijeka,
    HomeScore = 0,
    AwayScore = 0,
    Stadium = stadionMaximir,
    Status = MatchStatus.Scheduled,
    Attendance = 0,
    Referee = "TBD",
    Round = "Kolo 25"
};

dinamo.HomeMatches.AddRange(new[] { utakmica1, utakmica4 });
dinamo.AwayMatches.Add(utakmica3);
hajduk.HomeMatches.Add(utakmica2);
hajduk.AwayMatches.Add(utakmica1);
rijeka.HomeMatches.Add(utakmica3);
rijeka.AwayMatches.AddRange(new[] { utakmica2, utakmica4 });

// ============================================================
//  STATISTIKE IGRAČA PO UTAKMICAMA  (N-N veza)
// ============================================================
var stat1 = new PlayerStat { Id = 1, Player = ivanusec, Match = utakmica1, Goals = 1, Assists = 1, MinutesPlayed = 90, YellowCards = 0, RedCard = false, Rating = 8.5 };
var stat2 = new PlayerStat { Id = 2, Player = petković, Match = utakmica1, Goals = 1, Assists = 0, MinutesPlayed = 80, YellowCards = 1, RedCard = false, Rating = 7.8 };
var stat3 = new PlayerStat { Id = 3, Player = durdov, Match = utakmica1, Goals = 1, Assists = 0, MinutesPlayed = 90, YellowCards = 0, RedCard = false, Rating = 7.2 };

var stat4 = new PlayerStat { Id = 4, Player = durdov, Match = utakmica2, Goals = 2, Assists = 1, MinutesPlayed = 90, YellowCards = 0, RedCard = false, Rating = 9.1 };
var stat5 = new PlayerStat { Id = 5, Player = kalik, Match = utakmica2, Goals = 1, Assists = 0, MinutesPlayed = 90, YellowCards = 1, RedCard = false, Rating = 7.5 };
var stat6 = new PlayerStat { Id = 6, Player = tomecak, Match = utakmica2, Goals = 0, Assists = 0, MinutesPlayed = 60, YellowCards = 2, RedCard = true, Rating = 4.5 };

var stat7 = new PlayerStat { Id = 7, Player = ivanusec, Match = utakmica3, Goals = 0, Assists = 1, MinutesPlayed = 90, YellowCards = 0, RedCard = false, Rating = 7.0 };
var stat8 = new PlayerStat { Id = 8, Player = murić, Match = utakmica3, Goals = 1, Assists = 0, MinutesPlayed = 85, YellowCards = 0, RedCard = false, Rating = 7.9 };

// Dodajemo statistike na utakmice i igrače
utakmica1.PlayerStats.AddRange(new[] { stat1, stat2, stat3 });
utakmica2.PlayerStats.AddRange(new[] { stat4, stat5, stat6 });
utakmica3.PlayerStats.AddRange(new[] { stat7, stat8 });

ivanusec.Stats.AddRange(new[] { stat1, stat7 });
petković.Stats.Add(stat2);
durdov.Stats.AddRange(new[] { stat3, stat4 });
kalik.Stats.Add(stat5);
tomecak.Stats.Add(stat6);
murić.Stats.Add(stat8);

// ============================================================
//  TRANSFERI
// ============================================================
var transfer1 = new Transfer
{
    Id = 1,
    Player = livakovic,
    FromClub = dinamo,
    ToClub = dinamo,   // primjer - ostao u klubu
    TransferDate = new DateTime(2022, 7, 1),
    Fee = 0m
};
var transfer2 = new Transfer
{
    Id = 2,
    Player = petković,
    FromClub = hajduk,
    ToClub = dinamo,
    TransferDate = new DateTime(2023, 1, 31),
    Fee = 4.5m
};

livakovic.Transfers.Add(transfer1);
petković.Transfers.Add(transfer2);

// ============================================================
//  Sve klube prikupimo u jednu listu za LINQ upite
// ============================================================
var sviKlubovi = new List<Club> { dinamo, hajduk, rijeka };
var sviIgraci = sviKlubovi.SelectMany(k => k.Players).ToList();
var sveUtakmice = new List<Match> { utakmica1, utakmica2, utakmica3, utakmica4 };

Console.WriteLine("========================================");
Console.WriteLine("        FOOTBALL CLUB - LINQ UPITI");
Console.WriteLine("========================================\n");

// ----------------------------------------------------------
// UPIT 1: Svi igrači na poziciji Napadač (Forward), sortirani po tržišnoj vrijednosti
// ----------------------------------------------------------
Console.WriteLine("1. NAPADAČI po tržišnoj vrijednosti (silazno):");
var napadaci = sviIgraci
    .Where(p => p.Position == PlayerPosition.Forward)
    .OrderByDescending(p => p.MarketValue)
    .ToList();

foreach (var p in napadaci)
    Console.WriteLine($"   {p.FullName} ({p.Club.Name}) - {p.MarketValue}M EUR");

// ----------------------------------------------------------
// UPIT 2: Broj igrača po klubu
// ----------------------------------------------------------
Console.WriteLine("\n2. BROJ IGRAČA PO KLUBU:");
var igraciBrojPoKlubu = sviKlubovi
    .Select(k => new { Klub = k.Name, BrojIgraca = k.Players.Count })
    .OrderByDescending(x => x.BrojIgraca)
    .ToList();

foreach (var x in igraciBrojPoKlubu)
    Console.WriteLine($"   {x.Klub}: {x.BrojIgraca} igrača");

// ----------------------------------------------------------
// UPIT 3: Završene utakmice (Status == Finished)
// ----------------------------------------------------------
Console.WriteLine("\n3. ZAVRŠENE UTAKMICE:");
var zavrseneUtakmice = sveUtakmice
    .Where(u => u.Status == MatchStatus.Finished)
    .OrderBy(u => u.Date)
    .ToList();

foreach (var u in zavrseneUtakmice)
    Console.WriteLine($"   {u.Date:dd.MM.yyyy} | {u.HomeClub.Name} {u.Result} {u.AwayClub.Name} [{u.Round}]");

// ----------------------------------------------------------
// UPIT 4: Top strijelci - igrači koji su dali barem 1 gol
// ----------------------------------------------------------
Console.WriteLine("\n4. TOP STRIJELCI (ukupno golova):");
var strijelci = sviIgraci
    .Where(p => p.Stats.Sum(s => s.Goals) > 0)
    .Select(p => new
    {
        Ime = p.FullName,
        Klub = p.Club.Name,
        Golovi = p.Stats.Sum(s => s.Goals),
        Asistencije = p.Stats.Sum(s => s.Assists)
    })
    .OrderByDescending(x => x.Golovi)
    .ToList();

foreach (var s in strijelci)
    Console.WriteLine($"   {s.Ime} ({s.Klub}) - {s.Golovi} gol(a), {s.Asistencije} asistencija");

// ----------------------------------------------------------
// UPIT 5: Igrači kojima ugovor istječe prije kraja 2026.
// ----------------------------------------------------------
Console.WriteLine("\n5. IGRAČI S UGOVOROM DO KRAJA 2026.:");
var skoriIstekUgovora = sviIgraci
    .Where(p => p.ContractUntil <= new DateTime(2026, 12, 31))
    .OrderBy(p => p.ContractUntil)
    .ToList();

foreach (var p in skoriIstekUgovora)
    Console.WriteLine($"   {p.FullName} ({p.Club.Name}) - istječe: {p.ContractUntil:dd.MM.yyyy}");

// ----------------------------------------------------------
// UPIT 6: Utakmica s najvećom gledanošću
// ----------------------------------------------------------
Console.WriteLine("\n6. UTAKMICA S NAJVEĆOM GLEDANOŠĆU:");
var najpraćenaUtakmica = sveUtakmice
    .Where(u => u.Status == MatchStatus.Finished)
    .OrderByDescending(u => u.Attendance)
    .First();

Console.WriteLine($"   {najpraćenaUtakmica.HomeClub.Name} vs {najpraćenaUtakmica.AwayClub.Name}");
Console.WriteLine($"   Datum: {najpraćenaUtakmica.Date:dd.MM.yyyy} | Gledatelji: {najpraćenaUtakmica.Attendance:N0}");

// ----------------------------------------------------------
// UPIT 7: Prosječna ocjena igrača po utakmicama (samo igrači koji imaju stat)
// ----------------------------------------------------------
Console.WriteLine("\n7. PROSJEČNA OCJENA PO IGRAČU:");
var prosjecneOcjene = sviIgraci
    .Where(p => p.Stats.Count > 0)
    .Select(p => new
    {
        Ime = p.FullName,
        ProsjecnaOcjena = p.Stats.Average(s => s.Rating)
    })
    .OrderByDescending(x => x.ProsjecnaOcjena)
    .ToList();

foreach (var x in prosjecneOcjene)
    Console.WriteLine($"   {x.Ime} - prosječna ocjena: {x.ProsjecnaOcjena:F2}");

// ----------------------------------------------------------
// UPIT 8: Ozlijeđeni igrači
// ----------------------------------------------------------
Console.WriteLine("\n8. OZLIJEĐENI IGRAČI:");
var ozlijedjeni = sviIgraci
    .Where(p => p.IsInjured)
    .ToList();

if (ozlijedjeni.Count == 0)
    Console.WriteLine("   Nema ozlijeđenih igrača.");
else
    foreach (var p in ozlijedjeni)
        Console.WriteLine($"   {p.FullName} ({p.Club.Name})");

Console.WriteLine("\n========================================");
Console.WriteLine("Async/await demo:");
Console.WriteLine("========================================");

// ----------------------------------------------------------
// ASYNC/AWAIT DEMO
// ----------------------------------------------------------
await DohvatiPodatkeAsync("GNK Dinamo Zagreb");

static async Task DohvatiPodatkeAsync(string nazivKluba)
{
    Console.WriteLine($"Dohvaćanje podataka za klub '{nazivKluba}'...");
    await Task.Delay(500); // simulira async poziv (npr. prema bazi)
    Console.WriteLine($"Podaci za '{nazivKluba}' uspješno dohvaćeni!\n");
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
