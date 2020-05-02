using System.Security.Claims;

namespace Nameless.AspNetCore.Identity {
    public class UserClaim {
        #region Public Properties

        public string UserID { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        #endregion

        #region Public Static Methods

        public static Claim Parse (UserClaim claim) {
            if (claim == null) { return null; }

            var result = new Claim (claim.Type, claim.Value);
            result.Properties[nameof (UserID)] = claim.UserID.ToString ();
            return result;
        }

        #endregion
    }
}