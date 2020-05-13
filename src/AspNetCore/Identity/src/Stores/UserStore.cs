using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.Persistence;

namespace Nameless.AspNetCore.Identity {
    public sealed class UserStore : UserStoreBase<User, Guid, UserClaim, UserLogin, UserToken> {
        #region Private Read-Only Fields

        private readonly IRepository _repository;

        #endregion

        #region Public Constructors

        public UserStore (IRepository repository, IdentityErrorDescriber describer) : base (describer) {
            Prevent.ParameterNull (repository, nameof (repository));

            _repository = repository;
        }

        #endregion

        #region Public Override Methods

        public override IQueryable<User> Users => _repository.Query<User> ();

        public override Task AddClaimsAsync (User user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (user, nameof (user));

            if (!claims.Any ()) { return Task.CompletedTask; }

            var userClaims = new List<UserClaim> ();

            foreach (var claim in claims) {
                cancellationToken.ThrowIfCancellationRequested ();

                userClaims.Add (new UserClaim {
                    UserId = user.Id,
                        ClaimType = claim.Type,
                        ClaimValue = claim.Value
                });
            }

            return _repository.SaveAsync (userClaims.ToArray (), token : cancellationToken);
        }

        public override Task AddLoginAsync (User user, UserLoginInfo login, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNull (login, nameof (login));

            var userLogin = new UserLogin {
                UserId = user.Id,
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                ProviderDisplayName = login.ProviderDisplayName
            };

            return _repository.SaveAsync (userLogin, cancellationToken);
        }

        public override Task<IdentityResult> CreateAsync (User user, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (user, nameof (user));

            return _repository
                .SaveAsync (user, cancellationToken)
                .ContinueWith (StoreUtils.IdentityResultContinuation);
        }

        public override Task<IdentityResult> DeleteAsync (User user, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (user, nameof (user));

            return _repository
                .DeleteAsync (user, cancellationToken)
                .ContinueWith (StoreUtils.IdentityResultContinuation);
        }

        public override Task<User> FindByEmailAsync (string normalizedEmail, CancellationToken cancellationToken = default) {
            Prevent.ParameterNullOrWhiteSpace (normalizedEmail, nameof (normalizedEmail));

            return _repository.FindOneAsync<User> (_ => _.NormalizedEmail == normalizedEmail, cancellationToken);
        }

        public override Task<User> FindByIdAsync (string userId, CancellationToken cancellationToken = default) {
            Prevent.ParameterNullOrWhiteSpace (userId, nameof (userId));

            var id = Guid.Parse (userId);
            return _repository.FindOneAsync<User> (_ => _.Id == id, cancellationToken);
        }

        public override Task<User> FindByNameAsync (string normalizedUserName, CancellationToken cancellationToken = default) {
            Prevent.ParameterNullOrWhiteSpace (normalizedUserName, nameof (normalizedUserName));

            return _repository.FindOneAsync<User> (_ => _.NormalizedUserName == normalizedUserName, cancellationToken);
        }

        public override Task<IList<Claim>> GetClaimsAsync (User user, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (user, nameof (user));

            var items = _repository
                .Query<UserClaim> ()
                .Where (_ => _.UserId == user.Id);

            IList<Claim> result = new List<Claim> ();
            foreach (var item in items) {
                cancellationToken.ThrowIfCancellationRequested ();
                result.Add (item.ToClaim ());
            }
            return Task.FromResult (result);
        }

        public override Task<IList<UserLoginInfo>> GetLoginsAsync (User user, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (user, nameof (user));

            var items = _repository
                .Query<UserLogin> ()
                .Where (_ => _.UserId == user.Id);

            IList<UserLoginInfo> result = new List<UserLoginInfo> ();
            foreach (var item in items) {
                cancellationToken.ThrowIfCancellationRequested ();
                result.Add (item.ToUserLoginInfo ());
            }
            return Task.FromResult (result);
        }

