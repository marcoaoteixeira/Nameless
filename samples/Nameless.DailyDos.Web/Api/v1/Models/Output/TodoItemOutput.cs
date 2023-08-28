using System.Text.Json.Serialization;

namespace Nameless.DailyDos.Web.Api.v1.Models.Output {
    public sealed record TodoItemOutput {
        #region Public Properties

        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("description")]
        public string Description { get; init; } = null!;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; init; }

        [JsonPropertyName("finished_at")]
        public DateTime? FinishedAt { get; init; }

        [JsonPropertyName("concluded")]
        public bool Concluded => FinishedAt.GetValueOrDefault() != DateTime.MinValue;

        #endregion
    }
}
