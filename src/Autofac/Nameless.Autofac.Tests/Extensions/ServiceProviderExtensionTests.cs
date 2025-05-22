using Autofac.Extensions.DependencyInjection;
using Moq;
using Nameless.Autofac.Mockers;

namespace Nameless.Autofac;

public class ServiceProviderExtensionTests {
    [Test]
    public void WhenRegisterAutofacDisposeHandler_ThenHostApplicationMustRegisterDisposeHandler() {
        // arrange
        var cts = new CancellationTokenSource();
        var hostApplicationLifetime = new HostApplicationLifetimeMocker()
            .WithApplicationStopped(cts.Token)
            .Build();

        var lifetimeScopeMocker = new LifetimeScopeMocker()
            .WithResolve(hostApplicationLifetime);

        var serviceProvider = new AutofacServiceProvider(lifetimeScopeMocker.Build());

        // act
        ServiceProviderExtensions.RegisterAutofacDisposeHandler(serviceProvider);
        cts.Cancel();

        // assert
        lifetimeScopeMocker.Verify(mock => mock.Dispose(), Times.Once());
    }

    [Test]
    public void WhenRegisterAutofacDisposeHandler_WhenHostApplicationLifetimeNull_ThenDoNothing() {
        // arrange
        var cts = new CancellationTokenSource();
        var lifetimeScopeMocker = new LifetimeScopeMocker();

        var serviceProvider = new AutofacServiceProvider(lifetimeScopeMocker.Build());

        // act
        ServiceProviderExtensions.RegisterAutofacDisposeHandler(serviceProvider);
        cts.Cancel();

        // assert
        lifetimeScopeMocker.Verify(mock => mock.Dispose(), Times.Never());
    }
}