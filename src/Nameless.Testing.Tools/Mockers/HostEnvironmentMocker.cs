using Microsoft.Extensions.Hosting;

namespace Nameless.Testing.Tools.Mockers;

public sealed class HostEnvironmentMocker : MockerBase<IHostEnvironment> {
    public HostEnvironmentMocker WithEnvironmentName(string environmentName) {
        MockInstance.Setup(mock => mock.EnvironmentName)
                    .Returns(environmentName);

        return this;
    }
}