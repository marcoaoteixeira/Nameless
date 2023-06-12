namespace Nameless.WebApplication.Domain.v1.Auth.Models.Input {

    public sealed class AuthenticationInput {

        #region Public Properties

        public string? Email { get; set; }
        public string? Password { get; set; }

        #endregion
    }
}
