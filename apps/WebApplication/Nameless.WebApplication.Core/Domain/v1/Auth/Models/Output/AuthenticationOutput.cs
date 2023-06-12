using Newtonsoft.Json;

namespace Nameless.WebApplication.Domain.v1.Auth.Models.Output {

    public sealed class AuthenticationOutput {

        #region Public Properties

        [JsonProperty("user_id")]
        public string UserId { get; set; } = default!;
        
        [JsonProperty("username")]
        public string UserName { get; set; } = default!;

        [JsonProperty("email")]
        public string Email { get; set; } = default!;

        [JsonProperty("claims")]
        public Dictionary<string, string[]> Claims { get; set; } = new();

        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = default!;
        
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = default!;

        #endregion
    }
}
