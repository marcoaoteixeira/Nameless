using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.AspNetCore.Identity.Stores.NHibernate.Models;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace Nameless.AspNetCore.Identity.Stores {
    public partial class UserStore : IUserRoleStore<User> {
        #region IUserRoleStore<User> Members

        public Task AddToRoleAsync (User user, string roleName, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (roleName, nameof (roleName));

            var criteria = Session.CreateCriteria<Role> ();

            criteria
                .Add (Restrictions.Eq (nameof (Role.Name), roleName));

            cancellationToken.ThrowIfCancellationRequested ();
            var role = criteria.UniqueResult<Role> ();

            var userInRole = new UserInRole {
                UserID = user.ID,
                RoleID = role.ID
            };

            return Session.SaveOrUpdateAsync (userInRole, cancellationToken);
        }

        public Task<IList<string>> GetRolesAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            Role role = null;
            UserInRole userInRole = null;
            return Session
                .QueryOver (() => role)
                .JoinEntityAlias (
                    alias: () => userInRole,
                    withClause: () => role.ID == userInRole.RoleID,
                    joinType : JoinType.InnerJoin
                )
                .Where (() => userInRole.UserID == user.ID)
                .ListAsync (cancellationToken)
                .ContinueWith (continuation => {
                    IList<string> result = null;
                    if (continuation.CanContinue ()) {
                        result = continuation.Result.Select (_ => _.Name).ToList ();
                    }
                    return result ?? new List<string> ();
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public Task<IList<User>> GetUsersInRoleAsync (string roleName, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (roleName, nameof (roleName));

            User user = null;
            Role role = null;
            UserInRole userInRole = null;
            return Session
                .QueryOver (() => user)
                .JoinEntityAlias (
                    alias: () => userInRole,
                    withClause: () => userInRole.UserID == user.ID,
                    joinType : JoinType.InnerJoin
                )
                .JoinEntityAlias (
                    alias: () => role,
                    withClause: () => role.ID == userInRole.RoleID,
                    joinType : JoinType.InnerJoin
                )
                .Where (() => role.Name == roleName)
                .ListAsync (cancellationToken)
                .ContinueWith (continuation => {
                    IList<User> result = null;
                    if (continuation.CanContinue ()) {
                        result = continuation.Result;
                    }
                    return result ?? new List<User> ();
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public Task<bool> IsInRoleAsync (User user, string roleName, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (roleName, nameof (roleName));

            var roleCriteria = Session.CreateCriteria<Role> ();

            roleCriteria
                .Add (Restrictions.Eq (nameof (Role.Name), roleName));

            cancellationToken.ThrowIfCancellationRequested ();
            var role = roleCriteria.UniqueResult<Role> ();

            var userInRoleCriteria = Session.CreateCriteria<UserInRole> ();

            userInRoleCriteria
                .Add (Restrictions.Eq (nameof (UserInRole.UserID), user.ID))
                .Add (Restrictions.Eq (nameof (UserInRole.RoleID), role.ID));

            cancellationToken.ThrowIfCancellationRequested ();
            var isInRole = userInRoleCriteria.UniqueResult<UserInRole> () != null;

            return Task.FromResult (isInRole);
        }

        public Task RemoveFromRoleAsync (User user, string roleName, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (roleName, nameof (roleName));

            var criteria = Session.CreateCriteria<Role> ();

            criteria
                .Add (Restrictions.Eq (nameof (Role.Name), roleName));

            cancellationToken.ThrowIfCancellationRequested ();
            var role = criteria.UniqueResult<Role> ();

            var userInRole = new UserInRole {
                UserID = user.ID,
                RoleID = role.ID
            };

            return Session.DeleteAsync (userInRole, cancellationToken);
        }

        #endregion
    }
}