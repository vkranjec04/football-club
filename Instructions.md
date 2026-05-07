# Instructions: Running the Project for the First Time

## Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, or Docker instance) depending on your `appsettings.json` configuration.
- EF Core CLI tools (if running migrations manually: `dotnet tool install --global dotnet-ef`)

## Steps to Run

1. **Navigate to the core project directory**  
   Open your terminal and ensure you are in the folder containing `FootballClub.csproj`:
   ```bash
   cd FootballClub
   ```

2. **Restore dependencies**  
   Download all required NuGet packages:
   ```bash
   dotnet restore
   ```

3. **Update the Database**  
   Apply the existing Entity Framework migrations to generate your SQL schema:
   ```bash
   dotnet ef database update
   ```
   *Note: Ensure your database server is running and the connection string in `appsettings.json` or `appsettings.Development.json` is correct.*

4. **Build and Run the project**  
   Start the application:
   ```bash
   dotnet run
   ```

5. **Open in Browser**  
   Check the terminal output for the local URL (typically `http://localhost:5000` or `https://localhost:5001` upwards) and open it in your web browser.