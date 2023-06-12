using System.Security.Claims;

namespace Nameless.WebApplication.Services {

    public interface IAccessTokenService {

        #region Methods

        Task<string> GenerateAsync(Guid userId, string userName, string userEmail, CancellationToken cancellationToken = default);

        Task<ClaimsPrincipal> ExtractAsync(string token, CancellationToken cancellationToken = default);

        #endregion
    }
}
