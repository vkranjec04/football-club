# Sitemap / Routing Model

Ovaj dokument opisuje sve dostupne rute (URL-ove) unutar **FootballClub** ASP.NET Core aplikacije te mapira odgovarajući Controller, Akciju i Razor View.

| URL / Ruta | Controller | Action | View File | Napomena |
| :--- | :--- | :--- | :--- | :--- |
| `/` ili `/Home/Index` | `HomeController` | `Index` | `Views/Home/Index.cshtml` | Početni (Dashboard) pregled. Default ruta. |
| `/league-standings` | `LeagueController` | `Index` | `Views/League/Index.cshtml` | **Custom Ruta.** Prikazuje poredak u ligi. |
| `/standings` ili `/standings/{leagueName}` | `LeagueController` | `Index` | `Views/League/Index.cshtml` | **Custom Ruta.** Alternativni pregled poretka. |
| `/team-roster` | `PlayerController` | `Index` | `Views/Player/Index.cshtml` | **Custom Ruta.** Prikaz svih igrača u rosteru. |
| `/players/list` | `PlayerController` | `Index` | `Views/Player/Index.cshtml` | **Custom Ruta.** Alternativni URL za popis. |
| `/player-profile/{id}` | `PlayerController` | `Details` | `Views/Player/Details.cshtml` | **Custom Ruta.** Detaljan pregled odabranog igrača (constraint na int). |
| `/Player/Schedule` | `PlayerController` | `Schedule` | `Views/Player/Schedule.cshtml` | Prikazuje poseban raspored igrača. Koristi default usmjeravanje. |
| `/fixtures-and-results`| `MatchController` | `Index` | `Views/Match/Index.cshtml` | **Custom Ruta.** Prikaz završenih i nadolazećih utakmica. |
| `/Match/Details/{id}` | `MatchController` | `Details` | `Views/Match/Details.cshtml` | Detalji pojedinačne utakmice. Koristi default usmjeravanje. |
| `/Training/Index` | `TrainingController` | `Index` | `Views/Training/Index.cshtml` | Pregled treninga kluba. Koristi default usmjeravanje. |
| `/Training/Details/{id}` | `TrainingController` | `Details` | `Views/Training/Details.cshtml` | Detaljan pregled određenog treninga. Koristi default usmjeravanje. |
| `/Coach/Index` | `CoachController` | `Index` | `Views/Coach/Index.cshtml` | Pregled osoblja/trenera. Koristi default usmjeravanje. |
| `/Coach/Details/{id}` | `CoachController` | `Details` | `Views/Coach/Details.cshtml` | Detalji odabranog trenera. Koristi default usmjeravanje. |
| `/Medical/Index` | `MedicalController` | `Index` | `Views/Medical/Index.cshtml` | Prikaz ozljeđenih igrača. Koristi default usmjeravanje. |
| `/Tactics/Index` | `TacticsController` | `Index` | `Views/Tactics/Index.cshtml` | Taktička ploča kluba. Koristi default usmjeravanje. |

## Routing Configuration

Aplikacija primarno koristi ugrađenu *Pattern* rutu u `Program.cs`:
```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

Međutim, **Attribute Routing** nadjačava ili proširuje ove rute na navedenim Controllerima. Na primjer:
- `LeagueController` ima `[Route("league-standings")]` nad `Index()`
- `PlayerController` ima `[Route("player-profile/{id:int}")]` nad `Details(...)`
- `PlayerController` ima `[Route("team-roster")]` i `[Route("players/list")]` nad `Index(...)`
- `MatchController` ima `[Route("fixtures-and-results")]` nad `Index()`
