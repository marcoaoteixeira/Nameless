using System.Text.Json.Serialization;

namespace Nameless.Checklist.Web.Api.v2.Models.Output;

public sealed record ChecklistItemOutput {
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("item_description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("checked_at")]
    public DateTime? CheckedAt { get; init; }

    [JsonPropertyName("checked")]
    public bool Checked { get; init; }
}