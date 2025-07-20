using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.Web.IdentityModel;

/// <summary>
/// 
/// </summary>
public static class HostApplicationBuilderExtensions {
    public static THostApplicationBuilder RegisterJsonWebTokenProvider<THostApplicationBuilder>(this THostApplicationBuilder self, Action<JsonWebTokenOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services
            .Configure(configure ?? (_ => { }))
            .AddTransient<IJsonWebTokenProvider, JsonWebTokenProvider>();

        return self;
    }
}
