using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Nameless.AspNetCore.Identity.Stores {
    public partial class UserStore : IUserTwoFactorRecoveryCodeStore<User> {
        #region Public Constants

        public const string INTERNAL_LOGIN_PROVIDER = "NAMELESS_LOGINPROVIDER";
        public const string RECOVERY_CODE_TOKEN_NAME = "NAMELESS_RECOVERYCODES";

        #endregion

        #region IUserTwoFactorRecoveryCodeStore<User> Members

        public async Task<int> CountCodesAsync (User user, CancellationToken cancellationToken)  {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();

            var codes = await GetTokenAsync (user, INTERNAL_LOGIN_PROVIDER, RECOVERY_CODE_TOKEN_NAME, cancellationToken);

            return (!string.IsNullOrWhiteSpace (codes)) ? codes.Split(';').Length : 0;
        }

        public async Task<bool> RedeemCodeAsync (User user, string code, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();

            var codes = await GetTokenAsync (user, INTERNAL_LOGIN_PROVIDER, RECOVERY_CODE_TOKEN_NAME, cancellationToken) ?? string.Empty;
            var split = codes.Split (';').ToList ();
            if (split.Contains (code)) {
                split.Remove (code);
                await ReplaceCodesAsync (user, split, cancellationToken);
                return true;
            }
            return false;
        }

        public Task ReplaceCodesAsync (User user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken) {
            var codes = string.Join (";", recoveryCodes ?? Enumerable.Empty<string> ());
            return SetTokenAsync (user, INTERNAL_LOGIN_PROVIDER, RECOVERY_CODE_TOKEN_NAME, codes, cancellationToken);
        }

        #endregion
    }
}