# Instructions: Running the Project for the First Time

## Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, or Docker instance) — connection string lives in `appsettings.Development.json`.
- EF Core CLI tools (if running migrations manually: `dotnet tool install --global dotnet-ef`)

## Configuration (important)
The base `appsettings.json` ships with **empty placeholders** — real secrets are injected from Azure App Settings in production and are **not** committed. For **local development**, all secrets live in `appsettings.Development.json` (which is loaded only when `ASPNETCORE_ENVIRONMENT=Development`, the default for `dotnet run`):

| Setting | Local location |
|---|---|
| `ConnectionStrings:DefaultConnection` | `appsettings.Development.json` |
| `Jwt:Key` | `appsettings.Development.json` |
| `Authentication:Google:ClientId` / `ClientSecret` | `appsettings.Development.json` (get from [Google Cloud Console](https://console.cloud.google.com)) |
| `Storage:ConnectionString` | leave **empty** locally → attachments are saved to `wwwroot/uploads` (no Azure needed) |
| `Seed:DemoData` | `true` seeds sample clubs/players/etc. on first run |

> Note: if `Authentication:Google:*` is left as the placeholder value the app still starts and all non-Google features work; only the Google sign-in button will fail until you set real credentials.

## Authentication Setup
- The app seeds two local accounts on startup if no users exist: `admin` / `Admin123!` and `user` / `User123!`.
- Use the local login endpoint at `POST /api/auth/login` for username/password authentication.
- Use `GET /api/auth/google-login` to start the Google sign-in flow.
- The app now requires HTTPS redirection for external OAuth, so use the `https://localhost:5001` URL when testing Google login.

## Steps to Run

### Step 1: Start SQL Server
Choose one of the following options:

**Option A: Using Docker (Recommended)**  
If you have Docker installed, start a SQL Server instance:
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 --name ClubDB -d mcr.microsoft.com/mssql/server:2022-latest
```
If the container already exists, start it instead:
```bash
docker start ClubDB
```
*(Wait 10-15 seconds for SQL Server to fully initialize after the container starts)*

**Option B: Using Local SQL Server Express**  
If you have SQL Server Express installed locally:
- Open SQL Server Management Studio (SSMS)
- Connect to your local SQL Server instance
- Ensure TCP/IP is enabled in SQL Server Configuration Manager

**Option C: Using LocalDB**  
Modify `appsettings.Development.json` to use LocalDB:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ClubDB;Integrated Security=true;"
```

### Step 2: Navigate to the project directory
Open your terminal and ensure you are in the folder containing `FootballClub.csproj`:
```bash
cd FootballClub
```

### Step 3: Restore dependencies
Download all required NuGet packages:
```bash
dotnet restore
```

### Step 4: Update the Database (optional)
The app **applies pending migrations automatically on startup**, so this step is optional. To apply them manually:
```bash
dotnet ef database update
```

### Step 5: Build and Run the project
Start the application:
```bash
dotnet run
```

### Step 6: Open in Browser
Check the terminal output for the local URL (typically `https://localhost:5001`) and open it in your web browser.
If you only need the public pages, `http://localhost:5000` may still be shown, but Google OAuth callbacks should be tested over HTTPS.

---

## Running on Azure (Deployed)

The app is deployed to Azure free tier and is publicly available — no local setup needed to use it:

**Live URL:** https://footballclub-web-e9f2fuf8f2fzcvbe.francecentral-01.azurewebsites.net

Architecture:
- **App Service** (F1 Free, Linux, .NET 8) hosts the app.
- **Azure SQL Database** (Free offer, serverless) holds the data; schema is created/migrated automatically on first startup.
- **Azure Blob Storage** (container `attachments`) holds uploaded files (because the App Service disk is ephemeral). Selected automatically when `Storage:ConnectionString` is set.
- Production secrets (connection string, JWT key, Google OAuth, storage) live in **App Service → Settings → Environment variables**, never in the repo.

### Deploying changes (CI/CD)
Deployment is automatic via **GitHub Actions** (`.github/workflows/master_footballclub-web.yml`):
```bash
git push origin master   # builds FootballClub.csproj, publishes, and deploys to App Service
```
Watch progress in the repo's **Actions** tab.

### Notes
- **First request after ~20 min idle is slow (~30-60s):** F1 has no Always On and the serverless database auto-pauses; it wakes on the first hit, then responds normally.
- **Google login** requires the Authorized redirect URI in Google Cloud Console to be `https://footballclub-web-e9f2fuf8f2fzcvbe.francecentral-01.azurewebsites.net`.
- Seeded accounts work the same as locally: `admin` / `Admin123!` and `user` / `User123!`.

---

## Troubleshooting

**Error: "An existing connection was forcibly closed by the remote host"**
- Ensure SQL Server is running (check Docker container or SQL Server service)
- Wait 15+ seconds after starting the Docker container before running the app
- Try running `dotnet run` again to trigger automatic retry logic

**Error: "A network-related or instance-specific error occurred"**
- Verify your SQL Server instance is accessible at `localhost:1433`
- Check the connection string in `appsettings.json` matches your SQL Server setup
- If using LocalDB, update the connection string as shown above

**Database already exists but is empty**
- Delete the database: `dotnet ef database drop --force`
- Recreate it: `dotnet ef database update`
- The app will automatically seed sample data on startup

**Error: "The process cannot access the file because it is being used by another process"**
- Stop any running FootballClub instance before rebuilding
- Press Ctrl+C in the terminal running `dotnet run`, or use `Stop-Process -Name FootballClub -Force`
- Rebuild only after the executable is no longer running

**Error: Docker says the container name `ClubDB` is already in use**
- The container already exists
- Start it with `docker start ClubDB` instead of `docker run`
- If you want a fresh container, remove the old one first with `docker rm -f ClubDB`