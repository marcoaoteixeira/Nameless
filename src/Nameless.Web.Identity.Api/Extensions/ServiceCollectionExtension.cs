using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Web.Identity.Api.Handlers;
using Nameless.Web.Identity.Api.Requests;
using Nameless.Web.Identity.Api.Responses;

namespace Nameless.Web.Identity.Api;

public static class ServiceCollectionExtension {
    public static IServiceCollection RegisterIdentityApi(this IServiceCollection self, Action<IdentityApiOptions>? configure = null)
        => RegisterIdentityApi<IdentityUser, string>(self, configure);

    public static IServiceCollection RegisterIdentityApi<TUser, TKey>(this IServiceCollection self, Action<IdentityApiOptions>? configure = null)
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey> {

        ConfigureOptions(self, configure);
            
        RegisterRequestHandlers<TUser, TKey>(self);

        return self.AddMinimalEndpoints([typeof(ServiceCollectionExtension).Assembly]);
    }

    private static void ConfigureOptions(IServiceCollection services, Action<IdentityApiOptions>? configure) {
        var options = new IdentityApiOptions();

        (configure ?? (_ => { })).Invoke(options);
        services.AddSingleton(options);
    }

    private static void RegisterRequestHandlers<TUser, TKey>(IServiceCollection services)
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey> {
        services.AddTransient<IRequestHandler<AuthenticateUserRequest, AuthenticateUserResponse>, AuthenticateUserRequestHandler<TUser, TKey>>();
    }
}