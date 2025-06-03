using Microsoft.Extensions.DependencyInjection;
using Nameless.Security.Crypto;

namespace Nameless.Security;

public static class ServiceCollectionExtensions {
    public static IServiceCollection RegisterSecurityServices(this IServiceCollection self, Action<SecurityOptions>? configure = null) {
        return self.Configure(configure ?? (_ => { }))
                   .RegisterMainServices();
    }

    private static IServiceCollection RegisterMainServices(this IServiceCollection self) {
        return self.AddScoped<IPasswordGenerator, RandomPasswordGenerator>()
                   .AddScoped<ICryptographicService, RijndaelCryptographicService>();
    }
}