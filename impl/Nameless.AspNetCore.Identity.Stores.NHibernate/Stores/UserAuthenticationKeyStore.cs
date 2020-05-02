using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Nameless.AspNetCore.Identity.Stores {
    public partial class UserStore : IUserAuthenticatorKeyStore<User> {
        #region Private Constants

        private const string AUTHENTICATOR_KEY_TOKEN_NAME = "NAMELESS_WEBAPPLICATION_AUTHENTICATOR_KEY";

        #endregion

        #region IUserAuthenticatorKeyStore<User> Members

        public Task<string> GetAuthenticatorKeyAsync (User user, CancellationToken cancellationToken) {
            return GetTokenAsync (user, INTERNAL_LOGIN_PROVIDER, AUTHENTICATOR_KEY_TOKEN_NAME, cancellationToken);
        }

        public Task SetAuthenticatorKeyAsync (User user, string key, CancellationToken cancellationToken) {
            return SetTokenAsync (user, INTERNAL_LOGIN_PROVIDER, AUTHENTICATOR_KEY_TOKEN_NAME, key, cancellationToken);
        }

        #endregion
    }
}