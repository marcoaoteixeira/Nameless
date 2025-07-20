using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.Web.Identity.Security;

/// <summary>
/// 
/// </summary>
public static class HostApplicationBuilderExtensions {
    public static THostApplicationBuilder RegisterUserRefreshTokenServices<THostApplicationBuilder>(this THostApplicationBuilder self, Action<UserRefreshTokenOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services
            .Configure(configure ?? (_ => { }))
            .AddTransient<IUserRefreshTokenService, UserRefreshTokenService>();

        return self;
    }
}
