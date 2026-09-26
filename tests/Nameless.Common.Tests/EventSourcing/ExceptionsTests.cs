namespace Nameless.EventSourcing;

[UnitTest]
public class ExceptionsTests {
    [Fact]
    public void ConcurrencyConflictException_SetsPropertiesAndMessage() {
        // arrange & act
        var sut = new ConcurrencyConflictException("stream-1", expectedVersion: 3);

        // assert
        Assert.Multiple(
            () => Assert.Equal("stream-1", sut.StreamID),
            () => Assert.Equal(3, sut.ExpectedVersion),
            () => Assert.Contains("stream-1", sut.Message),
            () => Assert.Contains("3", sut.Message)
        );
    }

    [Fact]
    public void UnauthorizedActionException_SetsPropertiesAndMessage() {
        // arrange
        var actor = new ActorContext(Guid.NewGuid());
        var command = new object();

        // act
        var sut = new UnauthorizedActionException(actor, command);

        // assert
        Assert.Multiple(
            () => Assert.Same(actor, sut.Actor),
            () => Assert.Same(command, sut.Command),
            () => Assert.Contains(actor.UserID.ToString(), sut.Message)
        );
    }
}
