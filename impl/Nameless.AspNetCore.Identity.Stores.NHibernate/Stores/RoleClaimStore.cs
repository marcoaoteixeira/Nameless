using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NHibernate.Criterion;

namespace Nameless.AspNetCore.Identity.Stores {
    public partial class RoleStore : IRoleClaimStore<Role> {
        #region IRoleClaimStore<Role> Members

        public Task AddClaimAsync (Role role, Claim claim, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (role, nameof (role));
            Prevent.ParameterNull (claim, nameof (claim));

            var roleClaim = new RoleClaim {
                RoleID = role.ID,
                Type = claim.Type,
                Value = claim.Value
            };

            return Session.SaveOrUpdateAsync (roleClaim, cancellationToken);
        }

        public Task<IList<Claim>> GetClaimsAsync (Role role, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (role, nameof (role));

            var criteria = Session.CreateCriteria<RoleClaim> ();

            criteria
                .Add (Restrictions.Eq (nameof (RoleClaim.RoleID), role.ID));

            return criteria
                .ListAsync<RoleClaim> (cancellationToken)
                .ContinueWith (continuation => {
                    IList<Claim> result = new List<Claim> ();
                    if (continuation.CanContinue ()) {
                        result = continuation.Result.Select (RoleClaim.Parse).ToList ();
                    }
                    return result;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public Task RemoveClaimAsync (Role role, Claim claim, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (role, nameof (role));
            Prevent.ParameterNull (claim, nameof (claim));

            var criteria = Session.CreateCriteria<RoleClaim> ();

            criteria
                .Add (Restrictions.Eq (nameof (RoleClaim.RoleID), role.ID))
                .Add (Restrictions.Eq (nameof (RoleClaim.Type), claim.Type));

            cancellationToken.ThrowIfCancellationRequested ();
            var roleClaim = criteria.UniqueResult<RoleClaim> ();

            return Session.DeleteAsync (roleClaim, cancellationToken);
        }

        #endregion
    }
}