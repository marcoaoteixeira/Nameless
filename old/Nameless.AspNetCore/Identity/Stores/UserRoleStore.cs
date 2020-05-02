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

namespace Nameless.AspNetCore.Identity {
    public partial class UserStore : IUserRoleStore<User> {
        #region IUserRoleStore<User> Members

        public Task AddToRoleAsync (User user, string roleName, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (roleName, nameof (roleName));

            var sql = GetSQLScript (nameof (User), nameof (AddToRoleAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter ("UserID", Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter ("RoleName", roleName)
            });
        }

        public Task<IList<string>> GetRolesAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (GetRolesAsync));
            return Database.ExecuteReaderAsync (sql, reader => reader.GetStringOrDefault (nameof (Role.Name)), parameters: new[] {
                Parameter.CreateInputParameter ("UserID", Guid.Parse (user.ID), DbType.Guid)
            })
            .ToArrayAsync (cancellationToken)
            .ContinueWith (continuation => {
                IList<string> result = null;
                if (continuation.CanContinue ()) {
                    result = continuation.Result.ToList ();
                }
                return result;
            });
        }

        public Task<IList<User>> GetUsersInRoleAsync (string roleName, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (roleName, nameof (roleName));

            var sql = GetSQLScript (nameof (User), nameof (GetUsersInRoleAsync));
            return Database.ExecuteReaderAsync (sql, Mapper.Map<User, IDataRecord>, parameters: new[] {
                Parameter.CreateInputParameter ("RoleName", roleName)
            })
            .ToArrayAsync (cancellationToken)
            .ContinueWith (continuation => {
                IList<User> result = null;
                if (continuation.CanContinue ()) {
                    result = continuation.Result.ToList ();
                }
                return result;
            });
        }

        public Task<bool> IsInRoleAsync (User user, string roleName, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (roleName, nameof (roleName));

            var sql = GetSQLScript (nameof (User), nameof (IsInRoleAsync));
            return Database.ExecuteScalarAsync<bool> (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter ("UserID", Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter ("RoleName", roleName)
            });
        }

        public Task RemoveFromRoleAsync (User user, string roleName, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (roleName, nameof (roleName));

            var sql = GetSQLScript (nameof (User), nameof (RemoveFromRoleAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter ("UserID", Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter ("RoleName", roleName)
            });
        }

        #endregion
    }
}