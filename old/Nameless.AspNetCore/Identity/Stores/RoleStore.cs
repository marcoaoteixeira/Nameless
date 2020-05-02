using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Nameless.Data;
using Nameless.ObjectMapper;

namespace Nameless.AspNetCore.Identity {
    public partial class RoleStore : StoreBase, IRoleStore<Role> {

        #region Public Constructors

        public RoleStore (IDatabase database, IFileProvider fileProvider, IMapper mapper) : base (database, fileProvider, mapper) { }

        #endregion

        #region Private Methods

        private Task<IdentityResult> SaveAsync (string sql, Role role, CancellationToken cancellationToken = default) {
            return Database.ExecuteNonQueryAsync (sql, token : cancellationToken, parameters : new [] {
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
            return Database.ExecuteNonQueryAsync (sql, token : cancellationToken, parameters : new [] {
                Parameter.CreateInputParameter (nameof (Role.ID), Guid.Parse (role.ID), DbType.Guid)
            }).ContinueWith (IdentityResultContinuation);
        }

        public void Dispose () {
            /* Nothing to dispose of */
        }

        public Task<Role> FindByIdAsync (string roleId, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (roleId, nameof (roleId));

            var sql = GetSQLScript (nameof (Role), nameof (FindByIdAsync));
            return Database.ExecuteReaderSingleAsync (sql, Mapper.Map<Role, IDataRecord>, token : cancellationToken, parameters : new [] {
                Parameter.CreateInputParameter (nameof (Role.ID), Guid.Parse (roleId), DbType.Guid)
            });
        }

        public Task<Role> FindByNameAsync (string normalizedRoleName, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (normalizedRoleName, nameof (normalizedRoleName));

            var sql = GetSQLScript (nameof (Role), nameof (FindByNameAsync));
            return Database.ExecuteReaderSingleAsync (sql, Mapper.Map<Role, IDataRecord>, token : cancellationToken, parameters : new [] {
                Parameter.CreateInputParameter (nameof (Role.NormalizedName), normalizedRoleName)
            });
        }

        public Task<string> GetNormalizedRoleNameAsync (Role role, CancellationToken cancellationToken) {
            Prevent.ParameterNull (role, nameof (role));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync (Role role, CancellationToken cancellationToken) {
            Prevent.ParameterNull (role, nameof (role));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (role.ID);
        }

        public Task<string> GetRoleNameAsync (Role role, CancellationToken cancellationToken) {
            Prevent.ParameterNull (role, nameof (role));

            cancellationToken.ThrowIfCancellationRequested ();
            return Task.FromResult (role.Name);
        }

        public Task SetNormalizedRoleNameAsync (Role role, string normalizedName, CancellationToken cancellationToken) {
            Prevent.ParameterNull (role, nameof (role));
            Prevent.ParameterNullOrWhiteSpace (normalizedName, nameof (normalizedName));

            cancellationToken.ThrowIfCancellationRequested ();
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync (Role role, string roleName, CancellationToken cancellationToken) {
            Prevent.ParameterNull (role, nameof (role));
            Prevent.ParameterNullOrWhiteSpace (roleName, nameof (roleName));

            cancellationToken.ThrowIfCancellationRequested ();
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync (Role role, CancellationToken cancellationToken) {
            var sql = GetSQLScript (nameof (Role), nameof (UpdateAsync));
            return SaveAsync (sql, role, cancellationToken);
        }

        #endregion
    }
}