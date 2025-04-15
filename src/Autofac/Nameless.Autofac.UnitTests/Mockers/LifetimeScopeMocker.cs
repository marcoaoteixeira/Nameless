using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Hosting;
using Moq;
using Nameless.Mockers;

namespace Nameless.Autofac.Mockers;

public class LifetimeScopeMocker : MockerBase<ILifetimeScope> {
    public LifetimeScopeMocker() {
        var componentRegistryMock = new Mock<IComponentRegistry>();
        var serviceRegistration = new ServiceRegistration();

        componentRegistryMock.Setup(mock => mock.TryGetServiceRegistration(It.IsAny<Service>(), out serviceRegistration))
                             .Returns(true);

        Mock.Setup(mock => mock.ComponentRegistry)
            .Returns(componentRegistryMock.Object);
    }

    public LifetimeScopeMocker WithResolve<TComponent>(TComponent component) {
        var service = new TypedService(typeof(TComponent));
        var serviceRegistration = new ServiceRegistration();
        var parameters = Enumerable.Empty<Parameter>();
        var resolveRequest = new ResolveRequest(service, serviceRegistration, parameters);

        Mock.Setup(mock => mock.ResolveComponent(resolveRequest))
            .Returns(component);

        return this;
    }
}

public class HostApplicationLifetimeMocker : MockerBase<IHostApplicationLifetime> {
    public HostApplicationLifetimeMocker WithApplicationStopped(CancellationToken cancellationToken) {
        Mock.Setup(mock => mock.ApplicationStopped)
            .Returns(cancellationToken);

        return this;
    }
}