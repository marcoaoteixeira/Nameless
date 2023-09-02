using System.Text.Json.Serialization;

namespace Nameless.Checklist.Web.Api.v1.Models.Input {
    public sealed record CreateChecklistItemInput {
        #region Public Properties

        [JsonPropertyName("description")]
        public string Description { get; init; } = null!;

        #endregion
    }
}
