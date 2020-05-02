using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Dasync.Collections;
using Microsoft.AspNetCore.Identity;
using Nameless.Data;
using Nameless.ObjectMapper;

namespace Nameless.WebApplication.Identity {
    // Parent file is UserStore.cs !!!
    public partial class UserStore : IUserClaimStore<User> {
        #region IUserClaimStore<User> Members

        public async Task AddClaimsAsync (User user, IEnumerable<Claim> claims, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNull (claims, nameof (claims));

            var sql = GetSQLScript (nameof (UserClaim), nameof (AddClaimsAsync));
            using (var transaction = Database.StartTransaction ()) {
                foreach (var claim in claims) {
                    cancellationToken.ThrowIfCancellationRequested ();
                    await Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                        Parameter.CreateInputParameter (nameof (UserClaim.UserID), Guid.Parse (user.ID), DbType.Guid),
                        Parameter.CreateInputParameter (nameof (UserClaim.Type), claim.Type),
                        Parameter.CreateInputParameter (nameof (UserClaim.Value), claim.Value)
                    });
                }
                transaction.Commit ();
            }
        }

        public Task<IList<Claim>> GetClaimsAsync (User user, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));

            var sql = GetSQLScript (nameof (UserClaim), nameof (GetClaimsAsync));
            return Database.ExecuteReaderAsync (sql, Mapper.Map<UserClaim, IDataRecord>, parameters: new[] {
                Parameter.CreateInputParameter (nameof (UserClaim.UserID), Guid.Parse (user.ID), DbType.Guid)
            })
            .ToArrayAsync (cancellationToken)
            .ContinueWith (continuation => {
                IList<Claim> result = null;
                if (continuation.CanContinue ()) {
                    result = continuation.Result.Select (UserClaim.Parse).ToList ();
                }
                return result ?? new List<Claim> ();
            });
        }

        public Task<IList<User>> GetUsersForClaimAsync (Claim claim, CancellationToken cancellationToken) {
            Prevent.ParameterNull (claim, nameof (claim));

            var sql = GetSQLScript (nameof (UserClaim), nameof (GetUsersForClaimAsync));
            return Database.ExecuteReaderAsync (sql, Mapper.Map<User, IDataRecord>, parameters: new[] {
                Parameter.CreateInputParameter (nameof (UserClaim.Type), claim.Type)
            })
            .ToArrayAsync (cancellationToken)
            .ContinueWith (continuation => {
                IList<User> result = null;
                if (continuation.CanContinue ()) {
                    result = new List<User> (continuation.Result);
                }
                return result ?? new List<User> ();
            });
        }

        public async Task RemoveClaimsAsync (User user, IEnumerable<Claim> claims, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNull (claims, nameof (claims));

            var sql = GetSQLScript (nameof (UserClaim), nameof (RemoveClaimsAsync));
            using (var transaction = Database.StartTransaction ()) {
                foreach (var claim in claims) {
                    cancellationToken.ThrowIfCancellationRequested ();
                    await Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                        Parameter.CreateInputParameter (nameof (UserClaim.UserID), Guid.Parse (user.ID), DbType.Guid),
                        Parameter.CreateInputParameter (nameof (UserClaim.Type), claim.Type)
                    });
                }
                transaction.Commit ();
            }
        }

        public Task ReplaceClaimAsync (User user, Claim claim, Claim newClaim, CancellationToken cancellationToken) {
            Prevent.ParameterNull (user, nameof (user));
            Prevent.ParameterNull (claim, nameof (claim));
            Prevent.ParameterNull (newClaim, nameof (newClaim));

            var sql = GetSQLScript (nameof (UserClaim), nameof (ReplaceClaimAsync));
            return Database.ExecuteNonQueryAsync (sql, token: cancellationToken, parameters: new[] {
                Parameter.CreateInputParameter (nameof (UserClaim.UserID), Guid.Parse (user.ID), DbType.Guid),
                Parameter.CreateInputParameter ("OldClaimType", claim.Type),
                Parameter.CreateInputParameter ("NewClaimType", newClaim.Type),
                Parameter.CreateInputParameter ("NewClaimValue", newClaim.Value)
            });
        }

        #endregion
    }
}