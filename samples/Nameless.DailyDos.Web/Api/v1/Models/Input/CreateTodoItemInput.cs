using System.Text.Json.Serialization;

namespace Nameless.DailyDos.Web.Api.v1.Models.Input {
    public sealed record CreateTodoItemInput {
        #region Public Properties

        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        #endregion
    }
}
