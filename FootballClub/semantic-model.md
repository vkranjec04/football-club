# Semantički model baze podataka (Football Club)

Ovaj dokument opisuje relacijski model podataka za aplikaciju **FootballClub**. Sadrži sažeti popis glavnih tablica (entiteta), njihovih ključnih svojstava te veza između njih uspostavljenih pomoću Entity Framework Core okruženja.

## Entiteti i svojstva

### 1. `Club` (Klub)
Predstavlja nogometni klub.
- **Svojstva:** `Id` (PK), `Name`, `City`, `FoundedYear`, `Budget`, `LeagueName`.
- **Veze:** 
  - **1:1** prema `Stadium` (domaći stadion).
  - **1:N** prema `Coach` (klub ima više trenera).
  - **1:N** prema `Player` (klub ima više igrača).
  - **1:N** prema `LeagueStanding` (stanje kluba u ligama).
  - **1:N** prema `Match` kao domaćin (`HomeClub`) i kao gost (`AwayClub`).

### 2. `Player` (Igrač)
Predstavlja nogometaša.
- **Svojstva:** `Id` (PK), `FirstName`, `LastName`, `DateOfBirth`, `Nationality`, `Position`, `JerseyNumber`, `MarketValue`, `ContractUntil`, `IsInjured`.
- **Veze:**
  - **N:1** prema `Club` (igrač igra za jedan klub).
  - **1:N** prema `PlayerStat` (igrač ima više statističkih zapisa tijekom utakmica).
  - **1:N** prema `Transfer` (igrač ima povijest transfera).
  - **1:N** prema `PlayerScheduleItem` (dnevne aktivnosti igrača).

### 3. `Coach` (Trener)
Trener (glavni ili asistent).
- **Svojstva:** `Id` (PK), `FirstName`, `LastName`, `Nationality`, `DateOfBirth`, `ContractUntil`, `Role`.
- **Veze:**
  - **N:1** prema `Club` (pripada klubu).
  - **1:N** prema `TrainingSession` (trener vodi treninge).

### 4. `Stadium` (Stadion)
Nogometni stadion.
- **Svojstva:** `Id` (PK), `Name`, `City`, `Capacity`, `YearBuilt`.
- **Veze:**
  - Pripada klubu kao domaći stadion i koristi se kao mjesto odigravanja u `Match` (utakmici).

### 5. `Match` (Utakmica)
Pojedinačna odigrana ili zakazana utakmica.
- **Svojstva:** `Id` (PK), `Date`, `HomeScore`, `AwayScore`, `Status`, `Attendance`, `Referee`, `Round`.
- **Veze:**
  - **N:1** prema `Club` (`HomeClubId`).
  - **N:1** prema `Club` (`AwayClubId`).
  - **N:1** prema `Stadium` (gdje se igra).
  - **1:N** prema `PlayerStat` (statistike igrača ostvarene na ovoj utakmici - ovo realizira i N:N ovisnost između igrača i utakmice).

### 6. `PlayerStat` (Statistika igrača - Asocijativna tablica)
Statistički učinak igrača po utakmici.
- **Svojstva:** `Id` (PK), `MinutesPlayed`, `Goals`, `Assists`, `YellowCards`, `RedCards`, `Rating`.
- **Veze:**
  - **N:1** prema `Player`.
  - **N:1** prema `Match`.

### 7. `Transfer` (Transfer)
Zapis transfera igrača između dva kluba.
- **Svojstva:** `Id` (PK), `TransferDate`, `TransferFee`.
- **Veze:**
  - **N:1** prema `Player`.
  - **N:1** prema `Club` (`FromClubId`).
  - **N:1** prema `Club` (`ToClubId`).

### 8. `TrainingSession` (Trening)
Sadrži detalje odrađenih ili planiranih treninga.
- **Svojstva:** `Id` (PK), `Date`, `DurationMinutes`, `Intensity`, `Focus`.
- **Veze:**
  - **N:1** prema `Coach` (koji drži trening).

### 9. `LeagueStanding` (Poredak tablice)
Predstavlja učinak kluba unutar neke sezone.
- **Svojstva:** `Id` (PK), `Season`, `MatchesPlayed`, `Wins`, `Draws`, `Losses`, `GoalsFor`, `GoalsAgainst`, `Points`.
- **Veze:**
  - **N:1** prema `Club`.

### 10. `PlayerScheduleItem` (Raspored / Obaveze)
Prikazuje zasebne događaje ili aktivnosti u kalendaru za zaposlenika.
- **Svojstva:** `Id` (PK), `Title`, `Description`, `StartTime`, `EndTime`, `ResponsibilityType`.
- **Veze:**
  - **N:1** prema `Player`.
