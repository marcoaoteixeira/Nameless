using System.Text.Json.Serialization;

namespace Nameless.Web.Identity.Api.Inputs {
    public sealed record GetUserInput {
        #region Public Properties

        [JsonPropertyName("user_id")]
        public string? UserId { get; init; }

        [JsonPropertyName("username")]
        public string? Username { get; init; }

        [JsonPropertyName("email")]
        public string? Email { get; init; }

        #endregion
    }
}
