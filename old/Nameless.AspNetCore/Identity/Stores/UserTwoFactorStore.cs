using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Nameless.AspNetCore.Identity {
    public partial class UserStore : IUserTwoFactorStore<User> {
        #region IUserTwoFactorStore<User> Members

        public Task<bool> GetTwoFactorEnabledAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (user.TwoFactorEnabled);
        }

        public Task SetTwoFactorEnabledAsync (User user, bool enabled, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        #endregion
    }
}