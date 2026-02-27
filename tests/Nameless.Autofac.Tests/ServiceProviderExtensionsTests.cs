using Autofac.Extensions.DependencyInjection;
using Moq;
using Nameless.Autofac.Mockers;
using Xunit;

namespace Nameless.Autofac;

public class ServiceProviderExtensionsTests {
    [Fact]
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
        serviceProvider.UseAutofacDisposeHandler();
        cts.Cancel();

        // assert
        lifetimeScopeMocker.Verify(mock => mock.Dispose());
    }

    [Fact]
    public void WhenRegisterAutofacDisposeHandler_WhenHostApplicationLifetimeNull_ThenDoNothing() {
        // arrange
        var cts = new CancellationTokenSource();
        var lifetimeScopeMocker = new LifetimeScopeMocker();

        var serviceProvider = new AutofacServiceProvider(lifetimeScopeMocker.Build());

        // act
        serviceProvider.UseAutofacDisposeHandler();
        cts.Cancel();

        // assert
        lifetimeScopeMocker.Verify(mock => mock.Dispose());
    }
}