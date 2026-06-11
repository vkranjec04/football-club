namespace FootballClub.Web.Filters;

/// <summary>
/// Marks a controller or action that the <see cref="ActivityLogActionFilter"/> should not
/// auto-log — used where logging is handled explicitly with richer detail (authentication)
/// or where a generic entry would just be noise.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class SkipActivityLogAttribute : Attribute
{
}
