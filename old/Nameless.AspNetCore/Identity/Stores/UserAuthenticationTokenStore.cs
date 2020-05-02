using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.Data;

namespace Nameless.AspNetCore.Identity {
    public partial class UserStore : IUserAuthenticationTokenStore<User> {
        #region IUserAuthenticationTokenStore<User> Members

        public Task<string> GetTokenAsync (User user, string loginProvider, string name, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (loginProvider, nameof (loginProvider));
            Prevent.ParameterNullOrWhiteSpace (name, nameof (name));

            var sql = GetSQLScript (nameof (UserToken), nameof (GetTokenAsync));
            return Database.ExecuteReaderSingleAsync (sql, reader => reader.GetStringOrDefault (nameof (UserToken.Value)), token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (UserToken.UserID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (UserToken.LoginProvider), loginProvider),
                Parameter.CreateInputParameter (nameof (UserToken.Name), name)
            });
        }

        public Task RemoveTokenAsync (User user, string loginProvider, string name, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (loginProvider, nameof (loginProvider));
            Prevent.ParameterNullOrWhiteSpace (name, nameof (name));

            var sql = GetSQLScript (nameof (UserToken), nameof (RemoveTokenAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (UserToken.UserID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (UserToken.LoginProvider), loginProvider),
                Parameter.CreateInputParameter (nameof (UserToken.Name), name)
            });
        }

        public Task SetTokenAsync (User user, string loginProvider, string name, string value, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (loginProvider, nameof (loginProvider));
            Prevent.ParameterNullOrWhiteSpace (name, nameof (name));
            Prevent.ParameterNullOrWhiteSpace (value, nameof (value));

            var sql = GetSQLScript (nameof (UserToken), nameof (SetTokenAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (UserToken.UserID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (UserToken.LoginProvider), loginProvider),
                Parameter.CreateInputParameter (nameof (UserToken.Name), name),
                Parameter.CreateInputParameter (nameof (UserToken.Value), value)
            });
        }

        #endregion
    }
}