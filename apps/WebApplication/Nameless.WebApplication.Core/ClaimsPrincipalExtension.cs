using System;
using System.Security.Claims;

namespace Nameless.WebApplication {
    public static class ClaimsPrincipalExtension {
        #region Public Static Methods

        public static T GetUserID<T> (this ClaimsPrincipal self) {
            Prevent.ParameterNull (self, nameof (self));

            var userID = self.FindFirstValue (ClaimTypes.NameIdentifier);
            if (userID == null) { return default; }

            object result;
            switch (Type.GetTypeCode (typeof (T))) {
                case TypeCode.Int16:
                    result = short.Parse (userID);
                    break;

                case TypeCode.Int32:
                    result = int.Parse (userID);
                    break;

                case TypeCode.Int64:
                    result = long.Parse (userID);
                    break;

                case TypeCode.String:
                    result = userID;
                    break;

                default:
                    if (typeof (T) == typeof (Guid)) { result = Guid.Parse (userID); }
                    else { throw new InvalidOperationException ("Invalid type provided."); }
                    break;
            }

            return (T) result;
        }

        public static string GetUserName (this ClaimsPrincipal self) {
            Prevent.ParameterNull (self, nameof (self));

            return self.FindFirstValue (ClaimTypes.Name);
        }

        public static string GetUserEmail (this ClaimsPrincipal self) {
            Prevent.ParameterNull (self, nameof (self));

            return self.FindFirstValue (ClaimTypes.Email);
        }

        #endregion
    }
}