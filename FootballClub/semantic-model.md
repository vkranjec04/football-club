# Semantic Database Model

Ovaj dokument opisuje relacijski model podataka za aplikaciju **FootballClub**. Sadrži popis glavnih tablica (entiteta), ključnih svojstava te veza između njih.

## Entiteti i svojstva

### 1. Club
Predstavlja nogometni klub.
- **Svojstva:** `Id` (PK), `Name`, `City`, `FoundedYear`, `Budget`, `LeagueName`.
- **Veze:** 
  - **1-1** prema `Stadium` (svaki klub ima domaći stadion).
  - **1-N** prema `Coach` (klub ima trenera/stručni stožer).
  - **1-N** prema `Player` (klub ima više igrača).
  - **1-N** prema `Match` kao domaćin (`HomeMatches`).
  - **1-N** prema `Match` kao gost (`AwayMatches`).

### 2. Player
Predstavlja nogometaša.
- **Svojstva:** `Id` (PK), `FirstName`, `LastName`, `DateOfBirth`, `Position`, `JerseyNumber`, `MarketValue`, `IsInjured`.
- **Veze:**
  - **N-1** prema `Club` (više igrača igra za jedan klub).
  - **1-N** prema `PlayerStat` (igrač ima više statističkih zapisa kroz utakmice).
  - **1-N** prema `Transfer` (igrač ima povijest transfera).

### 3. Coach
Trener (glavni ili asistent).
- **Svojstva:** `Id` (PK), `FirstName`, `LastName`, `Role`, `ContractUntil`.
- **Veze:**
  - **N-1** prema `Club`.

### 4. Stadium
Nogometni stadion.
- **Svojstva:** `Id` (PK), `Name`, `Capacity`, `City`, `YearBuilt`.
- **Veze:**
  - **N-1** (ili 1-1) prema `Club` (Stadion koriste klubovi).

### 5. Match
Pojedinačna utakmica.
- **Svojstva:** `Id` (PK), `Date`, `HomeScore`, `AwayScore`, `Status`, `Attendance`, `Referee`.
- **Veze:**
  - **N-1** prema `Club` (`HomeClub`).
  - **N-1** prema `Club` (`AwayClub`).
  - **N-1** prema `Stadium`.
  - **1-N** prema `PlayerStat` (statistike igrača tijekom ove utakmice).

### 6. PlayerStat
Statistički učinak igrača po utakmici.
- **Svojstva:** `Id` (PK), `Goals`, `Assists`, `MinutesPlayed`, `YellowCards`, `RedCard`, `Rating`.
- **Veze:**
  - **N-1** prema `Player`.
  - **N-1** prema `Match`.

### 7. Transfer
Zapisivanje transfera igrača između klubova.
- **Svojstva:** `Id` (PK), `Fee`, `TransferDate`.
- **Veze:**
  - **N-1** prema `Player`.
  - **N-1** prema `Club` (`FromClub`).
  - **N-1** prema `Club` (`ToClub`).

### Dodatni entiteti (treninzi i rasporedi)
*Napomena: Ovi entiteti se trenutno uglavnom koriste u mock repozitorijima, spreči u bazi imaju slične asocijacije.*
- **TrainingSession:** N-1 prema `Club`, N-1 prema `Coach`, N-N prema `Player` (Participants).
- **PlayerScheduleItem:** N-1 prema `Player`.
