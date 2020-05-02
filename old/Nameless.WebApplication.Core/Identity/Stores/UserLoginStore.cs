using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dasync.Collections;
using Microsoft.AspNetCore.Identity;
using Nameless.Data;
using Nameless.ObjectMapper;

namespace Nameless.WebApplication.Identity {
    public partial class UserStore : IUserLoginStore<User> {
        #region IUserLoginStore<User> Members

        public Task AddLoginAsync (User user, UserLoginInfo login, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNull (login, nameof (login));

            var sql = GetSQLScript (nameof (UserLogin), nameof (AddLoginAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (UserLogin.UserID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (UserLogin.LoginProvider), login.LoginProvider),
                Parameter.CreateInputParameter (nameof (UserLogin.ProviderKey), login.ProviderKey),
                Parameter.CreateInputParameter (nameof (UserLogin.ProviderDisplayName), login.ProviderDisplayName)
            });
        }

        public Task<User> FindByLoginAsync (string loginProvider, string providerKey, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (loginProvider, nameof (loginProvider));
            Prevent.ParameterNullOrWhiteSpace (providerKey, nameof (providerKey));

            var sql = GetSQLScript (nameof (UserLogin), nameof (FindByLoginAsync));
            return Database.ExecuteReaderSingleAsync (sql, Mapper.Map<User, IDataRecord>, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (UserLogin.LoginProvider), loginProvider),
                Parameter.CreateInputParameter (nameof (UserLogin.ProviderKey), providerKey)
            });
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (UserLogin), nameof (GetLoginsAsync));
            return Database.ExecuteReaderAsync (sql, Mapper.Map<UserLogin, IDataRecord>, parameters: new[] {
                Parameter.CreateInputParameter (nameof (UserLogin.UserID), Guid.Parse (user.ID), DbType.Guid)
            })
            .ToArrayAsync (cancellationToken)
            .ContinueWith (continuation => {
                IList<UserLoginInfo> result = null;
                if (continuation.CanContinue ()) {
                    result = continuation.Result.Select (UserLogin.Parse).ToList ();
                }
                return result;
            });
        }

        public Task RemoveLoginAsync (User user, string loginProvider, string providerKey, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (loginProvider, nameof (loginProvider));
            Prevent.ParameterNullOrWhiteSpace (providerKey, nameof (providerKey));

            var sql = GetSQLScript (nameof (UserLogin), nameof (RemoveLoginAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (UserLogin.UserID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (UserLoginInfo.LoginProvider), loginProvider),
                Parameter.CreateInputParameter (nameof (UserLoginInfo.ProviderKey), providerKey)
            });
        }

        #endregion
    }
}