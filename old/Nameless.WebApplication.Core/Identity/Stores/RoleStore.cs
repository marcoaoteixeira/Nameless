using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.Data;
using Nameless.FileProvider;
using Nameless.ObjectMapper;

namespace Nameless.WebApplication.Identity {
    public partial class RoleStore : StoreBase, IRoleStore<Role> {

        #region Public Constructors

        public RoleStore (IDatabase database, IFileProvider fileProvider, IMapper mapper) : base (database, fileProvider, mapper) { }

        #endregion

        #region Private Methods

        private Task<IdentityResult> SaveAsync (string sql, Role role, CancellationToken cancellationToken = default) {
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (Role.ID), Guid.Parse (role.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (Role.Name), role.Name),
                Parameter.CreateInputParameter (nameof (Role.NormalizedName), role.NormalizedName)
            }).ContinueWith (IdentityResultContinuation);
        }

        #endregion

        #region IRoleStore<Role> Members

        public Task<IdentityResult> CreateAsync (Role role, CancellationToken cancellationToken) {
            var sql = GetSQLScript (nameof (Role), nameof (CreateAsync));
            role.ID = Guid.NewGuid ().ToString ();
            return SaveAsync (sql, role, cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync (Role role, CancellationToken cancellationToken) {
            Prevent.ParameterNull (role, nameof (role));

            var sql = GetSQLScript (nameof (Role), nameof (DeleteAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (Role.ID), Guid.Parse (role.ID), DbType.Guid)
            }).ContinueWith (IdentityResultContinuation);
        }

        public void Dispose () {
            /* Nothing to dispose of */
        }

        public Task<Role> FindByIdAsync (string roleId, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (roleId, nameof (roleId));

            var sql = GetSQLScript (nameof (Role), nameof (FindByIdAsync));
            return Database.ExecuteReaderSingleAsync (sql, Mapper.Map<Role, IDataRecord>, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (Role.ID), Guid.Parse (roleId), DbType.Guid)
            });
        }

        public Task<Role> FindByNameAsync (string normalizedRoleName, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (normalizedRoleName, nameof (normalizedRoleName));

            var sql = GetSQLScript (nameof (Role), nameof (FindByNameAsync));
            return Database.ExecuteReaderSingleAsync (sql, Mapper.Map<Role, IDataRecord>, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (Role.NormalizedName), normalizedRoleName)
            });
        }

        public Task<string> GetNormalizedRoleNameAsync (Role role, CancellationToken cancellationToken) {
            Prevent.ParameterNull (role, nameof (role));

            var sql = GetSQLScript (nameof (Role), nameof (GetNormalizedRoleNameAsync));
            return Database.ExecuteScalarAsync<string> (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (Role.ID), Guid.Parse (role.ID), DbType.Guid)
            });
        }

        public Task<string> GetRoleIdAsync (Role role, CancellationToken cancellationToken) {
            Prevent.ParameterNull (role, nameof (role));

            var sql = GetSQLScript (nameof (Role), nameof (GetRoleIdAsync));
            return Database.ExecuteScalarAsync<Guid> (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (Role.NormalizedName), role.NormalizedName)
            }).ContinueWith (continuation => continuation.CanContinue () ? continuation.Result.ToString () : null);
        }

        public Task<string> GetRoleNameAsync (Role role, CancellationToken cancellationToken) {
            Prevent.ParameterNull (role, nameof (role));

            var sql = GetSQLScript (nameof (Role), nameof (GetRoleNameAsync));
            return Database.ExecuteScalarAsync<string> (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (Role.ID), Guid.Parse (role.ID), DbType.Guid)
            });
        }

        public Task SetNormalizedRoleNameAsync (Role role, string normalizedName, CancellationToken cancellationToken) {
            Prevent.ParameterNull (role, nameof (role));
            Prevent.ParameterNullOrWhiteSpace (normalizedName, nameof (normalizedName));

            var sql = GetSQLScript (nameof (Role), nameof (SetNormalizedRoleNameAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (Role.ID), Guid.Parse (role.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (Role.NormalizedName), normalizedName)
            }).ContinueWith (continuation => {
                if (continuation.CanContinue ()) {
                    if (role != null) { role.NormalizedName = normalizedName; }
                }
            });
        }

        public Task SetRoleNameAsync (Role role, string roleName, CancellationToken cancellationToken) {
            Prevent.ParameterNull (role, nameof (role));
            Prevent.ParameterNullOrWhiteSpace (roleName, nameof (roleName));

            var sql = GetSQLScript (nameof (Role), nameof (SetRoleNameAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (Role.ID), Guid.Parse (role.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (Role.Name), roleName)
            }).ContinueWith (continuation => {
                if (continuation.CanContinue ()) {
                    if (role != null) { role.Name = roleName; }
                }
            });
        }

        public Task<IdentityResult> UpdateAsync (Role role, CancellationToken cancellationToken) {
            var sql = GetSQLScript (nameof (Role), nameof (UpdateAsync));
            return SaveAsync (sql, role, cancellationToken);
        }

        #endregion
    }
}