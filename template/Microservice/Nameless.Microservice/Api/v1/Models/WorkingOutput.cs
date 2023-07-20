using System.Text.Json.Serialization;

namespace Nameless.Microservice.Api.v1.Models {
    public sealed record WorkingOutput {
        #region Public Properties

        [JsonPropertyName("message")]
        public string? Message { get; init; }

        #endregion
    }
}
