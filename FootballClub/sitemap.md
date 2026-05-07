# Sitemap i Model usmjeravanja (Routing)

Ovaj dokument opisuje sve dostupne URL-ove unutar **FootballClub** aplikacije, uz vezane controllere, akcije i odgovarajuće view-ove (Razor stranice). Sukladno zahtjevima, implementiran je i opcionalni `[Route]` (Attribute Routing) za 4+ akcije.

## Mapiranje ruta i stranica

| URL / Ruta | Controller | Action | View File | Napomena / Routing tip |
| :--- | :--- | :--- | :--- | :--- |
| `/` ili `/Home/Index` | `HomeController` | `Index` | `Views/Home/Index.cshtml` | Početni (Dashboard) pregled kluba. **Default** usmjeravanje. |
| `/league-standings` | `LeagueController` | `Index` | `Views/League/Index.cshtml` | **Custom / Attribute Ruta:** `[Route("league-standings")]`. Prikaz tablice. |
| `/standings/{leagueName?}` | `LeagueController` | `Index` | `Views/League/Index.cshtml` | **Custom / Attribute Ruta:** `[Route("standings/{leagueName?}")]`. |
| `/team-roster` | `PlayerController` | `Index` | `Views/Player/Index.cshtml` | **Custom / Attribute Ruta:** `[Route("team-roster")]`. Roster igrača. |
| `/player-profile/{id}` | `PlayerController` | `Details` | `Views/Player/Details.cshtml` | **Custom / Attribute Ruta:** `[Route("player-profile/{id:int}")]`. Profil igrača. |
| `/staff-members` | `CoachController` | `Index` | `Views/Coach/Index.cshtml` | **Custom / Attribute Ruta:** `[Route("staff-members")]`. Pregled trenera i osoblja. |
| `/Match` | `MatchController` | `Index` | `Views/Match/Index.cshtml` | **Custom / Attribute Ruta:** `[Route("[controller]")]`. Raspored i rezultati. |
| `/Match/Details/{id}` | `MatchController` | `Details` | `Views/Match/Details.cshtml` | **Custom / Attribute Ruta:** `[HttpGet("Details/{id:int}")]`. Detalji s utakmice. |
| `/Player/Schedule` | `PlayerController` | `Schedule` | `Views/Player/Schedule.cshtml` | **Default** usmjeravanje. Tjedni kalendar i obveze. |
| `/Training/Index` | `TrainingController` | `Index` | `Views/Training/Index.cshtml` | **Default** usmjeravanje. Povijest i plan treninga. |
| `/Training/Details/{id}` | `TrainingController` | `Details` | `Views/Training/Details.cshtml` | **Default** usmjeravanje. Parametri određenog treninga. |
| `/Coach/Details/{id}` | `CoachController` | `Details` | `Views/Coach/Details.cshtml` | **Default** usmjeravanje. Detalji odabranog trenera. |
| `/Medical/Index` | `MedicalController` | `Index` | `Views/Medical/Index.cshtml` | **Default** usmjeravanje. Medicinski karton ozlijeđenih igrača. |
| `/Tactics/Index` | `TacticsController` | `Index` | `Views/Tactics/Index.cshtml` | **Default** usmjeravanje. Taktička konfiguracija formacije. |

## Pregled konfiguracije

Osnovno globalno mapiranje podešeno je u `Program.cs` prema *Pattern* ruti:
```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

Navedeni kontroleri (`MatchController`, `LeagueController`, `PlayerController`, `CoachController`) imaju dodano lokalno prebrisano **usmjeravanje pomoću atributa**. Npr. za pristup profilu igrača ne kucamo `/Player/Details/1` nego prilagođen SEO-friendly url `/player-profile/1`.
