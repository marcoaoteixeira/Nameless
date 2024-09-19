using System.Net;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Auth;

public sealed class JwtAuthorizationMiddleware {
    private readonly RequestDelegate _next;
    private readonly IJwtService _jwtService;

    // ReSharper disable once ConvertToPrimaryConstructor
    public JwtAuthorizationMiddleware(RequestDelegate next, IJwtService jwtService) {
        _next = Prevent.Argument.Null(next);
        _jwtService = Prevent.Argument.Null(jwtService);
    }

    public async Task InvokeAsync(HttpContext context) {
        const string key = nameof(HttpRequestHeader.Authorization);
        var header = context.Request.Headers[key].FirstOrDefault();

        if (header is not null) {
            var token = header.Split(Nameless.Root.Separators.SPACE).Last();
            if (_jwtService.TryValidate(token, out var principal)) {
                context.User = principal;
            }
        }

        await _next(context);
    }
}