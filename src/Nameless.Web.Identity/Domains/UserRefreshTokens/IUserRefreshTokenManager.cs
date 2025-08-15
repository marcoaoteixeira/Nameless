using Nameless.Web.Identity.Entities;

namespace Nameless.Web.Identity.Domains.UserRefreshTokens;

public interface IUserRefreshTokenManager {
    Task<UserRefreshToken> CreateAsync(User user, CancellationToken cancellationToken);

    Task<int> CleanUpAsync(User user, CancellationToken cancellationToken);

    Task<int> RevokeAsync(User user, RevokeUserRefreshTokenMetadata metadata, CancellationToken cancellationToken);

    Task<UserRefreshToken?> GetAsync(string token, CancellationToken cancellationToken);
}