using System.Text.Json.Serialization;

namespace Nameless.DailyDos.Web.Api.v1.Models.Input {
    public sealed record ListTodoItemInput {
        #region Public Properties

        [JsonPropertyName("description_like")]
        public string? DescriptionLike { get; set; }
        
        [JsonPropertyName("finished_before")]
        public DateTime? FinishedBefore { get; set; }

        #endregion
    }
}
