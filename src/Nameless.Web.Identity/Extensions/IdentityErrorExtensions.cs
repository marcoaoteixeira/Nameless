using Microsoft.AspNetCore.Identity;

namespace Nameless.Web.Identity;

public static class IdentityErrorExtensions {
    public static string ToErrorMessage(this IEnumerable<IdentityError> self) {
        return string.Join(separator: ", ", self.Select(error => error.Description));
    }
}