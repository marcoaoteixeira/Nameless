using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Nameless.AspNetCore.Identity.Stores {
    public partial class UserStore : IUserSecurityStampStore<User> {
        #region IUserSecurityStampStore<User> Members

        public Task<string> GetSecurityStampAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (user.SecurityStamp);
        }

        public Task SetSecurityStampAsync (User user, string stamp, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        #endregion
    }
}