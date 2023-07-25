using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Nameless.AspNetCore.Services;

namespace Nameless.AspNetCore.Middlewares {
    public sealed class JwtAuthorizationMiddleware {
        #region Private Read-Only Fields

        private readonly RequestDelegate _next;
        private readonly IJwtService _jwtService;

        #endregion

        #region Public Constructors

        public JwtAuthorizationMiddleware(RequestDelegate next, IJwtService jwtService) {
            _next = Prevent.Against.Null(next, nameof(next));
            _jwtService = Prevent.Against.Null(jwtService, nameof(jwtService));
        }

        #endregion

        #region Public Methods

        public async Task InvokeAsync(HttpContext context) {
            var key = HttpRequestHeader.Authorization.ToString();
            var header = context.Request.Headers[key].FirstOrDefault();

            if (header != null) {
                var token = header.Split(Constants.Separators.SPACE).Last();
                if (_jwtService.Validate(token, out var principal)) {
                    context.User = principal;
                }
            }

            await _next(context);
        }

        #endregion
    }

    public static class JwtAuthorizationMiddlewareExtension {
        #region Public Static Methods

        public static IApplicationBuilder UseJwtAuthorization(this IApplicationBuilder self)
            => self.UseMiddleware<JwtAuthorizationMiddleware>();

        #endregion
    }
}
