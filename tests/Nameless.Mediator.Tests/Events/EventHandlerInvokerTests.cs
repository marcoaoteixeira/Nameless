using Moq;
using Nameless.Mediator.Events.Fixtures;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Mediator.Events;

public class EventHandlerInvokerTests {
    [Fact]
    public void WhenPublishingEvent_WhenEventHandlersAreRegistered_ThenPublishEvent() {
        // arrange
        var loggerMocker = new LoggerMocker<object>();
        var evt = new SimpleEvent { Message = "This is a test." };
        var eventHandler = new SimpleEventHandler(loggerMocker.Build());
        var provider = new ServiceProviderMocker().WithGetService<IEnumerable<IEventHandler<SimpleEvent>>>([eventHandler])
                                                  .Build();
        var sut = new EventHandlerInvoker(provider);

        // act
        sut.PublishAsync(evt, CancellationToken.None);

        // assert
        loggerMocker.VerifyDebugCall(message => message.Contains(evt.Message), Times.Once());
    }

    [Fact]
    public void WhenPublishingEvent_WhenThereAreMultipleEventHandlersRegistered_ThenPublishEventUsingAllHandlers() {
        // arrange
        var loggerMocker = new LoggerMocker<object>();
        var logger = loggerMocker.Build();
        var evt = new SimpleEvent { Message = "This is a test." };
        IEventHandler<SimpleEvent>[] eventHandlers = [
            new SimpleEventHandler(logger),
            new YetAnotherSimpleEventHandler(logger)
        ];
        var provider = new ServiceProviderMocker().WithGetService<IEnumerable<IEventHandler<SimpleEvent>>>(eventHandlers)
                                                  .Build();
        var sut = new EventHandlerInvoker(provider);

        // act
        sut.PublishAsync(evt, CancellationToken.None);

        // assert
        loggerMocker.VerifyDebugCall(message => message.Contains(evt.Message), Times.Exactly(2));
    }
}
