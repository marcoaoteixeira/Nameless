﻿namespace Nameless.Web.Identity.Security;

public record UserRefreshTokenOptions {
    /// <summary>
    ///     Gets or sets the refresh token time-to-live.
    /// </summary>
    /// <remarks>
    ///     Keep in mind that the time-to-live of the refresh token should be
    ///     a little bit longer than the access token time-to-live, so that
    ///     when the access token expires, the refresh token can be used.
    /// </remarks>
    public TimeSpan TokenTtl { get; set; } = TimeSpan.FromMinutes(60);

    /// <summary>
    ///     Gets or sets the maximum number of tokens to retain before cleanup.
    /// </summary>
    public int TokenRetentionLimit { get; set; } = 10;

    /// <summary>
    ///     Gets or sets the token size.
    /// </summary>
    public int TokenSizeInBytes { get; set; } = 64;

    /// <summary>
    ///     Whether it should use refresh tokens. Default value is
    ///     <see langword="true"/>.
    /// </summary>
    public bool UseRefreshToken { get; set; } = true;
}