using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.Data;

namespace Nameless.WebApplication.Identity {
    public partial class UserStore : IUserPasswordStore<User> {
        #region IUserPasswordStore<User> Members

        public Task<string> GetPasswordHashAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (GetPasswordHashAsync));
            return Database.ExecuteScalarAsync<string> (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid)
            });
        }

        public Task<bool> HasPasswordAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (HasPasswordAsync));
            return Database.ExecuteScalarAsync<bool> (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid)
            });
        }

        public Task SetPasswordHashAsync (User user, string passwordHash, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (SetPasswordHashAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (User.PasswordHash), passwordHash)
            })
            .ContinueWith (continuation => {
                if (continuation.CanContinue ()) { user.PasswordHash = passwordHash; }
            });
        }

        #endregion
    }
}