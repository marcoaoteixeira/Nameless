using System.Security.Claims;
using Nameless.Results;

namespace Nameless.Microservices.Application.Identity;

public interface IIdentityService {
    Task<bool> AuthorizeAsync<TKey>(TKey userID, string policy, CancellationToken cancellationToken)
        where TKey : IComparable<TKey>;

    TKey GetUserID<TKey>(ClaimsPrincipal principal)
        where TKey : IComparable<TKey>;

    Task<ClaimsPrincipal?> GetCurrentUserAsync(CancellationToken cancellationToken);

    Task<ClaimsPrincipal?> GetUserAsync<TKey>(TKey userID, CancellationToken cancellationToken)
        where TKey : IComparable<TKey>;

    Task<Result<ClaimsPrincipal>> CreateUserAsync(User user, CancellationToken cancellationToken);

    Task<bool> DeleteAsync<TKey>(TKey userID, CancellationToken cancellationToken)
        where TKey : IComparable<TKey>;
}
