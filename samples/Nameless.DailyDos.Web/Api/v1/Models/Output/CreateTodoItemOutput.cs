using System.Text.Json.Serialization;

namespace Nameless.DailyDos.Web.Api.v1.Models.Output {
    public sealed record CreateTodoItemOutput {
        #region Public Properties

        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;
        
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("finished_at")]
        public DateTime? FinishedAt { get; set; }

        #endregion
    }
}
