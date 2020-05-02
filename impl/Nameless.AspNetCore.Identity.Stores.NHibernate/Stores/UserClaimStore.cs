using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace Nameless.AspNetCore.Identity.Stores {
    // Parent file is UserStore.cs !!!
    public partial class UserStore : IUserClaimStore<User> {
        #region IUserClaimStore<User> Members

        public Task AddClaimsAsync (User user, IEnumerable<Claim> claims, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNull (claims, nameof (claims));

            using (var transaction = Session.BeginTransaction ()) {
                foreach (var claim in claims) {
                    cancellationToken.ThrowIfCancellationRequested ();
                    var userClaim = new UserClaim {
                        UserID = user.ID,
                        Type = claim.Type,
                        Value = claim.Value
                    };
                    Session.Save (userClaim);
                }
                Session.Flush ();
                transaction.Commit ();
            }

            return Task.CompletedTask;
        }

        public Task<IList<Claim>> GetClaimsAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var criteria = Session.CreateCriteria<UserClaim> ();

            criteria
                .Add (Restrictions.Eq (nameof (UserClaim.UserID), user.ID));

            return criteria
                .ListAsync<UserClaim> (cancellationToken)
                .ContinueWith (continuation => {
                    IList<Claim> result = null;
                    if (continuation.CanContinue ()) {
                        result = continuation.Result.Select (UserClaim.Parse).ToList ();
                    }
                    return result ?? new List<Claim> ();
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public Task<IList<User>> GetUsersForClaimAsync (Claim claim, CancellationToken cancellationToken) {
            Prevent.ParameterNull (claim, nameof (claim));

            User user = null;
            UserClaim userClaim = null;
            return Session
                .QueryOver (() => user)
                .JoinEntityAlias (
                    alias: () => userClaim,
                    withClause: () => user.ID == userClaim.UserID,
                    joinType : JoinType.InnerJoin
                )
                .Where (() => userClaim.Type == claim.Type)
                .ListAsync (cancellationToken)
                .ContinueWith (continuation => {
                    IList<User> result = null;
                    if (continuation.CanContinue ()) {
                        result = new List<User> (continuation.Result);
                    }
                    return result ?? new List<User> ();
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public Task RemoveClaimsAsync (User user, IEnumerable<Claim> claims, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNull (claims, nameof (claims));

            using (var transaction = Session.BeginTransaction ()) {
                foreach (var claim in claims) {
                    cancellationToken.ThrowIfCancellationRequested ();
                    var userClaim = new UserClaim {
                        UserID = user.ID,
                        Type = claim.Type,
                        Value = claim.Value
                    };
                    Session.Delete (userClaim);
                }
                Session.Flush ();
                transaction.Commit ();
            }

            return Task.CompletedTask;
        }

        public Task ReplaceClaimAsync (User user, Claim claim, Claim newClaim, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNull (claim, nameof (claim));
            Prevent.ParameterNull (newClaim, nameof (newClaim));

            var criteria = Session.CreateCriteria<UserClaim> ();

            criteria
                .Add (Restrictions.Eq (nameof (UserClaim.UserID), user.ID))
                .Add (Restrictions.Eq (nameof (UserClaim.Type), claim.Type));

            cancellationToken.ThrowIfCancellationRequested ();
            var userClaim = criteria.UniqueResult<UserClaim> ();

            if (userClaim != null) {
                userClaim.Type = newClaim.Type;
                userClaim.Value = newClaim.Value;

                return Session.UpdateAsync (userClaim, cancellationToken);
            }
            return Task.CompletedTask;
        }

        #endregion
    }
}