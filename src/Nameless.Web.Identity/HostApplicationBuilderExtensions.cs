using Microsoft.Extensions.Hosting;

namespace Nameless.Web.Identity;

public static class HostApplicationBuilderExtensions {
    public static THostApplicationBuilder RegisterIdentityServices<THostApplicationBuilder>(this THostApplicationBuilder self)
        where THostApplicationBuilder : IHostApplicationBuilder {

    }
}
