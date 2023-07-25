using System.Text.Json.Serialization;

namespace Nameless.Microservice.Web.Api.v1.Models {
    public sealed record WorkingOutput {
        #region Public Properties

        [JsonPropertyName("message")]
        public string? Message { get; init; }

        #endregion
    }
}
