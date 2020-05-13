using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NHibernate;
using NHibernate.Criterion;

namespace Nameless.AspNetCore.Identity.Stores {
    public partial class UserStore : StoreBase, IUserStore<User> {
        #region Public Constructors

        public UserStore (ISession session) : base (session) { }

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

            user.ID = Guid.NewGuid ().ToString ();

            return Session.SaveAsync (user, cancellationToken).ContinueWith (IdentityResultContinuation);
        }

        public Task<IdentityResult> UpdateAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            return Session.UpdateAsync (user, cancellationToken).ContinueWith (IdentityResultContinuation);
        }

        public Task<IdentityResult> DeleteAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            return Session.DeleteAsync (user, cancellationToken).ContinueWith (IdentityResultContinuation);
        }

        public Task<User> FindByIdAsync (string userId, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (userId, nameof (userId));

            var criteria = Session.CreateCriteria<User> ();

            criteria
                .Add (Restrictions.Eq (nameof (User.ID), userId));

            return criteria.UniqueResultAsync<User> (cancellationToken);
        }

        public Task<User> FindByNameAsync (string normalizedUserName, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (normalizedUserName, nameof (normalizedUserName));

            var criteria = Session.CreateCriteria<User> ();

            criteria
                .Add (Restrictions.Eq (nameof (User.NormalizedUserName), normalizedUserName));

            return criteria.UniqueResultAsync<User> (cancellationToken);
        }

        public void Dispose () {
            /* Nothing to dispose of */
        }

        #endregion
    }
}