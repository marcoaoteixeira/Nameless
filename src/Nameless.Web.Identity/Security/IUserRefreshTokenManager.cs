using Nameless.Web.Identity.Entities;

namespace Nameless.Web.Identity.Security;

/// <summary>
///     Defines a contract for managing user refresh tokens.
/// </summary>
public interface IUserRefreshTokenManager {
    /// <summary>
    ///     Creates a new refresh token for the specified user.
    /// </summary>
    /// <param name="userID">
    ///     The user ID.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     An asynchronous operation representing the method execution.
    ///     The result of the asynchronous operation is the created token
    ///     string.
    /// </returns>
    Task<string> CreateAsync(Guid userID, CancellationToken cancellationToken);

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
    ///     Retrieves the user refresh token by the specified token string.
    /// </summary>
    /// <param name="token">
    ///     The refresh token string.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. Where the
    ///     result of the task is the <see cref="UserRefreshToken"/>.
    /// </returns>
    Task<UserRefreshToken?> GetAsync(string token, CancellationToken cancellationToken);
}