using Microsoft.Extensions.Hosting;

namespace Nameless.Testing.Tools.Mockers.Hosting;

public sealed class HostEnvironmentMocker : Mocker<IHostEnvironment> {
    public HostEnvironmentMocker WithEnvironmentName(string returnValue) {
        MockInstance.Setup(mock => mock.EnvironmentName)
                    .Returns(returnValue);

        return this;
    }
}