using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Hosting;
using Nameless.Hosting.Features.Mediator;
using Nameless.Testing.Tools.Mockers.Hosting;

namespace Nameless.Mediator;

// Test-only shim: registration moved from a ServiceCollection extension
// method to MediatorHostFeature. Keeps the existing RegisterMediator(...)
// call sites working against the host feature instead.
internal static class MediatorHostFeatureTestExtensions {
    public static IServiceCollection RegisterMediator(this IServiceCollection services, Action<MediatorFeatureOptions> configure) {
        var builder = new HostApplicationBuilderMocker().WithServices(services)
                                                         .Build();
        var feature = new MediatorHostFeature();

        feature.Configure(configure);
        feature.Register(builder, new HostFeatureContext([], NullLogger.Instance));

        return services;
    }
}
