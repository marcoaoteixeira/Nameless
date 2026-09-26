using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Nameless.Microservices.Common.Chores.Inputs;

public sealed record CreateChoreInput {
    [Description("The chore title")]
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [Description("The chore description")]
    [JsonPropertyName("description")]
    public required string Description { get; init; }

    [Description("The chore due date")]
    [JsonPropertyName("dueDate")]
    public DateTimeOffset? DueDate { get; init; }
}
