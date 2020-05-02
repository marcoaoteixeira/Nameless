using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.Data;

namespace Nameless.WebApplication.Identity {
    public partial class UserStore : IUserLockoutStore<User> {
        #region IUserLockoutStore<User> Members

        public Task<int> GetAccessFailedCountAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (GetAccessFailedCountAsync));
            return Database.ExecuteScalarAsync<int> (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid)
            });
        }

        public Task<bool> GetLockoutEnabledAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (GetLockoutEnabledAsync));
            return Database.ExecuteScalarAsync<bool> (sql, token: cancellationToken, parameters : new [] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid)
            });
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (GetLockoutEndDateAsync));
            return Database.ExecuteScalarAsync<object> (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid)
            })
            .ContinueWith (continuation => {
                DateTimeOffset? result = null;
                if (continuation.CanContinue ()) {
                    var value = (string)continuation.Result;
                    if (!string.IsNullOrWhiteSpace (value)) {
                        result = DateTimeOffset.Parse ((string)continuation.Result);
                    }
                }
                return result;
            });
        }

        public Task<int> IncrementAccessFailedCountAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (IncrementAccessFailedCountAsync));
            return Database.ExecuteScalarAsync<int> (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid)
            })
            .ContinueWith (continuation => {
                var result = -1;
                if (continuation.CanContinue( )) {
                    user.AccessFailedCount = continuation.Result;
                    result = continuation.Result;
                }
                return result;
            });
        }

        public Task ResetAccessFailedCountAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (ResetAccessFailedCountAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid)
            })
            .ContinueWith (continuation => {
                if (continuation.CanContinue ()) { user.AccessFailedCount = 0; }
            });
        }

        public Task SetLockoutEnabledAsync (User user, bool enabled, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (SetLockoutEnabledAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (User.LockoutEnabled), enabled, DbType.Boolean)
            })
            .ContinueWith (continuation => {
                if (continuation.CanContinue ()) { user.LockoutEnabled = enabled; }
            });
        }

        public Task SetLockoutEndDateAsync (User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (SetLockoutEndDateAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (User.LockoutEnd), lockoutEnd, DbType.DateTimeOffset)
            })
            .ContinueWith (continuation => {
                if (continuation.CanContinue ()) { user.LockoutEnd = lockoutEnd; }
            });
        }

        #endregion
    }
}