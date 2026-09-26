using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Nameless.Microservices.Common.Chores.Inputs;

public sealed record ListChoresInput {
    [Description("A value to search in the chore title")]
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [Description("A value to search in the chore description")]
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [Description("The start range for due date")]
    [JsonPropertyName("dueDateStart")]
    public DateTimeOffset? DueDateStart { get; init; }

    [Description("The end range for due date")]
    [JsonPropertyName("dueDateEnd")]
    public DateTimeOffset? DueDateEnd { get; init; }

    [Description("The start range for conclusion date")]
    [JsonPropertyName("conclusionDateStart")]
    public DateTimeOffset? ConclusionDateStart { get; init; }

    [Description("The end range for conclusion date")]
    [JsonPropertyName("conclusionDateEnd")]
    public DateTimeOffset? ConclusionDateEnd { get; init; }

    [Description("Only those chores that have a conclusion date")]
    [JsonPropertyName("done")]
    public bool? Done { get; init; }
}