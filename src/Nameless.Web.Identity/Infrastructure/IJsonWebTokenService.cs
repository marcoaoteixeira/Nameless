namespace Nameless.Web.Identity.Infrastructure;

/// <summary>
///     Represents a service for managing JSON Web Tokens (JWTs).
/// </summary>
public interface IJsonWebTokenService {
    /// <summary>
    ///     Creates a new JSON Web Token for the specified user ID.
    /// </summary>
    /// <param name="userID">
    ///     The ID of the user for whom the token is created.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token to observe while waiting for the task to
    ///     complete.
    /// </param>
    /// <returns>
    ///     An asynchronous operation that returns a
    ///     <see cref="JsonWebTokenResponse"/>.
    /// </returns>
    Task<JsonWebTokenResponse> CreateAsync(Guid userID, CancellationToken cancellationToken);

    /// <summary>
    ///     Refreshes the JSON Web Token using the provided refresh token.
    /// </summary>
    /// <param name="refreshToken">
    ///     The refresh token used to obtain a new access token.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token to observe while waiting for the task to
    ///     complete.
    /// </param>
    /// <returns>
    ///     An asynchronous operation that returns a
    ///     <see cref="JsonWebTokenResponse"/>.
    /// </returns>
    Task<JsonWebTokenResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
}