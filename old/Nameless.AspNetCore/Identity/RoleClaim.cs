using System.Security.Claims;

namespace Nameless.AspNetCore.Identity {
    public class RoleClaim {
        #region Public Properties

        public string RoleID { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        #endregion

        #region Public Static Methods

        public static Claim Parse (RoleClaim claim) {
            if (claim == null) { return null; }

            var result = new Claim (claim.Type, claim.Value);
            result.Properties[nameof (RoleID)] = claim.RoleID.ToString ();
            return result;
        }

        #endregion
    }
}