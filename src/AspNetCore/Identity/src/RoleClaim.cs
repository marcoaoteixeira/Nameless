using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Nameless.AspNetCore.Identity {
    public class RoleClaim : IdentityRoleClaim<Guid> {

        #region Public Override Methods

        public override Claim ToClaim () {
            var result = new Claim (ClaimType, ClaimValue);
            result.Properties[nameof (RoleId)] = RoleId.ToString ();
            return result;
        }

        public virtual bool Equals (RoleClaim obj) => obj != null && obj.RoleId == RoleId && obj.ClaimType == ClaimType;

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) => Equals (obj as RoleClaim);

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += RoleId.GetHashCode () * 7;
                hash += (ClaimType ?? string.Empty).GetHashCode () * 7;
            }
            return hash;
        }

        #endregion
    }
}