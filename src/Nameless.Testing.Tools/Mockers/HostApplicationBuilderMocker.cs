using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.Testing.Tools.Mockers;

public sealed class HostApplicationBuilderMocker : MockerBase<IHostApplicationBuilder> {
    public HostApplicationBuilderMocker WithServices(IServiceCollection services) {
        MockInstance.Setup(mock => mock.Services)
                    .Returns(services);

        return this;
    }

    public HostApplicationBuilderMocker WithConfigurationManager(IConfigurationManager configurationManager) {
        MockInstance.Setup(mock => mock.Configuration)
                    .Returns(configurationManager);

        return this;
    }

    public HostApplicationBuilderMocker WithEnvironment(IHostEnvironment hostEnvironment) {
        MockInstance.Setup(mock => mock.Environment)
                    .Returns(hostEnvironment);

        return this;
    }
}