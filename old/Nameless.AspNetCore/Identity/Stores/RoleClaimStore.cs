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

namespace Nameless.AspNetCore.Identity {
    public partial class RoleStore : IRoleClaimStore<Role> {
        #region IRoleClaimStore<Role> Members

        public Task AddClaimAsync (Role role, Claim claim, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (role, nameof (role));
            Prevent.ParameterNull (claim, nameof (claim));

            var sql = GetSQLScript (nameof (RoleClaim), nameof (AddClaimAsync));
            return Database.ExecuteNonQueryAsync (sql, token : cancellationToken, parameters : new [] {
                Parameter.CreateInputParameter (nameof (RoleClaim.RoleID), Guid.Parse (role.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (RoleClaim.Type), claim.Type),
                Parameter.CreateInputParameter (nameof (RoleClaim.Value), claim.Value)
            });
        }

        public async Task<IList<Claim>> GetClaimsAsync (Role role, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (role, nameof (role));

            var sql = GetSQLScript (nameof (RoleClaim), nameof (GetClaimsAsync));
            return await Database.ExecuteReaderAsync (sql, Mapper.Map<RoleClaim, IDataRecord>, parameters : new [] {
                Parameter.CreateInputParameter (nameof (RoleClaim.RoleID), Guid.Parse (role.ID), DbType.Guid)
            })
            .ToListAsync (cancellationToken)
            .ContinueWith (continuation => {
                IList<Claim> result = null;
                if (continuation.CanContinue ()) {
                    result = continuation.Result.Select (RoleClaim.Parse).ToList ();
                }
                return result ?? new List<Claim> ();
            });
        }

        public Task RemoveClaimAsync (Role role, Claim claim, CancellationToken cancellationToken = default) {
            Prevent.ParameterNull (role, nameof (role));
            Prevent.ParameterNull (claim, nameof (claim));

            var sql = GetSQLScript (nameof (RoleClaim), nameof (RemoveClaimAsync));
            return Database.ExecuteNonQueryAsync (sql, token : cancellationToken, parameters : new [] {
                Parameter.CreateInputParameter (nameof (RoleClaim.RoleID), Guid.Parse (role.ID), DbType.Guid),
                Parameter.CreateInputParameter (nameof (RoleClaim.Type), claim.Type)
            });
        }

        #endregion
    }
}