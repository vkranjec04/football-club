using System.Globalization;
using System.Text.Json;
using FootballClub.Data;
using FootballClub.Models.Enums;
using FootballClub.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Controllers;

/// <summary>
/// Browser-facing AI extraction endpoints. Each returns JSON the Create page uses to
/// pre-fill the form (human-in-the-loop): the user reviews and saves through the normal
/// Create action, so the AI never writes to the database directly.
/// </summary>
[Route("ai")]
[Authorize]
public class AiController : Controller
{
    private readonly IAiClient _ai;
    private readonly ApplicationDbContext _context;

    public AiController(IAiClient ai, ApplicationDbContext context)
    {
        _ai = ai;
        _context = context;
    }

    [HttpPost("extract/player")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExtractPlayer([FromForm] string? text, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Json(new { success = false, error = "Enter a player description." });
        }

        var clubs = _context.Clubs.OrderBy(c => c.Name).Select(c => new { c.Id, c.Name }).ToList();
        var positions = Enum.GetNames<PlayerPosition>();

        var schema = new AiEntitySchema(
            $"Extract football player data. Today's date is {DateTime.Now:yyyy-MM-dd}. If only an age is given, estimate the date of birth as January 1 of (current year minus the age). "
            + $"For position use exactly one of: {string.Join(", ", positions)}. "
            + $"For clubName choose from this list when it matches: {string.Join(", ", clubs.Select(c => c.Name))}. "
            + "Market value is in millions of euros.",
            new[]
            {
                new AiFieldSpec("firstName"),
                new AiFieldSpec("lastName"),
                new AiFieldSpec("dateOfBirth", AiFieldType.Date, "ISO date yyyy-MM-dd"),
                new AiFieldSpec("nationality"),
                new AiFieldSpec("position", AiFieldType.Enum, enumValues: positions),
                new AiFieldSpec("jerseyNumber", AiFieldType.Integer),
                new AiFieldSpec("marketValue", AiFieldType.Number, "in millions of euros"),
                new AiFieldSpec("contractUntil", AiFieldType.Date, "ISO date yyyy-MM-dd"),
                new AiFieldSpec("isInjured", AiFieldType.Boolean),
                new AiFieldSpec("clubName")
            });

        var result = await _ai.ExtractAsync(text, schema, cancellationToken);
        if (!result.Success)
        {
            return Json(new { success = false, error = result.Error });
        }

        var warnings = new List<string>(result.Warnings);
        var (clubId, clubWarning) = ResolveName(Str(result.Fields, "clubName"), clubs.Select(c => (c.Id, c.Name)).ToList(), "Club");
        if (clubWarning != null) warnings.Add(clubWarning);
        var (position, positionWarning) = MapEnum<PlayerPosition>(Str(result.Fields, "position"), "Position");
        if (positionWarning != null) warnings.Add(positionWarning);

        var data = new Dictionary<string, object?>
        {
            ["FirstName"] = Str(result.Fields, "firstName"),
            ["LastName"] = Str(result.Fields, "lastName"),
            ["DateOfBirth"] = IsoDate(result.Fields, "dateOfBirth"),
            ["Nationality"] = Str(result.Fields, "nationality"),
            ["Position"] = position,
            ["JerseyNumber"] = Int(result.Fields, "jerseyNumber"),
            ["MarketValue"] = Dec(result.Fields, "marketValue"),
            ["ContractUntil"] = IsoDate(result.Fields, "contractUntil"),
            ["IsInjured"] = Bool(result.Fields, "isInjured"),
            ["ClubId"] = clubId
        };

        return Json(new { success = true, warnings, data, dateFields = new[] { "DateOfBirth", "ContractUntil" } });
    }

    [HttpPost("extract/staff")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExtractStaff([FromForm] string? text, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Json(new { success = false, error = "Enter a staff description." });
        }

        var clubs = _context.Clubs.OrderBy(c => c.Name).Select(c => new { c.Id, c.Name }).ToList();

        var schema = new AiEntitySchema(
            $"Extract football club staff member data. Today's date is {DateTime.Now:yyyy-MM-dd}. If only an age is given, estimate the date of birth as January 1 of (current year minus the age). "
            + "Role is a free-text job title such as Head Coach, Assistant Coach, Goalkeeping Coach, Physiotherapist, Fitness Coach, Analyst. "
            + $"For clubName choose from this list when it matches: {string.Join(", ", clubs.Select(c => c.Name))}.",
            new[]
            {
                new AiFieldSpec("firstName"),
                new AiFieldSpec("lastName"),
                new AiFieldSpec("nationality"),
                new AiFieldSpec("dateOfBirth", AiFieldType.Date, "ISO date yyyy-MM-dd"),
                new AiFieldSpec("contractUntil", AiFieldType.Date, "ISO date yyyy-MM-dd"),
                new AiFieldSpec("role", AiFieldType.String, "job title"),
                new AiFieldSpec("clubName")
            });

        var result = await _ai.ExtractAsync(text, schema, cancellationToken);
        if (!result.Success)
        {
            return Json(new { success = false, error = result.Error });
        }

        var warnings = new List<string>(result.Warnings);
        var (clubId, clubWarning) = ResolveName(Str(result.Fields, "clubName"), clubs.Select(c => (c.Id, c.Name)).ToList(), "Club");
        if (clubWarning != null) warnings.Add(clubWarning);

        var data = new Dictionary<string, object?>
        {
            ["FirstName"] = Str(result.Fields, "firstName"),
            ["LastName"] = Str(result.Fields, "lastName"),
            ["Nationality"] = Str(result.Fields, "nationality"),
            ["DateOfBirth"] = IsoDate(result.Fields, "dateOfBirth"),
            ["ContractUntil"] = IsoDate(result.Fields, "contractUntil"),
            ["Role"] = Str(result.Fields, "role"),
            ["ClubId"] = clubId
        };

        return Json(new { success = true, warnings, data, dateFields = new[] { "DateOfBirth", "ContractUntil" } });
    }

