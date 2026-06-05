namespace FootballClub.Web.Services;

/// <summary>
/// Defines what to extract for one entity: free-text instructions (including any
/// lookup context such as valid enum values or existing names) plus the field list.
/// </summary>
public class AiEntitySchema
{
    public string Instructions { get; }

    public IReadOnlyList<AiFieldSpec> Fields { get; }

    public AiEntitySchema(string instructions, IReadOnlyList<AiFieldSpec> fields)
    {
        Instructions = instructions;
        Fields = fields;
    }
}
