using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Nameless.Microservices.Common.Chores.Outputs;

public sealed record CreateChoreOutput {
    [Description("ID of the created chore")]
    [JsonPropertyName("id")]
    public required Guid ID { get; init; }
}
