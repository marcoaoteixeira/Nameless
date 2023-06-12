using Newtonsoft.Json;

namespace Nameless.WebApplication.Domain.v1.Token.Models.Output {

    public sealed class TokenOutput {
        #region Public Properties

        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = default!;
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = default!;

        #endregion
    }
}
