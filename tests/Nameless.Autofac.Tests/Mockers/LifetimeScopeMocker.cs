﻿using Autofac;
using Autofac.Core;
using Moq;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Autofac.Mockers;

public sealed class LifetimeScopeMocker : MockerBase<ILifetimeScope> {
    public LifetimeScopeMocker() {
        var componentRegistryMock = new Mock<IComponentRegistry>();
        var serviceRegistration = new ServiceRegistration();

        componentRegistryMock
           .Setup(mock => mock.TryGetServiceRegistration(It.IsAny<Service>(), out serviceRegistration))
           .Returns(true);

        MockInstance.Setup(mock => mock.ComponentRegistry)
                    .Returns(componentRegistryMock.Object);
    }

    public LifetimeScopeMocker WithResolve<TComponent>(TComponent component) {
        var service = new TypedService(typeof(TComponent));
        var serviceRegistration = new ServiceRegistration();
        var parameters = Enumerable.Empty<Parameter>();
        var resolveRequest = new ResolveRequest(service, serviceRegistration, parameters);

        MockInstance.Setup(mock => mock.ResolveComponent(resolveRequest))
                    .Returns(component);

        return this;
    }
}