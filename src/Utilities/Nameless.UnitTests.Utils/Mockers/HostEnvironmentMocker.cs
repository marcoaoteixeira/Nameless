using Microsoft.Extensions.Hosting;

namespace Nameless.Mockers;

public class HostEnvironmentMocker : MockerBase<IHostEnvironment> {
    public HostEnvironmentMocker WithEnvironmentName(string environmentName) {
        Mock.Setup(mock => mock.EnvironmentName)
                    .Returns(environmentName);

        return this;
    }
}
