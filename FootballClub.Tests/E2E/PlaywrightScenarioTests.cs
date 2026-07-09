using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Xunit;
using static Microsoft.Playwright.Assertions;

namespace FootballClub.Tests.E2E;

/// <summary>
/// Playwright end-to-end scenario ("Playwright scenarij 10 koraka" grading criterion): a single
/// user journey through the real UI in a real (headless) Chromium, against the app self-hosted
/// on Kestrel with the seeded in-memory database. The ten steps cover the auth gate,
/// registration, navigation, the full CRUD cycle on a player, global search and logout.
/// </summary>
public class PlaywrightScenarioTests : IAsyncLifetime
{
    private const string Username = "e2e.tester";
    private const string Password = "Playwright1";
    private const string PlayerFullName = "Zvonimir Playwright";

    private E2EWebApplicationFactory _factory = null!;
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;

    public async Task InitializeAsync()
    {
        // Idempotent browser download: a fast no-op when Chromium is already installed, so plain
        // `dotnet test` works on a machine that never ran Playwright before.
        var exitCode = Microsoft.Playwright.Program.Main(new[] { "install", "chromium" });
        Assert.Equal(0, exitCode);

        _factory = new E2EWebApplicationFactory(Guid.NewGuid().ToString("N"));
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
    }

    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task TenStepUserJourney()
    {
        try
        {
            await RunJourneyAsync();
        }
        catch (Exception ex)
        {
            // Surface server-side warnings/errors next to the Playwright failure; a browser test
            // otherwise only sees the symptom (missing element), not the server exception.
            throw new Xunit.Sdk.XunitException(
                $"{ex.Message}\n--- Server logs ---\n{string.Join("\n", _factory.ServerLogs)}", ex);
        }
    }

    private async Task RunJourneyAsync()
    {
        var baseUrl = _factory.ServerAddress;
        // en-US pins the request culture (and thus date/number formats) regardless of the
        // machine the test runs on.
        var context = await _browser.NewContextAsync(new BrowserNewContextOptions { Locale = "en-US" });
        var page = await context.NewPageAsync();

        // ----- Step 1: anonymous visit is bounced to the login page (client-side auth gate) -----
        await page.GotoAsync(new Uri(baseUrl, "/team-roster").ToString());
        await Expect(page).ToHaveURLAsync(new Regex("/Account/Login"));

        // ----- Step 2: register a new account through the UI -----
        await page.GotoAsync(new Uri(baseUrl, "/Account/Register").ToString());
        await page.FillAsync("#username", Username);
        await page.FillAsync("#email", "e2e.tester@example.com");
        await page.FillAsync("#password", Password);
        await page.FillAsync("#confirmPassword", Password);
        await page.ClickAsync("button[type=submit]");

        // ----- Step 3: registration lands on the dashboard, topbar shows the session -----
        await Expect(page).ToHaveURLAsync(new Regex($"^{Regex.Escape(baseUrl.ToString())}(\\?.*)?$"));
        await Expect(page.Locator("#authStatus")).ToContainTextAsync(Username);

        // ----- Step 4: navigate to Players via the sidebar menu -----
        await page.ClickAsync(".sidebar-nav a:has-text('Players')");
        await Expect(page).ToHaveURLAsync(new Regex("/team-roster"));
        await Expect(page.Locator("h3", new PageLocatorOptions { HasTextString = "Player Roster" })).ToBeVisibleAsync();

        // ----- Step 5: create a new player through the Create form (CRUD: Create) -----
        await page.ClickAsync("a:has-text('Add player')");
        await Expect(page).ToHaveURLAsync(new Regex("/Player/Create"));
        await page.FillAsync("input[name='FirstName']", "Zvonimir");
        await page.FillAsync("input[name='LastName']", "Playwright");
        await page.FillAsync("input[name='Nationality']", "Croatian");
        await page.SelectOptionAsync("select[name='Position']", "Midfielder");
        await page.FillAsync("input[name='JerseyNumber']", "42");
        await page.FillAsync("input[name='MarketValue']", "10");
        await page.SelectOptionAsync("select[name='ClubId']", new SelectOptionValue { Label = "Dinamo Zagreb" });
        await page.ClickAsync("form[action='/Player/Create'] button[type=submit]");

        // ----- Step 6: the new player is listed in the roster (CRUD: Read) -----
        await Expect(page).ToHaveURLAsync(new Regex("/team-roster"));
        var playerRow = page.Locator("#playersTableBody tr", new PageLocatorOptions { HasTextString = PlayerFullName });
        await Expect(playerRow).ToBeVisibleAsync();
        await Expect(playerRow).ToContainTextAsync("#42");

        // ----- Step 7: find the player via global search (Ctrl+K) and open the details page -----
        await page.Keyboard.PressAsync("Control+k");
        await page.FillAsync("#globalSearchInput", "Playwright");
        // Wait for the live record results (the "Igrači" group renders after the API replies).
        await Expect(page.Locator(".global-search__group-label", new PageLocatorOptions { HasTextString = "Igrači" })).ToBeVisibleAsync();
        await page.ClickAsync($".global-search__item:has-text('{PlayerFullName}')");
        await Expect(page).ToHaveURLAsync(new Regex("/player-profile/\\d+"));
        await Expect(page.Locator(".page-body")).ToContainTextAsync(PlayerFullName);

        // ----- Step 8: edit the player and save (CRUD: Update) -----
        await page.ClickAsync("a:has-text('Edit Player')");
        await Expect(page).ToHaveURLAsync(new Regex("/Player/Edit/\\d+"));
        await page.FillAsync("input[name='JerseyNumber']", "99");
        await page.ClickAsync("button[type=submit]:has-text('Save')");
        await Expect(page).ToHaveURLAsync(new Regex("/team-roster"));
        await Expect(playerRow).ToContainTextAsync("#99");

        // ----- Step 9: delete the player from the details page (CRUD: Delete, soft delete) -----
        await playerRow.Locator("a:has-text('Details')").ClickAsync();
        page.Dialog += (_, dialog) => dialog.AcceptAsync(); // the delete form asks for confirm()
        await page.ClickAsync("button:has-text('Delete Player')");
        await Expect(page).ToHaveURLAsync(new Regex("/team-roster"));
        await Expect(playerRow).ToContainTextAsync("Deleted");

        // ----- Step 10: log out; the auth gate blocks the app again -----
        await page.ClickAsync("#authLogoutLink");
        await Expect(page).ToHaveURLAsync(new Regex("/Account/Login"));
        await page.GotoAsync(new Uri(baseUrl, "/team-roster").ToString());
        await Expect(page).ToHaveURLAsync(new Regex("/Account/Login"));
    }
}
