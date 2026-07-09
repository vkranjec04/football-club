using System.ComponentModel;
using FootballClub.Data;
using FootballClub.Models.Mapping;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace FootballClub.Web.Features.Staff;

/// <summary>MCP tools exposing club staff data; mirrors <see cref="StaffApiController"/>.</summary>
[McpServerToolType]
public static class StaffMcpTools
{
    [McpServerTool(Name = "list_staff")]
    [Description("Lists non-deleted club staff members (coaches, physios, analysts, ...), optionally filtered by name or role.")]
    public static List<StaffDto> ListStaff(
        ApplicationDbContext context,
        [Description("Optional case-insensitive filter matched against first/last name or role.")] string? search = null)
    {
        var q = (search ?? string.Empty).Trim();
        var query = context.StaffMembers.Include(staff => staff.Club).Where(staff => !staff.IsDeleted).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(staff =>
                staff.FirstName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                staff.LastName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                staff.FullName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                staff.Role.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        return query.OrderBy(staff => staff.LastName).ThenBy(staff => staff.FirstName).Select(staff => staff.ToDto()).ToList();
    }

    [McpServerTool(Name = "get_staff")]
    [Description("Gets a single staff member by id.")]
    public static StaffDto GetStaff(ApplicationDbContext context, [Description("Staff member id.")] int id)
    {
        var staff = context.StaffMembers.Include(s => s.Club).FirstOrDefault(s => s.Id == id && !s.IsDeleted);
        return staff?.ToDto() ?? throw new McpException($"Staff member {id} was not found.");
    }
}