        public override Task<IList<User>> GetUsersForClaimAsync (Claim claim, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (claim, nameof (claim));

            var users = _repository.Query<User> ();
            var userClaims = _repository.Query<UserClaim> ();

            var query = from userClaim in userClaims
            join user in users on userClaim.UserId equals user.Id
            where userClaim.ClaimType == claim.Type &&
                userClaim.ClaimValue == claim.Value
            select user;

            IList<User> result = new List<User> ();
            foreach (var item in query) {
                cancellationToken.ThrowIfCancellationRequested ();
                result.Add (item);
            }
            return Task.FromResult (result);
        }

        public override Task RemoveClaimsAsync (User user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNull (claims, nameof (claims));

            var items = new List<UserClaim> ();
            foreach (var claim in claims) {
                cancellationToken.ThrowIfCancellationRequested ();
                items.Add (new UserClaim {
                    UserId = user.Id,
                        ClaimType = claim.Type,
                        ClaimValue = claim.Value
                });
            }

            return _repository.DeleteAsync (items.ToArray (), token : cancellationToken);
        }

        public override Task RemoveLoginAsync (User user, string loginProvider, string providerKey, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (loginProvider, nameof (loginProvider));
            Prevent.ParameterNullOrWhiteSpace (providerKey, nameof (providerKey));

            var userLogin = new UserLogin {
                UserId = user.Id,
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            };

            return _repository.DeleteAsync (userLogin, token : cancellationToken);
        }

        public override async Task ReplaceClaimAsync (User user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNull (claim, nameof (claim));
            Prevent.ParameterNull (newClaim, nameof (newClaim));

            var currentUserClaim = await _repository
                .FindOneAsync<UserClaim> (_ => _.UserId == user.Id && _.ClaimType == claim.Type, token : cancellationToken);

            if (currentUserClaim != null) {
                currentUserClaim.ClaimType = claim.Type;
                currentUserClaim.ClaimValue = claim.Value;
                await _repository.SaveAsync (currentUserClaim, token : cancellationToken);
            }
        }

        public override Task<IdentityResult> UpdateAsync (User user, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (user, nameof (user));

            return _repository.SaveAsync (user, token : cancellationToken)
                .ContinueWith (StoreUtils.IdentityResultContinuation);
        }

        protected override Task AddUserTokenAsync (UserToken token) {
            Prevent.ParameterNull (token, nameof (token));

            return _repository.SaveAsync (token);
        }

        protected override Task<UserToken> FindTokenAsync (User user, string loginProvider, string name, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNullOrWhiteSpace (loginProvider, nameof (loginProvider));
            Prevent.ParameterNullOrWhiteSpace (name, nameof (name));

            return _repository.FindOneAsync<UserToken> (
                expression: _ =>
                _.UserId == user.Id &&
                _.LoginProvider == loginProvider &&
                _.Name == name,
                token : cancellationToken
            );
        }

        protected override Task<User> FindUserAsync (Guid userId, CancellationToken cancellationToken) {
            return _repository.FindOneAsync<User> (userId, token : cancellationToken);
        }

        protected override Task<UserLogin> FindUserLoginAsync (string loginProvider, string providerKey, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (loginProvider, nameof (loginProvider));
            Prevent.ParameterNullOrWhiteSpace (providerKey, nameof (providerKey));

            return _repository.FindOneAsync<UserLogin> (
                expression: _ =>
                _.LoginProvider == loginProvider &&
                _.ProviderKey == providerKey,
                token : cancellationToken
            );
        }

        protected override Task<UserLogin> FindUserLoginAsync (Guid userId, string loginProvider, string providerKey, CancellationToken cancellationToken) {
            Prevent.ParameterNullOrWhiteSpace (loginProvider, nameof (loginProvider));
            Prevent.ParameterNullOrWhiteSpace (providerKey, nameof (providerKey));

            return _repository.FindOneAsync<UserLogin> (
                expression: _ => _.UserId == userId &&
                _.LoginProvider == loginProvider &&
                _.ProviderKey == providerKey,
                token : cancellationToken
            );
        }

        protected override Task RemoveUserTokenAsync (UserToken token) {
            Prevent.ParameterNull (token, nameof (token));

            return _repository.DeleteAsync (token);
        }

        #endregion
    }
}