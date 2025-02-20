using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.Mockers;

public class HostApplicationBuilderMocker : MockerBase<IHostApplicationBuilder> {
    public HostApplicationBuilderMocker WithServices(IServiceCollection services) {
        Mock.Setup(mock => mock.Services)
                    .Returns(services);

        return this;
    }

    public HostApplicationBuilderMocker WithConfigurationManager(IConfigurationManager configurationManager) {
        Mock.Setup(mock => mock.Configuration)
                    .Returns(configurationManager);

        return this;
    }

    public HostApplicationBuilderMocker WithEnvironment(IHostEnvironment hostEnvironment) {
        Mock.Setup(mock => mock.Environment)
                    .Returns(hostEnvironment);

        return this;
    }
}
