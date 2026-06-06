using System.Text.Json.Serialization;

namespace Nameless.Microservices.Contracts.DTOs;

public record ChoreDto {
    [JsonPropertyName("id")]
    public Guid ID { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }
    
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("dueDate")]
    public DateTime? DueDate { get; init; }

    [JsonPropertyName("conclusionDate")]
    public DateTime? ConclusionDate { get; init; }
}
