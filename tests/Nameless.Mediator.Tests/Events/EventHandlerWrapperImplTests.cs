using Moq;
using Nameless.Mediator.Events.Fixtures;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Mediator.Events;
public class EventHandlerWrapperImplTests {
    [Fact]
    public async Task WhenHandle_WhenThereAreEventHandlersRegistered_ThenExecuteEventHandlers() {
        // arrange
        var loggerMocker = new LoggerMocker<object>();

        var evt = new SimpleEvent { Message = nameof(EventHandlerWrapperImplTests) };
        var eventHandler = new SimpleEventHandler(loggerMocker.Build());

        var serviceProvider = new ServiceProviderMocker().WithGetService<IEnumerable<IEventHandler<SimpleEvent>>>([eventHandler])
                                                         .Build();
        var sut = new EventHandlerWrapperImpl<SimpleEvent>();

        // act
        await sut.HandleAsync(evt, serviceProvider, CancellationToken.None);

        // assert
        loggerMocker.VerifyDebugCall(message => message.Contains(evt.Message), Times.Once());
    }

    [Fact]
    public async Task WhenHandle_WhenThereAreNoEventHandlersRegistered_ThenLogMissingEventHandlers() {
        // arrange
        var loggerMocker = new LoggerMocker<EventHandlerWrapper>().WithAnyLogLevel();
        var loggerFactory = new LoggerFactoryMocker().WithCreateLogger(loggerMocker.Build())
                                                     .Build();

        var evt = new SimpleEvent { Message = nameof(EventHandlerWrapperImplTests) };
        var serviceProvider = new ServiceProviderMocker().WithGetService(loggerFactory)
                                                         .WithGetService<IEnumerable<IEventHandler<SimpleEvent>>>([])
                                                         .Build();

        var sut = new EventHandlerWrapperImpl<SimpleEvent>();

        // act
        await sut.HandleAsync(evt, serviceProvider, CancellationToken.None);

        // assert
        loggerMocker.VerifyDebugCall(message => message.Contains("Event handler not found"), Times.Once());
    }
}
