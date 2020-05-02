using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.Data;
using Nameless.ObjectMapper;

namespace Nameless.AspNetCore.Identity {
    public partial class UserStore : IUserEmailStore<User> {
        #region IUserEmailStore<User> Members

        public Task<User> FindByEmailAsync (string normalizedEmail, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (normalizedEmail, nameof (normalizedEmail));

            cancellationToken.ThrowIfCancellationRequested ();
            var sql = GetSQLScript (nameof (User), nameof (FindByEmailAsync));
            return Database.ExecuteReaderSingleAsync (sql, Mapper.Map<User, IDataRecord>, token : cancellationToken, parameters : new [] {
                Parameter.CreateInputParameter (nameof (User.NormalizedEmail), normalizedEmail)
            });
        }

        public Task<string> GetEmailAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (user.NormalizedEmail);
        }

        public Task SetEmailAsync (User user, string email, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync (User user, bool confirmed, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync (User user, string normalizedEmail, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        #endregion
    }
}