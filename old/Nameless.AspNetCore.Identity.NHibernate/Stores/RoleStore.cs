using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NHibernate;
using NHibernate.Criterion;

namespace Nameless.AspNetCore.Identity.Stores {
    public partial class RoleStore : StoreBase, IRoleStore<Role> {

        #region Public Constructors

        public RoleStore (ISession session) : base (session) { }

        #endregion

        #region IRoleStore<Role> Members

        public Task<IdentityResult> CreateAsync (Role role, CancellationToken cancellationToken) {
            Prevent.ParameterNull (role, nameof (role));

            role.ID = Guid.NewGuid ().ToString ();

            return Session.SaveAsync (role, cancellationToken).ContinueWith (IdentityResultContinuation);
        }

        public Task<IdentityResult> DeleteAsync (Role role, CancellationToken cancellationToken) {
            Prevent.ParameterNull (role, nameof (role));

            return Session.DeleteAsync (role, cancellationToken).ContinueWith (IdentityResultContinuation);
        }

        public void Dispose () {
            /* Nothing to dispose of */
        }

        public Task<Role> FindByIdAsync (string roleId, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (roleId, nameof (roleId));

            var criteria = Session.CreateCriteria<Role> ();

            criteria
                .Add (Restrictions.Eq (nameof (Role.ID), roleId));

            return criteria.UniqueResultAsync<Role> (cancellationToken);
        }

        public Task<Role> FindByNameAsync (string normalizedRoleName, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (normalizedRoleName, nameof (normalizedRoleName));

            var criteria = Session.CreateCriteria<Role> ();

            criteria
                .Add (Restrictions.Eq (nameof (Role.NormalizedName), normalizedRoleName));

            return criteria.UniqueResultAsync<Role> (cancellationToken);
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
            Prevent.ParameterNull (role, nameof (role));

            return Session.UpdateAsync (role, cancellationToken).ContinueWith (IdentityResultContinuation);
        }

        #endregion
    }
}