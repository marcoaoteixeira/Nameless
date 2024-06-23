using System.Text.Json.Serialization;

namespace Nameless.Web.Identity.Api.Outputs {
    public sealed record AuthenticateUserOutput {
        #region Public Properties

        [JsonPropertyName("token")]
        public string? Token { get; init; }

        [JsonPropertyName("error")]
        public string? Error { get; init; }

        [JsonPropertyName("succeeded")]
        public bool Succeeded { get; init; }
    }

    #endregion
}
