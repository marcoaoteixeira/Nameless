using System.Text.Json.Serialization;

namespace Nameless.Microservice.Web.Api.v1.Models {
    public sealed record CreateTodoInput {
        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;
    }
}
