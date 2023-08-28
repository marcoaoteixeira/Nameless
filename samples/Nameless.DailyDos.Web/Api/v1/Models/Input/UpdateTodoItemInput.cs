using System.Text.Json.Serialization;

namespace Nameless.DailyDos.Web.Api.v1.Models.Input {
    public sealed record UpdateTodoItemInput {
        #region Public Properties

        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        [JsonPropertyName("finished_at")]
        public DateTime? FinishedAt { get; set; }

        #endregion
    }
}
