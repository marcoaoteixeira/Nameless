using System.Text.Json.Serialization;

namespace Nameless.Microservices.Api.v1.Models {
    public sealed record CreateTodoInput {
        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;
    }
}