    [HttpPost("extract/training")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExtractTraining([FromForm] string? text, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Json(new { success = false, error = "Enter a training session description." });
        }

        var intensities = Enum.GetNames<TrainingIntensity>();
        var staff = _context.StaffMembers
            .Where(s => !s.IsDeleted)
            .Select(s => new { s.Id, Name = s.FirstName + " " + s.LastName })
            .ToList();

        var schema = new AiEntitySchema(
            $"Extract a football training session. The current date/time is {DateTime.Now:yyyy-MM-dd HH:mm}. Resolve relative dates/times (e.g. 'tomorrow at 14:00') against it and return full ISO date-time. "
            + $"For intensity use exactly one of: {string.Join(", ", intensities)}. "
            + (staff.Count > 0
                ? $"For leadStaffName choose from this list when it matches: {string.Join(", ", staff.Select(s => s.Name))}."
                : "There are no staff members on record; leave leadStaffName null."),
            new[]
            {
                new AiFieldSpec("title"),
                new AiFieldSpec("focusArea", AiFieldType.String, "main focus, e.g. fitness, tactics, set pieces"),
                new AiFieldSpec("startTime", AiFieldType.Date, "ISO date-time yyyy-MM-ddTHH:mm:ss"),
                new AiFieldSpec("endTime", AiFieldType.Date, "ISO date-time yyyy-MM-ddTHH:mm:ss"),
                new AiFieldSpec("location"),
                new AiFieldSpec("intensity", AiFieldType.Enum, enumValues: intensities),
                new AiFieldSpec("leadStaffName"),
                new AiFieldSpec("notes")
            });

        var result = await _ai.ExtractAsync(text, schema, cancellationToken);
        if (!result.Success)
        {
            return Json(new { success = false, error = result.Error });
        }

        var warnings = new List<string>(result.Warnings);
        var (leadStaffId, staffWarning) = ResolveName(Str(result.Fields, "leadStaffName"), staff.Select(s => (s.Id, s.Name)).ToList(), "Coach");
        if (staffWarning != null) warnings.Add(staffWarning);
        var (intensity, intensityWarning) = MapEnum<TrainingIntensity>(Str(result.Fields, "intensity"), "Intensity");
        if (intensityWarning != null) warnings.Add(intensityWarning);

        var data = new Dictionary<string, object?>
        {
            ["Title"] = Str(result.Fields, "title"),
            ["FocusArea"] = Str(result.Fields, "focusArea"),
            ["StartTime"] = IsoDate(result.Fields, "startTime"),
            ["EndTime"] = IsoDate(result.Fields, "endTime"),
            ["Location"] = Str(result.Fields, "location"),
            ["Intensity"] = intensity,
            ["LeadStaffId"] = leadStaffId,
            ["Notes"] = Str(result.Fields, "notes")
        };

        return Json(new { success = true, warnings, data, dateFields = new[] { "StartTime", "EndTime" } });
    }

    // ----- helpers: read raw JSON fields (untrusted AI output) -----

    private static string? Str(IReadOnlyDictionary<string, JsonElement> fields, string key)
        => fields.TryGetValue(key, out var e) && e.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(e.GetString())
            ? e.GetString()!.Trim()
            : null;

    private static int? Int(IReadOnlyDictionary<string, JsonElement> fields, string key)
    {
        if (!fields.TryGetValue(key, out var e)) return null;
        if (e.ValueKind == JsonValueKind.Number && e.TryGetInt32(out var i)) return i;
        if (e.ValueKind == JsonValueKind.String && int.TryParse(e.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var j)) return j;
        return null;
    }

    private static decimal? Dec(IReadOnlyDictionary<string, JsonElement> fields, string key)
    {
        if (!fields.TryGetValue(key, out var e)) return null;
        if (e.ValueKind == JsonValueKind.Number && e.TryGetDecimal(out var d)) return d;
        if (e.ValueKind == JsonValueKind.String && decimal.TryParse(e.GetString(), NumberStyles.Number, CultureInfo.InvariantCulture, out var d2)) return d2;
        return null;
    }

    private static bool? Bool(IReadOnlyDictionary<string, JsonElement> fields, string key)
    {
        if (!fields.TryGetValue(key, out var e)) return null;
        return e.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => null
        };
    }

    private static string? IsoDate(IReadOnlyDictionary<string, JsonElement> fields, string key)
    {
        var raw = Str(fields, key);
        if (raw == null) return null;
        if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            return dt.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }
        if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var year) && year is >= 1900 and <= 2100)
        {
            return new DateTime(year, 1, 1).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }
        return null;
    }

    private static (string? value, string? warning) MapEnum<TEnum>(string? raw, string label) where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(raw)) return (null, null);
        return Enum.TryParse<TEnum>(raw, ignoreCase: true, out var parsed)
            ? (parsed.ToString(), null)
            : (null, $"{label} \"{raw}\" was not recognized.");
    }

    private static (int? id, string? warning) ResolveName(string? name, List<(int Id, string Name)> options, string label)
    {
        if (string.IsNullOrWhiteSpace(name)) return (null, null);

        var matches = options.Where(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
        if (matches.Count == 0)
        {
            matches = options
                .Where(o => o.Name.Contains(name, StringComparison.OrdinalIgnoreCase) || name.Contains(o.Name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (matches.Count == 1) return (matches[0].Id, null);
        if (matches.Count == 0) return (null, $"{label} \"{name}\" was not found - pick one manually.");
        return (null, $"Multiple {label} options match \"{name}\" - pick one manually.");
    }
}
