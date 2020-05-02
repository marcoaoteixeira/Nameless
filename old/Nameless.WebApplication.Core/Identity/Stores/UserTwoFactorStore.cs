using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.Data;

namespace Nameless.WebApplication.Identity {
    public partial class UserStore : IUserTwoFactorStore<User> {
        #region IUserTwoFactorStore<User> Members

        public Task<bool> GetTwoFactorEnabledAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (GetTwoFactorEnabledAsync));
            return Database.ExecuteScalarAsync<bool> (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid)
            });
        }

        public Task SetTwoFactorEnabledAsync (User user, bool enabled, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (SetTwoFactorEnabledAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (User.TwoFactorEnabled), enabled, DbType.Boolean)
            })
            .ContinueWith (continuation => {
                if (continuation.CanContinue ()) { user.TwoFactorEnabled = enabled; }
            });
        }

        #endregion
    }
}