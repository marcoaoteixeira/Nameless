using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Mediator.Events;
using Nameless.Mediator.Events.Fixtures;
using Nameless.Mediator.Fixtures;
using Nameless.Mediator.Requests;
using Nameless.Mediator.Requests.Fixtures;
using Nameless.Mediator.Streams;
using Nameless.Mediator.Streams.Fixtures;

namespace Nameless.Mediator;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public async Task WhenRegisteringMediatorServices_ThenResolveServices() {
        // arrange
        var services = new ServiceCollection();
        services.TryAddSingleton((ILogger)NullLogger.Instance);
        services.TryAddSingleton(new PrintServiceMocker().Build());
        services.RegisterMediator(opts => {
            opts.Assemblies = [typeof(ServiceCollectionExtensionsTests).Assembly];

            opts
                .RegisterRequestPipelineBehavior(typeof(MessageRequestPipelineBehavior))
                .RegisterRequestPipelineBehavior(typeof(YetAnotherMessageRequestPipelineBehavior))
                .RegisterRequestPipelineBehavior(typeof(OpenGenericRequestPipelineBehavior<,>))
                .RegisterRequestPipelineBehavior(typeof(PerformanceRequestPipelineBehavior<,>));

            opts
                .RegisterStreamPipelineBehavior(typeof(MessageStreamPipelineBehavior))
                .RegisterStreamPipelineBehavior(typeof(YetAnotherMessageStreamPipelineBehavior));
        });
        using var provider = services.BuildServiceProvider();

        // act
        var mediator = provider.GetService<IMediator>();
        var eventHandlerInvoker = provider.GetService<IEventHandlerInvoker>();
        var requestHandlerInvoker = provider.GetService<IRequestHandlerInvoker>();
        var streamHandlerInvoker = provider.GetService<IStreamHandlerInvoker>();

        var messageEventHandlers = provider.GetServices<IEventHandler<MessageEvent>>().ToArray();
        var multipleMessageEventHandler = provider.GetService<IEventHandler<MessageOneEvent>>();

        var messageRequestHandlers = provider.GetServices<IRequestHandler<MessageRequest, MessageResponse>>().ToArray();
        var messageRequestPipelineBehaviors =
            provider.GetServices<IRequestPipelineBehavior<MessageRequest, MessageResponse>>().ToArray();
        var openGenericRequestPipelineBehavior =
            provider.GetService<IRequestPipelineBehavior<MessageEvent, MessageEvent>>();

        var messageStreamHandlers = provider.GetServices<IStreamHandler<MessageStream, string>>().ToArray();
        var messageStreamPipelineBehaviors =
            provider.GetServices<IStreamPipelineBehavior<MessageStream, string>>().ToArray();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(mediator);
            Assert.NotNull(eventHandlerInvoker);
            Assert.NotNull(requestHandlerInvoker);
            Assert.NotNull(streamHandlerInvoker);

            Assert.NotEmpty(messageEventHandlers);
            Assert.Equal(expected: 2, messageEventHandlers.Length);
            Assert.NotNull(multipleMessageEventHandler);

            Assert.NotEmpty(messageRequestHandlers);
            Assert.Single(messageRequestHandlers);
            Assert.NotEmpty(messageRequestPipelineBehaviors);
            Assert.Equal(expected: 4, messageRequestPipelineBehaviors.Length);
            Assert.NotNull(openGenericRequestPipelineBehavior);

            Assert.NotEmpty(messageStreamHandlers);
            Assert.Single(messageStreamHandlers);
            Assert.NotEmpty(messageStreamPipelineBehaviors);
            Assert.Equal(expected: 2, messageStreamPipelineBehaviors.Length);
        });
    }
}