using Nameless.Web.Identity.Entities;

namespace Nameless.Web.Identity.Security;

/// <summary>
///     User refresh token services.
/// </summary>
public interface IUserRefreshTokenService {
    /// <summary>
    ///     Creates a new refresh token.
    /// </summary>
    /// <param name="userID">
    ///     The user ID.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     An asynchronous operation representing the method execution.
    ///     The result of the asynchronous operation is a new
    ///     <see cref="UserRefreshToken"/>.
    /// </returns>
    Task<UserRefreshToken> CreateAsync(Guid userID, CancellationToken cancellationToken);

    /// <summary>
    ///     Revokes the specified token.
    /// </summary>
    /// <param name="token">
    ///     The token.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     An asynchronous operation representing the method execution.
    /// </returns>
    Task RevokeAsync(string token, CancellationToken cancellationToken);

    /// <summary>
    ///     Checks if the specified token still active.
    /// </summary>
    /// <param name="token">
    ///     The token.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     An asynchronous operation representing the method execution.
    ///     Where the result of the asynchronous operation is
    ///     <see langword="true"/> if the token still active; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    Task<bool> IsActiveAsync(string token, CancellationToken cancellationToken);
}