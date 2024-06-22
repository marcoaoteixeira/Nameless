using Nameless.Web.Services;

namespace Nameless.Web {
    public static class JwtClaimsExtension {
        #region Public Static Methods

        public static Dictionary<string, object> ToDictionary(this JwtClaims self) {
            var properties = typeof(JwtClaims).GetProperties();
            var result = new Dictionary<string, object>();
            foreach (var property in properties) {
                var value = property.GetValue(self);
                if (value is null) {
                    continue;
                }

                var description = property.GetDescription()
                    ?? property.Name;

                result[description] = value;
            }
            return result;
        }

        #endregion
    }
}
