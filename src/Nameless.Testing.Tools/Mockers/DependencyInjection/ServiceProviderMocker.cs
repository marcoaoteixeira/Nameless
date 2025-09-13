namespace Nameless.Testing.Tools.Mockers.DependencyInjection;

public sealed class ServiceProviderMocker : Mocker<IServiceProvider> {
    public ServiceProviderMocker WithGetService(Type serviceType, object returnValue) {
        MockInstance
           .Setup(mock => mock.GetService(serviceType))
           .Returns(returnValue);

        return this;
    }

    public ServiceProviderMocker WithGetService<TService>(TService returnValue) {
        MockInstance
           .Setup(mock => mock.GetService(typeof(TService)))
           .Returns(returnValue);

        return this;
    }
}