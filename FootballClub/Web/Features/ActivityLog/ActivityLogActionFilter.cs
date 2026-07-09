using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FootballClub.Web.Features.ActivityLog;

/// <summary>
/// Global action filter that records every state-changing request (POST/PUT/PATCH/DELETE)
/// to the audit trail via <see cref="IActivityLogger"/>. This is the automatic half of the
/// logging mechanism: it captures MVC form posts and JSON API calls alike with no
/// per-controller code. Actions or controllers marked <see cref="SkipActivityLogAttribute"/>
/// are ignored (e.g. authentication, which is logged explicitly with richer detail).
/// </summary>
public sealed class ActivityLogActionFilter : IAsyncActionFilter
{
    private static readonly HashSet<string> MutatingMethods =
        new(StringComparer.OrdinalIgnoreCase) { "POST", "PUT", "PATCH", "DELETE" };

    private readonly IActivityLogger _activityLogger;

    public ActivityLogActionFilter(IActivityLogger activityLogger)
    {
        _activityLogger = activityLogger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        var shouldLog = MutatingMethods.Contains(request.Method)
            && !context.ActionDescriptor.EndpointMetadata.OfType<SkipActivityLogAttribute>().Any();

        var executed = await next();

        if (!shouldLog)
        {
            return;
        }

        var routeValues = context.RouteData.Values;
        var controller = routeValues.TryGetValue("controller", out var c) ? c?.ToString() : null;
        var action = routeValues.TryGetValue("action", out var a) ? a?.ToString() : null;
        var id = routeValues.TryGetValue("id", out var i) ? i?.ToString() : null;

        // In an action filter the result has not executed yet, so the response status code is
        // not reliable; read it from the result itself where the result exposes one.
        var statusCode = (executed.Result as IStatusCodeActionResult)?.StatusCode;
        var hadException = executed.Exception != null && !executed.ExceptionHandled;
        var success = !hadException
            && (statusCode is null || statusCode.Value < 400)
            && context.ModelState.IsValid;

        var outcome = statusCode?.ToString() ?? (success ? "OK" : "FAILED");
        var description = $"{request.Method} {request.Path}{request.QueryString} -> {outcome}";

        await _activityLogger.LogAsync(
            action: string.IsNullOrWhiteSpace(action) ? request.Method : action!,
            entityType: controller,
            entityId: id,
            description: description,
            success: success);
    }
}
