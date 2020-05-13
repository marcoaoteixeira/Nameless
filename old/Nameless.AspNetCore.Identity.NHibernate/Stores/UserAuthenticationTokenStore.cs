using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NHibernate.Criterion;

namespace Nameless.AspNetCore.Identity.Stores {
    public partial class UserStore : IUserAuthenticationTokenStore<User> {
        #region IUserAuthenticationTokenStore<User> Members

        public Task<string> GetTokenAsync (User user, string loginProvider, string name, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (loginProvider, nameof (loginProvider));
            Prevent.ParameterNullOrWhiteSpace (name, nameof (name));

            var criteria = Session.CreateCriteria<UserToken> ();

            criteria
                .Add (Restrictions.Eq (nameof (UserToken.UserID), user.ID))
                .Add (Restrictions.Eq (nameof (UserToken.LoginProvider), loginProvider))
                .Add (Restrictions.Eq (nameof (UserToken.Name), name));

            return criteria
                .UniqueResultAsync<UserToken> (cancellationToken)
                .ContinueWith (continuation => continuation.CanContinue () ? continuation.Result.Value : null, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public Task RemoveTokenAsync (User user, string loginProvider, string name, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (loginProvider, nameof (loginProvider));
            Prevent.ParameterNullOrWhiteSpace (name, nameof (name));

            var criteria = Session.CreateCriteria<UserToken> ();

            criteria
                .Add (Restrictions.Eq (nameof (UserToken.UserID), user.ID))
                .Add (Restrictions.Eq (nameof (UserToken.LoginProvider), loginProvider))
                .Add (Restrictions.Eq (nameof (UserToken.Name), name));

            cancellationToken.ThrowIfCancellationRequested ();
            var userToken = criteria.UniqueResult<UserToken> ();

            return userToken != null ? Session.DeleteAsync (userToken, cancellationToken) : Task.CompletedTask;
        }

        public Task SetTokenAsync (User user, string loginProvider, string name, string value, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (loginProvider, nameof (loginProvider));
            Prevent.ParameterNullOrWhiteSpace (name, nameof (name));
            Prevent.ParameterNullOrWhiteSpace (value, nameof (value));

            var criteria = Session.CreateCriteria<UserToken> ();

            criteria
                .Add (Restrictions.Eq (nameof (UserToken.UserID), user.ID))
                .Add (Restrictions.Eq (nameof (UserToken.LoginProvider), loginProvider))
                .Add (Restrictions.Eq (nameof (UserToken.Name), name));

            cancellationToken.ThrowIfCancellationRequested ();
            var userToken = criteria.UniqueResult<UserToken> ();

            if (userToken != null) {
                userToken.Value = value;
                return Session.SaveOrUpdateAsync (userToken, cancellationToken);
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}