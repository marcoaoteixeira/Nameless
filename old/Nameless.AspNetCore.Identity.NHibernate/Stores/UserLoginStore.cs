using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace Nameless.AspNetCore.Identity.Stores {
    public partial class UserStore : IUserLoginStore<User> {
        #region IUserLoginStore<User> Members

        public Task AddLoginAsync (User user, UserLoginInfo login, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNull (login, nameof (login));

            var userLogin = new UserLogin {
                UserID = user.ID,
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                ProviderDisplayName = login.ProviderDisplayName
            };

            return Session.SaveAsync (userLogin, cancellationToken);
        }

        public Task<User> FindByLoginAsync (string loginProvider, string providerKey, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (loginProvider, nameof (loginProvider));
            Prevent.ParameterNullOrWhiteSpace (providerKey, nameof (providerKey));

            User user = null;
            UserLogin userLogin = null;
            return Session
                .QueryOver (() => user)
                .JoinEntityAlias (
                    alias: () => userLogin,
                    withClause: () => user.ID == userLogin.UserID,
                    joinType : JoinType.InnerJoin
                )
                .Where (() => userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey)
                .SingleOrDefaultAsync (cancellationToken);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var criteria = Session.CreateCriteria<UserLogin> ();

            criteria
                .Add (Restrictions.Eq (nameof (UserLogin.UserID), user.ID));

            return criteria
                .ListAsync<UserLogin> (cancellationToken)
                .ContinueWith (continuation => {
                    IList<UserLoginInfo> result = null;
                    if (continuation.CanContinue ()) {
                        result = continuation.Result.Select (UserLogin.Parse).ToList ();
                    }
                    return result ?? new List<UserLoginInfo> ();
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public Task RemoveLoginAsync (User user, string loginProvider, string providerKey, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (loginProvider, nameof (loginProvider));
            Prevent.ParameterNullOrWhiteSpace (providerKey, nameof (providerKey));

            var userLogin = new UserLogin {
                UserID = user.ID,
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            };

            return Session.DeleteAsync (userLogin, cancellationToken);
        }

        #endregion
    }
}