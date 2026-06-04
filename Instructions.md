# Instructions: Running the Project for the First Time

## Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, or Docker instance) depending on your `appsettings.json` configuration.
- EF Core CLI tools (if running migrations manually: `dotnet tool install --global dotnet-ef`)
- A JWT secret in `appsettings.json` under `Jwt`.
- Google OAuth client settings in [`appsettings.Development.json` under `Authentication:Google`](https://console.cloud.google.com).

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
Modify `appsettings.json` to use LocalDB:
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

### Step 4: Update the Database
Apply the existing Entity Framework migrations to generate your SQL schema:
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