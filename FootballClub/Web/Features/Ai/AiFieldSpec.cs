namespace FootballClub.Web.Features.Ai;

/// <summary>Describes one field the AI should extract; used to build the JSON response schema.</summary>
public class AiFieldSpec
{
    public string Name { get; }

    public AiFieldType Type { get; }

    public string? Description { get; }

    public IReadOnlyList<string>? EnumValues { get; }

    public AiFieldSpec(string name, AiFieldType type = AiFieldType.String, string? description = null, IReadOnlyList<string>? enumValues = null)
    {
        Name = name;
        Type = type;
        Description = description;
        EnumValues = enumValues;
    }
}
