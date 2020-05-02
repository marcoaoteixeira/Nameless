using System.Security.Claims;

namespace Nameless.AspNetCore.Identity {
    public class UserClaim {
        #region Public Virtual Properties

        public virtual string UserID { get; set; }
        public virtual string Type { get; set; }
        public virtual string Value { get; set; }

        #endregion

        #region Public Static Methods

        public static Claim Parse (UserClaim claim) {
            if (claim == null) { return null; }

            var result = new Claim (claim.Type, claim.Value);
            result.Properties[nameof (UserID)] = claim.UserID.ToString ();
            return result;
        }

        #endregion

        #region Public Virtual Methods

        public virtual bool Equals (UserClaim obj) => obj != null && obj.UserID == UserID && obj.Type == Type;

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) => Equals (obj as UserClaim);

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += (UserID ?? string.Empty).GetHashCode () * 7;
                hash += (Type ?? string.Empty).GetHashCode () * 7;
            }
            return hash;
        }

        #endregion
    }
}