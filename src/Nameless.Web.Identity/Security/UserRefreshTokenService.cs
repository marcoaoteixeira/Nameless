using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nameless.EntityFrameworkCore;
using Nameless.Web.Identity.Entities;
using Nameless.Web.Identity.Infrastructure;

namespace Nameless.Web.Identity.Security;

/// <summary>
///     Default implementation of <see cref="IUserRefreshTokenService"/>.
/// </summary>
public class UserRefreshTokenService : IUserRefreshTokenService {
    private readonly IDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<UserRefreshTokenOptions> _options;

    private DbSet<User> Users => _dbContext.Set<User>();
    private DbSet<UserRefreshToken> UserRefreshTokens => _dbContext.Set<UserRefreshToken>();

    /// <summary>
    ///     Initializes a new instance of <see cref="UserRefreshTokenService"/> class.
    /// </summary>
    /// <param name="dbContext">
    ///     The DB context.
    /// </param>
    /// <param name="timeProvider">
    ///     The time provider.
    /// </param>
    /// <param name="httpContextAccessor">
    ///     The HTTP context accessor.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    public UserRefreshTokenService(IDbContext dbContext, TimeProvider timeProvider, IHttpContextAccessor httpContextAccessor, IOptions<UserRefreshTokenOptions> options) {
        _dbContext = Prevent.Argument.Null(dbContext);
        _timeProvider = Prevent.Argument.Null(timeProvider);
        _httpContextAccessor = Prevent.Argument.Null(httpContextAccessor);
        _options = Prevent.Argument.Null(options);
    }

    /// <inheritdoc />
    /// <exception cref="UserNotFoundException">
    ///     if the specified user is not found.
    /// </exception>
    public async Task<UserRefreshToken> CreateAsync(Guid userID, CancellationToken cancellationToken) {
        if (!_options.Value.UseRefreshToken) {
            return new UserRefreshToken();
        }

        Prevent.Argument.Default(userID);

        var userExists = await Users.AnyAsync(user => user.Id == userID, cancellationToken)
                                    .ConfigureAwait(continueOnCapturedContext: false);
        if (!userExists) {
            throw new UserNotFoundException();
        }

        var options = _options.Value;
        var now = _timeProvider.GetUtcNow().DateTime;
        var ip = _httpContextAccessor.HttpContext?.GetIPv4();
        var newRefreshTokenID = Guid.NewGuid();

        await RemovePreviousUserRefreshTokensAsync(userID, options, cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

        await RevokePreviousRefreshTokensAsync(userID, now, ip, newRefreshTokenID, cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

        var expiresAt = now.Add(options.TokenTtl);
        var userRefreshToken = new UserRefreshToken {
            Id = Guid.NewGuid(),
            UserId = userID,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(options.TokenSizeInBytes)),
            ExpiresAt = expiresAt,
            CreatedAt = now,
            CreatedByIp = _httpContextAccessor.HttpContext?.GetIPv4()
        };

        var entry = await UserRefreshTokens.AddAsync(userRefreshToken, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken)
                        .ConfigureAwait(continueOnCapturedContext: false);

        return entry.Entity;
    }

    public async Task RevokeAsync(string token, CancellationToken cancellationToken) {
        if (!_options.Value.UseRefreshToken) {
            return;
        }

        Prevent.Argument.NullOrWhiteSpace(token);

        var userRefreshToken = await UserRefreshTokens.SingleOrDefaultAsync(userRefreshToken => userRefreshToken.Token == token, cancellationToken)
                                                      .ConfigureAwait(continueOnCapturedContext: false);

        if (userRefreshToken is not null) {
            var username = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            userRefreshToken.RevokeReason = $"Revoked by user '{username}'";
            userRefreshToken.RevokedAt = _timeProvider.GetUtcNow().DateTime;
            userRefreshToken.RevokedByIp = _httpContextAccessor.HttpContext?.GetIPv4();

            await _dbContext.SaveChangesAsync(cancellationToken)
                            .ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    public async Task<bool> IsActiveAsync(string token, CancellationToken cancellationToken) {
        if (!_options.Value.UseRefreshToken) {
            return false;
        }

        Prevent.Argument.NullOrWhiteSpace(token);

        var userRefreshToken = await UserRefreshTokens.SingleOrDefaultAsync(item => item.Token == token, cancellationToken)
                                                      .ConfigureAwait(continueOnCapturedContext: false);

        return userRefreshToken?.IsActive(_timeProvider.GetUtcNow().DateTime) ?? false;
    }

    private async Task RevokePreviousRefreshTokensAsync(Guid userID, DateTime now, string? revokedByIp, Guid newRefreshTokenID, CancellationToken cancellationToken) {
        await UserRefreshTokens.Where(userRefreshToken =>
                                   userRefreshToken.UserId == userID &&
                                   userRefreshToken.RevokedAt == null &&
                                   userRefreshToken.ExpiresAt <= now)
                               .ExecuteUpdateAsync(update =>
                                       update.SetProperty(userRefreshToken => userRefreshToken.RevokedAt, now)
                                             .SetProperty(userRefreshToken => userRefreshToken.RevokedByIp, revokedByIp)
                                             .SetProperty(userRefreshToken => userRefreshToken.RevokeReason, "Housekeeping")
                                             .SetProperty(userRefreshToken => userRefreshToken.ReplacedByToken, newRefreshTokenID.ToString())
                                 , cancellationToken)
                               .ConfigureAwait(continueOnCapturedContext: false);
    }

    private async Task RemovePreviousUserRefreshTokensAsync(Guid userID, UserRefreshTokenOptions options, CancellationToken cancellationToken) {
        var userRefreshTokenCount = await UserRefreshTokens.CountAsync(userRefreshToken => userRefreshToken.UserId == userID, cancellationToken)
                                                           .ConfigureAwait(continueOnCapturedContext: false);

        if (options.TokenRetentionLimit > userRefreshTokenCount) {
            return;
        }

        await UserRefreshTokens.Where(item => item.UserId == userID)
                               .OrderByDescending(item => item.CreatedAt)
                               .Skip(options.TokenRetentionLimit)
                               .ExecuteDeleteAsync(cancellationToken)
                               .ConfigureAwait(continueOnCapturedContext: false);
    }
}