using System.Reflection;
using Nameless.Web.Services;

namespace Nameless.Web;

internal static class JwtClaimsExtension {
    internal static Dictionary<string, object> ToDictionary(this JwtParameters self) {
        Prevent.Argument.Null(self);

        var properties = typeof(JwtParameters).GetProperties(BindingFlags.Instance | BindingFlags.Public);
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
}