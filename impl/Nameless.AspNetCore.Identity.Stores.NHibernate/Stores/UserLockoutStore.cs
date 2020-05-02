using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Nameless.AspNetCore.Identity.Stores {
    public partial class UserStore : IUserLockoutStore<User> {
        #region IUserLockoutStore<User> Members

        public Task<int> GetAccessFailedCountAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (user.LockoutEnabled);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (user.LockoutEnd);
        }

        public Task<int> IncrementAccessFailedCountAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            user.AccessFailedCount++;
            return Task.FromResult (user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync (User user, bool enabled, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync (User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        #endregion
    }
}