using System.Text.Json.Serialization;

namespace Nameless.Checklist.Web.Api.v2.Models.Input;

public sealed record ListChecklistItemsInput {
    [JsonPropertyName("item_description_like")]
    public string? DescriptionLike { get; init; }

    [JsonPropertyName("checked_before")]
    public DateTime? CheckedBefore { get; init; }
}