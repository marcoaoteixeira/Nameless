using System.Security.Claims;

namespace Nameless.AspNetCore.Identity {
    public class RoleClaim {
        #region Public Virtual Properties

        public virtual string RoleID { get; set; }
        public virtual string Type { get; set; }
        public virtual string Value { get; set; }

        #endregion

        #region Public Static Methods

        public static Claim Parse (RoleClaim claim) {
            if (claim == null) { return null; }

            var result = new Claim (claim.Type, claim.Value);
            result.Properties[nameof (RoleID)] = claim.RoleID.ToString ();
            return result;
        }

        #endregion

        #region Public Virtual Methods

        public virtual bool Equals (RoleClaim obj) => obj != null && obj.RoleID == RoleID && obj.Type == Type;

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) => Equals (obj as RoleClaim);

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += (RoleID ?? string.Empty).GetHashCode () * 7;
                hash += (Type ?? string.Empty).GetHashCode () * 7;
            }
            return hash;
        }

        #endregion
    }
}