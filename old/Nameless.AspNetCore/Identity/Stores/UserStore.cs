using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Nameless.Data;
using Nameless.ObjectMapper;

namespace Nameless.AspNetCore.Identity {
    public partial class UserStore : StoreBase, IUserStore<User> {
        #region Public Constructors

        public UserStore (IDatabase database, IFileProvider fileProvider, IMapper mapper)
            : base (database, fileProvider, mapper) { }

        #endregion

        #region Private Methods

        private Task<IdentityResult> SaveAsync (string sql, User user, CancellationToken cancellationToken) {
            return Database.ExecuteNonQueryAsync (sql, token : cancellationToken, parameters : new [] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (User.Email), user.Email),
                Parameter.CreateInputParameter (nameof (User.EmailConfirmed), user.EmailConfirmed, DbType.Boolean),
                Parameter.CreateInputParameter (nameof (User.NormalizedEmail), user.NormalizedEmail),
                Parameter.CreateInputParameter (nameof (User.UserName), user.UserName),
                Parameter.CreateInputParameter (nameof (User.NormalizedUserName), user.NormalizedUserName),
                Parameter.CreateInputParameter (nameof (User.PhoneNumber), user.PhoneNumber),
                Parameter.CreateInputParameter (nameof (User.PhoneNumberConfirmed), user.PhoneNumberConfirmed, DbType.Boolean),
                Parameter.CreateInputParameter (nameof (User.LockoutEnabled), user.LockoutEnabled, DbType.Boolean),
                Parameter.CreateInputParameter (nameof (User.LockoutEnd), user.LockoutEnd, DbType.DateTimeOffset),
                Parameter.CreateInputParameter (nameof (User.AccessFailedCount), user.AccessFailedCount, DbType.Int32),
                Parameter.CreateInputParameter (nameof (User.PasswordHash), user.PasswordHash),
                Parameter.CreateInputParameter (nameof (User.SecurityStamp), user.SecurityStamp),
                Parameter.CreateInputParameter (nameof (User.TwoFactorEnabled), user.TwoFactorEnabled, DbType.Boolean),
                Parameter.CreateInputParameter (nameof (User.AvatarUrl), user.AvatarUrl),
            })
            .ContinueWith (IdentityResultContinuation);
        }

        #endregion

        #region IUserStore<User> Members

        public Task<string> GetUserIdAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (user.ID);
        }

        public Task<string> GetUserNameAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (user.UserName);
        }

        public Task SetUserNameAsync (User user, string userName, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (userName, nameof (userName));

            cancellationToken.ThrowIfCancellationRequested ();
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync (User user, string normalizedName, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (normalizedName, nameof (normalizedName));

            cancellationToken.ThrowIfCancellationRequested ();
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            var sql = GetSQLScript (nameof (User), nameof (CreateAsync));
            user.ID = Guid.NewGuid ().ToString ();
            return SaveAsync (sql, user, cancellationToken);
        }

        public Task<IdentityResult> UpdateAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (UpdateAsync));
            return SaveAsync (sql, user, cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (User), nameof (DeleteAsync));
            return Database.ExecuteNonQueryAsync (sql, token : cancellationToken, parameters : new [] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (user.ID), DbType.Guid)
            }).ContinueWith (IdentityResultContinuation);
        }

        public Task<User> FindByIdAsync (string userId, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (userId, nameof (userId));

            var sql = GetSQLScript (nameof (User), nameof (FindByIdAsync));
            return Database.ExecuteReaderSingleAsync (sql, Mapper.Map<User, IDataRecord>, token : cancellationToken, parameters : new [] {
                Parameter.CreateInputParameter (nameof (User.ID), Guid.Parse (userId), DbType.Guid)
            });
        }

        public Task<User> FindByNameAsync (string normalizedUserName, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (normalizedUserName, nameof (normalizedUserName));

            var sql = GetSQLScript (nameof (User), nameof (FindByNameAsync));
            return Database.ExecuteReaderSingleAsync (sql, Mapper.Map<User, IDataRecord>, token : cancellationToken, parameters : new [] {
                Parameter.CreateInputParameter (nameof (User.NormalizedUserName), normalizedUserName)
            });
        }

        public void Dispose () {
            /* Nothing to dispose of */
        }

        #endregion
    }
}