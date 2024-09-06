using System.Text.Json.Serialization;

namespace Nameless.Checklist.Web.Api.v1.Models.Output;

public sealed record ChecklistItemOutput {
    #region Public Properties

    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; } = null!;

    [JsonPropertyName("checked_at")]
    public DateTime? CheckedAt { get; init; }

    [JsonPropertyName("checked")]
    public bool Checked { get; init; }

    #endregion
}