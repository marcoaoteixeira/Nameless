using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Web.Identity.Entities;
using Nameless.Web.Identity.Internals;
using Nameless.Web.Identity.Security;

namespace Nameless.Web.Identity.Domains.UserRefreshTokens;

public class UserRefreshTokenManager : IUserRefreshTokenManager {
    private readonly DbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TimeProvider _timeProvider;
    private readonly IOptions<RefreshTokenOptions> _options;
    private readonly ILogger<UserRefreshTokenManager> _logger;

    public UserRefreshTokenManager(DbContext dbContext, IHttpContextAccessor httpContextAccessor, TimeProvider timeProvider, IOptions<RefreshTokenOptions> options, ILogger<UserRefreshTokenManager> logger) {
        _dbContext = Guard.Against.Null(dbContext);
        _httpContextAccessor = Guard.Against.Null(httpContextAccessor);
        _timeProvider = Guard.Against.Null(timeProvider);
        _options = Guard.Against.Null(options);
        _logger = Guard.Against.Null(logger);
    }

    public async Task<UserRefreshToken> CreateAsync(User user, CancellationToken cancellationToken) {
        var options = _options.Value;
        var now = _timeProvider.GetUtcNow();
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(options.TokenSizeInBytes));
        var newRefreshToken = new UserRefreshToken {
            Id = Guid.CreateVersion7(now),
            UserId = user.Id,
            Token = token,
            ExpiresAt = now.Add(options.TokenExpiresIn),
            CreatedAt = now,
            CreatedByIp = _httpContextAccessor.HttpContext?.GetIPv4()
        };

        try {
            var entityEntry = await _dbContext.Set<UserRefreshToken>()
                                              .AddAsync(newRefreshToken, cancellationToken)
                                              .ConfigureAwait(continueOnCapturedContext: false);

            await _dbContext.SaveChangesAsync(cancellationToken)
                            .ConfigureAwait(continueOnCapturedContext: false);

            return entityEntry.Entity;
        }
        catch (Exception ex) {
            _logger.CreateUserRefreshTokenFailure(ex);

            throw;
        }
    }

    public async Task<int> CleanUpAsync(User user, CancellationToken cancellationToken) {
        var options = _options.Value;
        var userRefreshTokenCount = await _dbContext.Set<UserRefreshToken>()
                                                    .CountAsync(userRefreshToken => userRefreshToken.UserId == user.Id, cancellationToken)
                                                    .ConfigureAwait(continueOnCapturedContext: false);

        if (options.TokenRetentionLimit > userRefreshTokenCount) {
            return 0;
        }

        try {
            return await _dbContext.Set<UserRefreshToken>()
                                   .Where(item => item.UserId == user.Id)
                                   .OrderByDescending(item => item.CreatedAt)
                                   .Skip(options.TokenRetentionLimit)
                                   .ExecuteDeleteAsync(cancellationToken)
                                   .ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (Exception ex) {
            _logger.CleanUpUserRefreshTokensFailure(ex);

            throw;
        }
    }

    public async Task<int> RevokeAsync(User user, RevokeUserRefreshTokenMetadata metadata, CancellationToken cancellationToken) {
        try {
            var now = _timeProvider.GetUtcNow();
            var expiresAt = metadata.ExpiresAt.GetValueOrDefault(now);

            return await _dbContext.Set<UserRefreshToken>()
                                   .Where(userRefreshToken =>
                                       userRefreshToken.UserId == user.Id &&
                                       userRefreshToken.RevokedAt == null &&
                                       userRefreshToken.ExpiresAt <= expiresAt)
                                   .ExecuteUpdateAsync(update =>
                                           update.SetProperty(userRefreshToken => userRefreshToken.RevokedAt, now)
                                                 .SetProperty(userRefreshToken => userRefreshToken.RevokedByIp, metadata.RevokedByIp)
                                                 .SetProperty(userRefreshToken => userRefreshToken.RevokeReason, metadata.RevokeReason)
                                                 .SetProperty(userRefreshToken => userRefreshToken.ReplacedByToken, metadata.ReplacedByToken)
                                     , cancellationToken)
                                   .ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (Exception ex) {
            _logger.RevokeUserRefreshTokensFailure(ex);

            throw;
        }
    }

    public Task<UserRefreshToken?> GetAsync(string token, CancellationToken cancellationToken) {
        Guard.Against.NullOrWhiteSpace(token);

        return _dbContext.Set<UserRefreshToken>()
                         .AsNoTrackingWithIdentityResolution()
                         .SingleOrDefaultAsync(item => item.Token == token, cancellationToken);
    }
}