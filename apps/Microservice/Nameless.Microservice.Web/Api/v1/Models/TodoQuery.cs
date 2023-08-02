using System.Text.Json.Serialization;

namespace Nameless.Microservice.Web.Api.v1.Models {
    public sealed record TodoQuery {
        #region Public Properties

        [JsonPropertyName("description_like")]
        public string? DescriptionLike { get; set; }
        [JsonPropertyName("finished_before")]
        public DateTime? FinishedBefore { get; set; }

        #endregion
    }
}
