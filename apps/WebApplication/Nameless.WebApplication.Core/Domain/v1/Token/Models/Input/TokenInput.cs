using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Nameless.WebApplication.Domain.v1.Token.Models.Input {

    public sealed class TokenInput {

        #region Public Properties

        [Required]
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = default!;

        [Required]
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = default!;

        #endregion
    }
}
