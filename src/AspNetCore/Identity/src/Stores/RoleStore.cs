using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.Persistence;

namespace Nameless.AspNetCore.Identity {
    public sealed class RoleStore : RoleStoreBase<Role, Guid, UserRole, RoleClaim> {
        #region Private Read-Only Fields

        private readonly IRepository _repository;

        #endregion

        #region Public Constructors

        public RoleStore (IRepository repository, IdentityErrorDescriber describer) : base (describer) {
            Prevent.ParameterNull (repository, nameof (repository));

            _repository = repository;
        }

        #endregion

        #region Public Override Methods

        public override IQueryable<Role> Roles => _repository.Query<Role> ();

        public override Task AddClaimAsync (Role role, Claim claim, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (role, nameof (role));
            Prevent.ParameterNull (claim, nameof (claim));

            var roleClaim = new RoleClaim {
                RoleId = role.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };

            return _repository.SaveAsync (roleClaim, token : cancellationToken);
        }

        public override Task<IdentityResult> CreateAsync (Role role, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (role, nameof (role));

            return _repository.SaveAsync (role, token : cancellationToken)
                .ContinueWith (StoreUtils.IdentityResultContinuation);
        }

        public override Task<IdentityResult> DeleteAsync (Role role, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (role, nameof (role));

            return _repository.DeleteAsync (role, token : cancellationToken)
                .ContinueWith (StoreUtils.IdentityResultContinuation);
        }

        public override Task<Role> FindByIdAsync (string id, CancellationToken cancellationToken = default) {
            Prevent.ParameterNullOrWhiteSpace (id, nameof (id));

            return _repository.FindOneAsync<Role> (Guid.Parse (id), token : cancellationToken);
        }

        public override Task<Role> FindByNameAsync (string normalizedName, CancellationToken cancellationToken = default) {
            Prevent.ParameterNullOrWhiteSpace (normalizedName, nameof (normalizedName));

            return _repository.FindOneAsync<Role> (
                expression: _ => _.NormalizedName == normalizedName,
                token: cancellationToken
            );
        }

        public override Task<IList<Claim>> GetClaimsAsync (Role role, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (role, nameof (role));

            var query = _repository
                .Query<RoleClaim> ()
                .Where (_ => _.RoleId == role.Id);

            IList<Claim> result = new List<Claim> ();
            foreach (var item in query) {
                cancellationToken.ThrowIfCancellationRequested ();
                result.Add (item.ToClaim ());
            }
            return Task.FromResult (result);
        }

        public override Task RemoveClaimAsync (Role role, Claim claim, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (role, nameof (role));
            Prevent.ParameterNull (claim, nameof (claim));

            var roleClaim = new RoleClaim {
                RoleId = role.Id,
                ClaimType = claim.Type
            };
            return _repository.DeleteAsync (roleClaim, token: cancellationToken);
        }

        public override Task<IdentityResult> UpdateAsync (Role role, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (role, nameof (role));

            return _repository.SaveAsync (role, token : cancellationToken)
                .ContinueWith (StoreUtils.IdentityResultContinuation);
        }

        #endregion
    }
}