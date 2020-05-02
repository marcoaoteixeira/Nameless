using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Nameless.AspNetCore.Identity.Stores {
    public partial class UserStore : IUserPhoneNumberStore<User> {
        #region IUserPhoneNumberStore<User> Members

        public Task<string> GetPhoneNumberAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberAsync (User user, string phoneNumber, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync (User user, bool confirmed, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        #endregion
    }
}