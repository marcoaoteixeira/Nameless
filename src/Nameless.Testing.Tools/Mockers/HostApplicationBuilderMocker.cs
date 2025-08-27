using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.Testing.Tools.Mockers;

public sealed class HostApplicationBuilderMocker : Mocker<IHostApplicationBuilder> {
    public HostApplicationBuilderMocker WithServices(IServiceCollection returnValue) {
        MockInstance.Setup(mock => mock.Services)
                    .Returns(returnValue);

        return this;
    }

    public HostApplicationBuilderMocker WithConfiguration(IConfigurationManager returnValue) {
        MockInstance.Setup(mock => mock.Configuration)
                    .Returns(returnValue);

        return this;
    }

    public HostApplicationBuilderMocker WithEnvironment(IHostEnvironment returnValue) {
        MockInstance.Setup(mock => mock.Environment)
                    .Returns(returnValue);

        return this;
    }
}