using System.Reflection;
using Nameless.Web.Services;

namespace Nameless.Web;

public static class JwtClaimsExtension {
    #region Public Static Methods

    public static Dictionary<string, object> ToDictionary(this JwtClaims self) {
        var properties = typeof(JwtClaims).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var result = new Dictionary<string, object>();
        foreach (var property in properties) {
            var value = property.GetValue(self);
            if (value is null) {
                continue;
            }
            result[property.GetDescription()] = value;
        }
        return result;
    }

    #endregion
}