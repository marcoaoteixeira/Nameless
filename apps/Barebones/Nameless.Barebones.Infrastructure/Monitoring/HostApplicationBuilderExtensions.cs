using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.Barebones.Infrastructure.Monitoring;

public static class HostApplicationBuilderExtensions {
    public static THostApplicationBuilder RegisterActivitySourceManager<THostApplicationBuilder>(this THostApplicationBuilder self)
        where THostApplicationBuilder : IHostApplicationBuilder {

        self.Services
            .AddSingleton<IActivitySourceManager, ActivitySourceManager>();

        return self;
    }
}