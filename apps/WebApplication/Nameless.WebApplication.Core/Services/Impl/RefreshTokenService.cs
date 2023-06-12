using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nameless.AspNetCore;
using Nameless.Services;
using Nameless.Services.Impl;
using Nameless.WebApplication.Entities;
using Nameless.WebApplication.Options;

namespace Nameless.WebApplication.Services.Impl
{

    public sealed class RefreshTokenService : IRefreshTokenService {

        #region Private Read-Only Fields

        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonWebTokenOptions _options;

        #endregion

        #region Public Properties

        private IClock _clock = default!;
        public IClock Clock {
            get { return _clock ??= Nameless.Services.Impl.DefaultClock.Instance; }
            set { _clock = value ?? Nameless.Services.Impl.DefaultClock.Instance; }
        }

        #endregion

        #region Public Constructors

        public RefreshTokenService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IOptions<JsonWebTokenOptions> options) {
            Prevent.Null(dbContext, nameof(dbContext));
            Prevent.Null(httpContextAccessor, nameof(httpContextAccessor));
            Prevent.Null(options, nameof(options));

            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _options = options.Value ?? JsonWebTokenOptions.Default;
        }

        #endregion

        #region Private Static Methods

        private static string GenerateToken() {
            var buffer = RandomNumberGenerator.GetBytes(256).Concat(Guid.NewGuid().ToByteArray()).ToArray();
            return Convert.ToBase64String(buffer);
        }

        #endregion

        #region Private Methods

        private Task<UserRefreshToken?> GetFirstUsefulRefreshToken(Guid userId, CancellationToken cancellationToken) {
            var now = Clock.UtcNow;

            return _dbContext
                .UserRefreshTokens
                .OrderByDescending(_ => _.CreatedIn)
                .FirstOrDefaultAsync(_ =>
                    _.UserId == userId &&
                    _.RevokedIn == default &&
                    _.ExpiresIn > now
                , cancellationToken);
        }

        #endregion

        #region IUserRefreshTokenService Members

        public async Task<string> GenerateAsync(Guid userId, CancellationToken cancellationToken = default) {
            var current = await GetFirstUsefulRefreshToken(userId, cancellationToken);
            if (current != default) { return current.Token; }

            var now = Clock.UtcNow;
            var userRefreshToken = new UserRefreshToken {
                Id = Guid.NewGuid(),
                CreatedByIp = _httpContextAccessor.HttpContext.GetIpAddress(),
                CreatedIn = now,
                ExpiresIn = now.AddSeconds(_options.RefreshTokenTtl),
                Token = GenerateToken(),
                UserId = userId
            };

            await _dbContext.UserRefreshTokens.AddAsync(userRefreshToken, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await SanitizeAsync(userId, cancellationToken);

            return userRefreshToken.Token;
        }

        public async Task SanitizeAsync(Guid userId, CancellationToken cancellationToken = default) {
            var now = Clock.UtcNow;

            await _dbContext
                .UserRefreshTokens
                .Where(_ =>
                    _.UserId == userId && (
                        _.RevokedIn != default ||
                        _.ExpiresIn <= now
                    ))
                .ExecuteDeleteAsync(cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<string> ReplaceAsync(string previousRefreshToken, CancellationToken cancellationToken = default) {
            Prevent.NullOrWhiteSpaces(previousRefreshToken, nameof(previousRefreshToken));

            var now = Clock.UtcNow;
            var previousUserRefreshToken = await _dbContext
                .UserRefreshTokens
                .SingleOrDefaultAsync(_ =>_.Token == previousRefreshToken, cancellationToken);

            if (previousUserRefreshToken == default || !previousUserRefreshToken.IsActive(now)) {
                throw new SecurityTokenException("Refresh token invalid.");
            }

            // Creates the new refresh token
            var userId = previousUserRefreshToken.UserId;
            var newUserRefreshToken = new UserRefreshToken {
                Id = Guid.NewGuid(),
                CreatedByIp = _httpContextAccessor.HttpContext.GetIpAddress(),
                CreatedIn = now,
                ExpiresIn = now.AddSeconds(_options.RefreshTokenTtl),
                Token = GenerateToken(),
                UserId = userId
            };
            
            // Revokes the previous refresh token.
            previousUserRefreshToken.ReplacedByToken = newUserRefreshToken.Id;
            previousUserRefreshToken.RevokedByIp = _httpContextAccessor.HttpContext.GetIpAddress();
            previousUserRefreshToken.RevokeReason = "TOKEN REPLACEMENT";
            previousUserRefreshToken.RevokedIn = now;

            await _dbContext.UserRefreshTokens.AddAsync(newUserRefreshToken, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await SanitizeAsync(userId, cancellationToken);

            return newUserRefreshToken.Token;
        }

        public async Task RevokeAsync(string refreshToken, string? reason = null, CancellationToken cancellationToken = default) {
            Prevent.NullOrWhiteSpaces(refreshToken, nameof(refreshToken));

            var userRefreshToken = await _dbContext
                .UserRefreshTokens
                .SingleOrDefaultAsync(_ => _.Token == refreshToken, cancellationToken);

            if (userRefreshToken == default) { throw new SecurityTokenException("Refresh token invalid."); }
            if (userRefreshToken.IsRevoked()) { return; }

            var now = Clock.UtcNow;

            userRefreshToken.ReplacedByToken = Guid.Empty;
            userRefreshToken.RevokedByIp = _httpContextAccessor.HttpContext.GetIpAddress();
            userRefreshToken.RevokeReason = reason ?? "TOKEN REVOCATION";
            userRefreshToken.RevokedIn = now;

            await _dbContext.SaveChangesAsync(cancellationToken);

            await SanitizeAsync(userRefreshToken.UserId, cancellationToken);
        }

        #endregion
    }
}
