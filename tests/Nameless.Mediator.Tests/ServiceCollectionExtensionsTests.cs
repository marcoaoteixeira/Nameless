using Microsoft.Extensions.DependencyInjection;
using Nameless.Mediator.Events;
using Nameless.Mediator.Requests;
using Nameless.Mediator.Streams;

namespace Nameless.Mediator;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public void WhenRegisteringMediatorServices_ThenResolveMainServices() {
        // arrange
        // arrange
        var services = new ServiceCollection();
        services.RegisterMediator(configure: _ => { });
        using var provider = services.BuildServiceProvider();

        // act
        var mediator = provider.GetService<IMediator>();
        var eventHandlerInvoker = provider.GetService<IEventHandlerInvoker>();
        var requestHandlerInvoker = provider.GetService<IRequestHandlerInvoker>();
        var streamHandlerInvoker = provider.GetService<IStreamHandlerInvoker>();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(mediator);
            Assert.NotNull(eventHandlerInvoker);
            Assert.NotNull(requestHandlerInvoker);
            Assert.NotNull(streamHandlerInvoker);
        });
    }

    [Fact]
    public void WhenRegisteringMediatorServices_WhenConfigureActionIsNull_ThenResolveMainServices() {
        // arrange
        // arrange
        var services = new ServiceCollection();
        services.RegisterMediator(configure: null);
        using var provider = services.BuildServiceProvider();

        // act
        var mediator = provider.GetService<IMediator>();
        var eventHandlerInvoker = provider.GetService<IEventHandlerInvoker>();
        var requestHandlerInvoker = provider.GetService<IRequestHandlerInvoker>();
        var streamHandlerInvoker = provider.GetService<IStreamHandlerInvoker>();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(mediator);
            Assert.NotNull(eventHandlerInvoker);
            Assert.NotNull(requestHandlerInvoker);
            Assert.NotNull(streamHandlerInvoker);
        });
    }
}