using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.Data;

namespace Nameless.WebApplication.Identity {
    public partial class UserStore : IUserSecurityStampStore<User> {
        #region IUserSecurityStampStore<User> Members

        public Task<string> GetSecurityStampAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (GetSecurityStampAsync));
            return Database.ExecuteScalarAsync<string> (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid)
            });
        }

        public Task SetSecurityStampAsync (User user, string stamp, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (SetSecurityStampAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (User.SecurityStamp), stamp)
            })
            .ContinueWith(continuation => {
                if (continuation.CanContinue ()) { user.SecurityStamp = stamp; }
            });
        }

        #endregion
    }
}