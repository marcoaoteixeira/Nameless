using Moq;
using Nameless.EventSourcing.UpCasting;

namespace Nameless.EventSourcing.Projections;

[UnitTest]
public class ProjectionRebuilderTests {
    private static EventEnvelope CreateEnvelope(long globalSequence) {
        return new EventEnvelope {
            EventID = Guid.NewGuid(),
            StreamID = "stream-1",
            AggregateType = "Test",
            AggregateID = Guid.NewGuid(),
            Version = 1,
            GlobalSequence = globalSequence,
            EventType = "test.event",
            EventSchemaVersion = 1,
            Payload = "{}",
            CausedBy = Guid.NewGuid(),
            CorrelationID = Guid.NewGuid(),
            OccurredAt = DateTimeOffset.UtcNow
        };
    }

    private static async IAsyncEnumerable<EventEnvelope> ToAsyncEnumerable(IEnumerable<EventEnvelope> source) {
        foreach (var item in source) {
            yield return item;
        }

        await Task.CompletedTask;
    }

    [Fact]
    public async Task RebuildAsync_ResetsProjectionsBeforeApplyingEvents() {
        // arrange
        var envelopes = new[] { CreateEnvelope(1), CreateEnvelope(2) };

        var eventStoreMock = new Mock<IEventStore>();
        eventStoreMock.Setup(store => store.GetAsync(0, It.IsAny<CancellationToken>()))
                      .Returns(ToAsyncEnumerable(envelopes));

        var calls = new List<string>();
        var projectionMock = new Mock<IProjection>();
        projectionMock.Setup(p => p.ResetAsync(It.IsAny<CancellationToken>()))
                      .Callback(() => calls.Add("reset"))
                      .Returns(Task.CompletedTask);
        projectionMock.Setup(p => p.ApplyAsync(It.IsAny<EventEnvelope>(), It.IsAny<CancellationToken>()))
                      .Callback(() => calls.Add("apply"))
                      .Returns(Task.CompletedTask);

        var sut = new ProjectionRebuilder(eventStoreMock.Object);

        // act
        await sut.RebuildAsync([projectionMock.Object], TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(["reset", "apply", "apply"], calls);
    }

    [Fact]
    public async Task RebuildAsync_AppliesEventsToEveryProjection() {
        // arrange
        var envelopes = new[] { CreateEnvelope(1) };

        var eventStoreMock = new Mock<IEventStore>();
        eventStoreMock.Setup(store => store.GetAsync(0, It.IsAny<CancellationToken>()))
                      .Returns(ToAsyncEnumerable(envelopes));

        var projectionAMock = new Mock<IProjection>();
        projectionAMock.Setup(p => p.ResetAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        projectionAMock.Setup(p => p.ApplyAsync(It.IsAny<EventEnvelope>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var projectionBMock = new Mock<IProjection>();
        projectionBMock.Setup(p => p.ResetAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        projectionBMock.Setup(p => p.ApplyAsync(It.IsAny<EventEnvelope>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var sut = new ProjectionRebuilder(eventStoreMock.Object);

        // act
        await sut.RebuildAsync([projectionAMock.Object, projectionBMock.Object], TestContext.Current.CancellationToken);

        // assert
        projectionAMock.Verify(p => p.ApplyAsync(envelopes[0], It.IsAny<CancellationToken>()), Times.Once);
        projectionBMock.Verify(p => p.ApplyAsync(envelopes[0], It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RebuildAsync_WithNullProjections_Throws() {
        // arrange
        var eventStoreMock = new Mock<IEventStore>();
        var sut = new ProjectionRebuilder(eventStoreMock.Object);

        // act & assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sut.RebuildAsync(null!, TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    public void Constructor_WithNullEventStore_Throws() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => new ProjectionRebuilder(null!));
    }
}
