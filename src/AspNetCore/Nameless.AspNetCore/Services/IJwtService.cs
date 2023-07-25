using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Nameless.AspNetCore.Services {
    public interface IJwtService {
        #region Methods

        string Generate(string userId, string userName, string userEmail);

        bool Validate(string token, [NotNullWhen(true)] out ClaimsPrincipal? principal);

        #endregion
    }
}
