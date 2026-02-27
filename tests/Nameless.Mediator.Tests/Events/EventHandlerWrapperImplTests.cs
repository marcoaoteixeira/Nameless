using Nameless.Mediator.Events.Fixtures;
using Nameless.Mediator.Fixtures;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.DependencyInjection;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Mediator.Events;

[UnitTest]
public class EventHandlerWrapperImplTests {
    [Fact]
    public async Task WhenHandlingEvent_WhenThereAreEventHandlers_ThenExecuteEventHandlers() {
        // arrange
        var printServiceMocker = new PrintServiceMocker();
        var eventHandler = new MessageEventHandler(printServiceMocker.Build());
        var evt = new MessageEvent { Message = nameof(EventHandlerWrapperImplTests) };
        var serviceProvider = new ServiceProviderMocker()
                              .WithGetService<IEnumerable<IEventHandler<MessageEvent>>>([eventHandler])
                              .Build();
        var sut = new EventHandlerWrapperImpl<MessageEvent>();

        // act
        await sut.HandleAsync(evt, serviceProvider, CancellationToken.None);

        // assert
        printServiceMocker.VerifyPrintCall(message => message.Contains(evt.Message));
    }

    [Fact]
    public async Task WhenHandle_WhenThereAreNoEventHandlers_ThenLogMissingEventHandlers() {
        // arrange
        var loggerMocker = new LoggerMocker<EventHandlerWrapper>().WithAnyLogLevel();
        var loggerFactory = new LoggerFactoryMocker().WithCreateLogger(loggerMocker.Build())
                                                     .Build();

        var evt = new MessageEvent { Message = nameof(EventHandlerWrapperImplTests) };
        var serviceProvider = new ServiceProviderMocker().WithGetService(loggerFactory)
                                                         .WithGetService<IEnumerable<IEventHandler<MessageEvent>>>([])
                                                         .Build();

        var sut = new EventHandlerWrapperImpl<MessageEvent>();

        // act
        await sut.HandleAsync(evt, serviceProvider, CancellationToken.None);

        // assert
        loggerMocker.VerifyDebug(message => message.Contains(value: "Event handler not found"));
    }
}