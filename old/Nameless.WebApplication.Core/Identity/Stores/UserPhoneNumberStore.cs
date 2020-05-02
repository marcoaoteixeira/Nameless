using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.Data;

namespace Nameless.WebApplication.Identity {
    public partial class UserStore : IUserPhoneNumberStore<User> {
        #region IUserPhoneNumberStore<User> Members

        public Task<string> GetPhoneNumberAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (GetPhoneNumberAsync));
            return Database.ExecuteScalarAsync<string> (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid)
            });
        }

        public Task<bool> GetPhoneNumberConfirmedAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (GetPhoneNumberConfirmedAsync));
            return Database.ExecuteScalarAsync<bool> (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid)
            });
        }

        public Task SetPhoneNumberAsync (User user, string phoneNumber, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (SetPhoneNumberAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (User.PhoneNumber), phoneNumber)
            })
            .ContinueWith (continuation => {
                if (continuation.CanContinue ()) { user.PhoneNumber = phoneNumber; }
            });
        }

        public Task SetPhoneNumberConfirmedAsync (User user, bool confirmed, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (SetPhoneNumberConfirmedAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (User.PhoneNumberConfirmed), confirmed)
            })
            .ContinueWith (continuation => {
                if (continuation.CanContinue ()) { user.PhoneNumberConfirmed = confirmed; }
            });
        }

        #endregion
    }
}