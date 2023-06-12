namespace Nameless.WebApplication.Services {

    public interface IRefreshTokenService {

        #region Methods

        Task<string> GenerateAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Discards all invalid refresh tokens for an user.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A <see cref="Task"/> representing the method execution.</returns>
        Task SanitizeAsync(Guid userId, CancellationToken cancellationToken = default);

        Task RevokeAsync(string refreshToken, string? reason = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Replaces the previous token and retrieves a new one.
        /// </summary>
        /// <param name="previousRefreshToken">The previous refresh token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A new refresh token.</returns>
        Task<string> ReplaceAsync(string previousRefreshToken, CancellationToken cancellationToken = default);

        #endregion
    }
}
