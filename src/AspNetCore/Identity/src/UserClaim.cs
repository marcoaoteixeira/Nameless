using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Nameless.AspNetCore.Identity {
    public class UserClaim : IdentityUserClaim<Guid> {

        #region Public Override Methods

        public override Claim ToClaim () {
            var result = new Claim (ClaimType, ClaimValue);
            result.Properties[nameof (UserId)] = UserId.ToString ();
            return result;
        }

        public virtual bool Equals (UserClaim obj) => obj != null && obj.UserId == UserId && obj.ClaimType == ClaimType;

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) => Equals (obj as UserClaim);

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += UserId.GetHashCode () * 7;
                hash += (ClaimType ?? string.Empty).GetHashCode () * 7;
            }
            return hash;
        }

        #endregion
    }
}