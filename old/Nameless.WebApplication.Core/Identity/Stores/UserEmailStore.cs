using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.Data;
using Nameless.ObjectMapper;

namespace Nameless.WebApplication.Identity {
    public partial class UserStore : IUserEmailStore<User> {
        #region IUserEmailStore<User> Members

        public Task<User> FindByEmailAsync (string normalizedEmail, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (normalizedEmail, nameof (normalizedEmail));

            var sql = GetSQLScript (nameof (User), nameof (FindByEmailAsync));
            return Database.ExecuteReaderSingleAsync (sql, Mapper.Map<User, IDataRecord>, token: cancellationToken, parameters : new [] {
                Parameter.CreateInputParameter (nameof (User.NormalizedEmail), normalizedEmail)
            });
        }

        public Task<string> GetEmailAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (GetEmailAsync));
            return Database.ExecuteScalarAsync<string> (sql, token: cancellationToken, parameters : new [] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid)
            });
        }

        public Task<bool> GetEmailConfirmedAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (GetEmailConfirmedAsync));
            return Database.ExecuteScalarAsync<bool> (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid)
            });
        }

        public Task<string> GetNormalizedEmailAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (GetNormalizedEmailAsync));
            return Database.ExecuteScalarAsync<string> (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid)
            });
        }

        public Task SetEmailAsync (User user, string email, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (SetEmailAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters : new [] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (User.Email), email)
            })
            .ContinueWith (continuation => {
                if (continuation.CanContinue ()) { user.Email = email; }
            });
        }

        public Task SetEmailConfirmedAsync (User user, bool confirmed, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (SetEmailConfirmedAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (User.EmailConfirmed), confirmed)
            })
            .ContinueWith (continuation => {
                if (continuation.CanContinue ()) { user.EmailConfirmed = confirmed; }
            });
        }

        public Task SetNormalizedEmailAsync (User user, string normalizedEmail, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (SetNormalizedEmailAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters : new [] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (User.NormalizedEmail), normalizedEmail)
            })
            .ContinueWith (continuation => {
                if (continuation.CanContinue ()) { user.NormalizedEmail = normalizedEmail; }
            });
        }

        #endregion
    }
}