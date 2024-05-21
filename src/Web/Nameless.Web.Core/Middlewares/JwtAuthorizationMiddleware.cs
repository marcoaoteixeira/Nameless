using System.Net;
using Microsoft.AspNetCore.Http;
using Nameless.Web.Services;
using CoreRoot = Nameless.Root;

namespace Nameless.Web.Middlewares {
    public sealed class JwtAuthorizationMiddleware {
        #region Private Read-Only Fields

        private readonly RequestDelegate _next;
        private readonly IJwtService _jwtService;

        #endregion

        #region Public Constructors

        public JwtAuthorizationMiddleware(RequestDelegate next, IJwtService jwtService) {
            _next = Guard.Against.Null(next, nameof(next));
            _jwtService = Guard.Against.Null(jwtService, nameof(jwtService));
        }

        #endregion

        #region Public Methods

        public async Task InvokeAsync(HttpContext context) {
            const string key = nameof(HttpRequestHeader.Authorization);
            var header = context.Request.Headers[key].FirstOrDefault();

            if (header is not null) {
                var token = header.Split(CoreRoot.Separators.SPACE).Last();
                if (_jwtService.TryValidate(token, out var principal)) {
                    context.User = principal;
                }
            }

            await _next(context);
        }

        #endregion
    }
}
