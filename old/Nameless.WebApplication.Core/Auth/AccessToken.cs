namespace Nameless.WebApplication.Auth {
    public sealed class AccessToken {
        #region Public Properties

        public string Token { get; }
        public int ExpiresIn { get; }

        #endregion

        #region Public Constructors

        public AccessToken (string token, int expiresIn) {
            Token = token;
            ExpiresIn = expiresIn;
        }

        #endregion
    }
}