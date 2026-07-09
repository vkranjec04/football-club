using System.ComponentModel;
using FootballClub.Data;
using FootballClub.Models.Mapping;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;

namespace FootballClub.Web.Features.Training;

/// <summary>MCP tool exposing training sessions; mirrors <see cref="TrainingApiController"/>.</summary>
[McpServerToolType]
public static class TrainingMcpTools
{
    [McpServerTool(Name = "list_training_sessions")]
    [Description("Lists non-deleted training sessions, optionally filtered by club name and/or restricted to sessions starting in the future.")]
    public static List<TrainingSessionDto> ListTrainingSessions(
        ApplicationDbContext context,
        [Description("Optional case-insensitive club name filter.")] string? clubName = null,
        [Description("When true, only returns sessions starting on or after now.")] bool upcomingOnly = false)
    {
        var query = context.TrainingSessions
            .Include(session => session.Club)
            .Include(session => session.LeadStaff)
            .Where(session => !session.IsDeleted)
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(clubName))
        {
            query = query.Where(session => session.Club.Name.Contains(clubName, StringComparison.OrdinalIgnoreCase));
        }

        if (upcomingOnly)
        {
            var now = DateTime.Now;
            query = query.Where(session => session.StartTime >= now);
        }

        return query.OrderBy(session => session.StartTime).Select(session => session.ToDto()).ToList();
    }
}
