# Entity Framework Skill

Purpose
- When to use: Use this skill whenever you add or modify EF model classes, configure the `ApplicationDbContext`, generate migrations, or update the database schema.
- Scope: Workspace-scoped — aimed at developers working on the `FootballClub` project using EF Core.

Quick Summary
- Add or update a model class (data annotations + navigation properties).
- Configure relationships, keys, indexes, and cascade rules in `ApplicationDbContext.OnModelCreating` using the Fluent API.
- Generate a migration, review it, then apply it to the database.

Preconditions / Assumptions
- Project uses EF Core and has an `ApplicationDbContext` registered in DI.
- `Microsoft.EntityFrameworkCore.Design` is referenced in the project.
- `dotnet-ef` is available (global tool or CLI package).
- You can build the solution locally before creating migrations.

Checklist (Quality Criteria)
- Project builds successfully.
- New/changed model classes compile.
- Migrations reflect only intended schema changes.
- Database update runs without runtime errors and preserves existing data where expected.
- Tests (if present) that depend on DB schema pass.

Step-by-step: Add a New Model Class (with Data Annotations)
1. Create the model file under `Models/` (or your chosen folder) and include required namespaces:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

public class Player
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public PlayerPosition Position { get; set; }

    // Navigation properties
    public int? CoachId { get; set; }
    [ForeignKey(nameof(CoachId))]
    public Coach Coach { get; set; }

    public IList<PlayerStat> PlayerStats { get; set; } = new List<PlayerStat>();
}

public class PlayerStat
{
    [Key]
    public int Id { get; set; }

    public int PlayerId { get; set; }
    [ForeignKey(nameof(PlayerId))]
    public Player Player { get; set; }

    public int Goals { get; set; }
    public int Assists { get; set; }
}
```

Notes on annotations
- `[Key]` marks the primary key (EF conventions usually pick `Id` or `<TypeName>Id`).
- `[Required]` makes the column NOT NULL.
- `[MaxLength(n)]` sets column length for string/varchar.
- `[ForeignKey("PropName")]` or using navigation property naming conventions helps EF discover relationships.
- Use `[NotMapped]` for properties you don't want persisted.

When to use Data Annotations vs Fluent API
- Use data annotations for simple constraints (`[Required]`, `[MaxLength]`, `[Column]`).
- Use Fluent API in `OnModelCreating` for complex mapping, composite keys, indexes, many-to-many join tables, or when annotations aren't available.

Step-by-step: Configure Relationships in `ApplicationDbContext.OnModelCreating`
1. Open `Data/ApplicationDbContext.cs` and locate the `OnModelCreating(ModelBuilder modelBuilder)` override.
2. Add Fluent API configuration for relationships, cascade behavior, keys, and indexes. Example:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Player -> PlayerStat : one-to-many
    modelBuilder.Entity<Player>()
        .HasMany(p => p.PlayerStats)
        .WithOne(s => s.Player)
        .HasForeignKey(s => s.PlayerId)
        .OnDelete(DeleteBehavior.Cascade); // or Restrict/SetNull depending on your rules

    // Player -> Coach : many players to one coach
    modelBuilder.Entity<Player>()
        .HasOne(p => p.Coach)
        .WithMany(c => c.Players)
        .HasForeignKey(p => p.CoachId)
        .OnDelete(DeleteBehavior.SetNull);

    // Example of configuring an index
    modelBuilder.Entity<Player>()
        .HasIndex(p => p.Name)
        .IsUnique(false);

    // Composite key example
    // modelBuilder.Entity<SomeEntity>().HasKey(e => new { e.KeyA, e.KeyB });
}
```

Tips for relationships
- Choose cascade behaviors deliberately: Cascade may delete dependent rows; SetNull or Restrict prevents accidental deletions.
- For many-to-many in EF Core 5+, either use implicit join entities or define an explicit join entity with its own class.
- Use `.HasConstraintName("FK_Name")` when you need specific DB constraint names for migrations.

Step-by-step: Generate and Apply Migrations
1. Build the project first to ensure models compile:

```bash
dotnet build
```

2. Create a migration (CLI examples assume running from the solution or project folder):

- Using `dotnet ef` (recommended for cross-platform CLI):

```bash
# Run from solution folder or adjust --project and --startup-project
dotnet ef migrations add AddPlayerAndPlayerStat --project FootballClub --startup-project FootballClub
```

- If you use Visual Studio's Package Manager Console:

```powershell
# Set Default Project to FootballClub in the PMC, then:
Add-Migration AddPlayerAndPlayerStat -Project FootballClub -StartupProject FootballClub
```

3. Inspect the generated migration files under `Migrations/`.
- Confirm Up() and Down() methods look correct and only include intended schema changes.
- Edit migration if unavoidable but be careful; prefer changing the model and regenerating whenever possible.

4. Apply the migration to the database:

```bash
# CLI
dotnet ef database update --project FootballClub --startup-project FootballClub

# PMC
Update-Database -Project FootballClub -StartupProject FootballClub
```

Troubleshooting migration issues
- If EF can't find the `DbContext`, add `--context ApplicationDbContext` to the `dotnet ef` commands.
- If multiple projects exist, always specify `--project` and `--startup-project`.
- If migrations include unexpected changes, clean the model, rebuild, and regenerate a migration.
- If `dotnet ef` isn't installed:

```bash
dotnet tool install --global dotnet-ef
# or ensure package
dotnet add package Microsoft.EntityFrameworkCore.Design
```

Seeding data and Data Migrations
- Prefer using `DataSeeder` (if present) or `IHostedService` during startup to seed non-production sample data.
- For data migrations (schema + data changes), consider a custom migration step in the generated migration's `Up()` method that uses `Sql()` or `migrationBuilder.UpdateData()`.

Safety & Review
- Always run migrations in a development environment first.
- Back up production databases before applying migrations.
- If using CI/CD: include a step to run `dotnet ef migrations script` to produce a SQL script for review and controlled application.

Example: create SQL script for PR review

```bash
dotnet ef migrations script --idempotent --output migrations.sql --project FootballClub --startup-project FootballClub
```

Prompts & Usage Examples
- "Add a `Season` model with `StartDate` and `EndDate`, configure one-to-many `Season->Matches`, and generate the migration named `AddSeason`." 
- "Configure many-to-many between `Player` and `TrainingSession` with an explicit join entity `PlayerTraining` and create migration." 

Ambiguities / Questions to Clarify
- Preferred cascade-delete policy for each relationship (Cascade vs Restrict vs SetNull).
- Naming conventions for migrations and constraint names.
- Whether to prefer data annotations or Fluent API for your codebase style consistency.

Next Steps / Iteration
1. Use this skill when editing models or `ApplicationDbContext`.
2. If you'd like, I can:
   - Draft example model(s) tailored to your app (e.g., `PlayerTraining` join entity).
   - Generate a sample migration file for review.
   - Add a `skills` check-list integration that runs `dotnet ef` commands automatically in a scripted workflow.

Related customizations to create next
- A `SKILL.md` for EF migrations policy (naming, release process, review checklist).
- A scripted `make-migration` helper script or Cake/PS script to standardize `dotnet ef` parameters.

End of skill.
