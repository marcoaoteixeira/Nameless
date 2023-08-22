using System.Text.Json.Serialization;

namespace Nameless.Microservices.Api.v1.Models {
    public sealed record UpdateTodoInput {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        [JsonPropertyName("finised_at")]
        public DateTime? FinishedAt { get; set; }
    }
}
