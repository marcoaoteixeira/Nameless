using System.Text.Json.Serialization;

namespace Nameless.Checklist.Web.Api.v1.Models.Input {
    public sealed record ListChecklistItemsInput {
        #region Public Properties

        [JsonPropertyName("description_like")]
        public string? DescriptionLike { get; init; }

        [JsonPropertyName("checked_before")]
        public DateTime? CheckedBefore { get; init; }

        #endregion
    }
}
