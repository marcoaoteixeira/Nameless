using Nameless.Mediator.Events.Fixtures;
using Nameless.Mediator.Fixtures;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Mediator.Events;

[UnitTest]
public class EventHandlerInvokerTests {
    [Fact]
    public async Task WhenPublishingEvent_ThenExecutesEventHandler() {
        // arrange
        var printServiceMock = new PrintServiceMocker();
        var evt = new MessageEvent();
        var evtHandler = new MessageEventHandler(printServiceMock.Build());
        var provider = new ServiceProviderMocker().WithGetService<IEnumerable<IEventHandler<MessageEvent>>>([evtHandler])
                                                  .Build();
        var sut = new EventHandlerInvoker(provider);

        // act
        await sut.PublishAsync(evt, CancellationToken.None);

        // assert
        printServiceMock.VerifyPrintCall();
    }

    [Fact]
    public async Task WhenPublishingEvent_WhenThereAreMultipleEventHandlerForEvent_ThenExecutesAllEventHandlers() {
        // arrange
        var printServiceMock = new PrintServiceMocker();
        var evt = new MessageEvent();
        var printService = printServiceMock.Build();
        var messageEventHandler = new MessageEventHandler(printService);
        var yetAnotherMessageEventHandler = new YetAnotherMessageEventHandler(printService);
        IEventHandler<MessageEvent>[] eventHandlers = [
            messageEventHandler,
            yetAnotherMessageEventHandler
        ];
        var provider = new ServiceProviderMocker().WithGetService<IEnumerable<IEventHandler<MessageEvent>>>(eventHandlers)
                                                  .Build();
        var sut = new EventHandlerInvoker(provider);

        // act
        await sut.PublishAsync(evt, CancellationToken.None);

        // assert
        printServiceMock.VerifyPrintCall(times: 2);
    }
}
