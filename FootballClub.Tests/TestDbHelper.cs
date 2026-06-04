using FootballClub.Data;
using Microsoft.Extensions.DependencyInjection;

namespace FootballClub.Tests;

internal static class TestDbHelper
{
    internal static T UseDb<T>(TestWebApplicationFactory factory, Func<ApplicationDbContext, T> action)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return action(db);
    }
}
