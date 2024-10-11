using System.Net;
using Microsoft.AspNetCore.Http;
using Nameless.Web.Services;

namespace Nameless.Web.Infrastructure;

/// <summary>
/// JSON Web Token authorization middleware.
/// </summary>
public sealed class JwtAuthorizationMiddleware {
    private readonly IJwtService _jwtService;
    private readonly RequestDelegate _next;

    // ReSharper disable once ConvertToPrimaryConstructor
    public JwtAuthorizationMiddleware(RequestDelegate next, IJwtService jwtService) {
        _next = Prevent.Argument.Null(next);
        _jwtService = Prevent.Argument.Null(jwtService);
    }

    public async Task InvokeAsync(HttpContext context) {
        const string key = nameof(HttpRequestHeader.Authorization);
        var header = context.Request.Headers[key].FirstOrDefault();

        if (header is not null) {
            var token = header.Split(Constants.Separators.SPACE).Last();
            if (_jwtService.TryValidate(token, out var principal)) {
                context.User = principal;
            }
        }

        await _next(context);
    }
}