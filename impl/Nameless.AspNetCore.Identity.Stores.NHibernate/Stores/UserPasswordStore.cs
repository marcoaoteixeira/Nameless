using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Nameless.AspNetCore.Identity.Stores {
    public partial class UserStore : IUserPasswordStore<User> {
        #region IUserPasswordStore<User> Members

        public Task<string> GetPasswordHashAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (!string.IsNullOrWhiteSpace (user.PasswordHash));
        }

        public Task SetPasswordHashAsync (User user, string passwordHash, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        #endregion
    }
}