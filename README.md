# football-club

Football Club is an ASP.NET Core MVC project focused on football club management operations.

The project models and visualizes:
- Club management dashboard KPIs
- Training session organization (focus area, intensity, participants, coach)
- Individual player weekly responsibilities (media, gym, therapy, massage, tactical video, and more)
- Players, coaches, clubs, and matches
- Transfers and stadium context

The app is structured as a standard MVC project with controllers, models, repositories, and Razor views, and is useful for learning C# and ASP.NET Core through a realistic club-management use case.

## Local secrets

The API expects Google OAuth settings and a JWT key. For development, use user-secrets so you do not store secrets in source control.

1. Initialize user-secrets for the web project:

	```bash
	dotnet user-secrets init --project FootballClub/FootballClub.csproj
	```

2. Set Google OAuth secrets:

	```bash
	dotnet user-secrets set "Authentication:Google:ClientId" "<client-id>" --project FootballClub/FootballClub.csproj
	dotnet user-secrets set "Authentication:Google:ClientSecret" "<client-secret>" --project FootballClub/FootballClub.csproj
	```

3. (Optional) Override the JWT key for local development:

	```bash
	dotnet user-secrets set "Jwt:Key" "<long-random-32-chars-min>" --project FootballClub/FootballClub.csproj
	```

## Run tests

Integration tests live in `FootballClub.Tests` and use an in-memory database per test.

```bash
dotnet test FootballClub.Tests/FootballClub.Tests.csproj
```