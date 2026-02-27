using Microsoft.Extensions.Hosting;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Autofac.Mockers;

public sealed class HostApplicationLifetimeMocker : Mocker<IHostApplicationLifetime> {
    public HostApplicationLifetimeMocker WithApplicationStopped(CancellationToken returnValue) {
        MockInstance.Setup(mock => mock.ApplicationStopped)
                    .Returns(returnValue);

        return this;
    }
}