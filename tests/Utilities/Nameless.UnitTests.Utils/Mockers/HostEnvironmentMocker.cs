using Microsoft.Extensions.Hosting;

namespace Nameless.Mockers;

public class HostEnvironmentMocker : MockerBase<IHostEnvironment> {
    public HostEnvironmentMocker WithEnvironmentName(string environmentName) {
        InnerMock.Setup(mock => mock.EnvironmentName)
                    .Returns(environmentName);

        return this;
    }
}
