using System.Text.Json.Serialization;

namespace Nameless.Checklist.Web.Api.v1.Models.Input {
    public sealed record UpdateChecklistItemInput {
        #region Public Properties

        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("description")]
        public string Description { get; init; } = null!;

        [JsonPropertyName("checked_at")]
        public DateTime? CheckedAt { get; init; }

        #endregion
    }
}
