using Nameless.Mediator.Events;

namespace Nameless.EventSourcing;

[UnitTest]
public class AggregateRootTests {
    private sealed record NameSet(string Name) : IEvent;
    private sealed record NameCleared : IEvent;

    private sealed class TestAggregate : AggregateRoot<Guid> {
        public string? Name { get; private set; }
        public int WhenCallCount { get; private set; }

        public TestAggregate() { }

        public TestAggregate(Guid id) {
            ID = id;
        }

        public void SetName(string name) {
            RaiseEvent(new NameSet(name));
        }

        public void ClearName() {
            RaiseEvent(new NameCleared());
        }

        public void RaiseNullEvent() {
            RaiseEvent(null!);
        }

        protected override void When(IEvent @event) {
            WhenCallCount++;

            switch (@event) {
                case NameSet nameSet:
                    Name = nameSet.Name;
                    break;
                case NameCleared:
                    Name = null;
                    break;
            }
        }
    }

    [Fact]
    public void RaiseEvent_AppliesEventAndTracksAsUncommitted() {
        // arrange
        var sut = new TestAggregate(Guid.NewGuid());

        // act
        sut.SetName("Alpha");

        // assert
        Assert.Multiple(
            () => Assert.Equal("Alpha", sut.Name),
            () => Assert.Equal(1, sut.Version),
            () => Assert.Single(sut.UncommittedEvents)
        );
    }

    [Fact]
    public void RaiseEvent_CalledMultipleTimes_IncrementsVersionEachTime() {
        // arrange
        var sut = new TestAggregate(Guid.NewGuid());

        // act
        sut.SetName("Alpha");
        sut.SetName("Beta");
        sut.ClearName();

        // assert
        Assert.Multiple(
            () => Assert.Equal(3, sut.Version),
            () => Assert.Equal(3, sut.WhenCallCount),
            () => Assert.Null(sut.Name),
            () => Assert.Equal(3, sut.UncommittedEvents.Count)
        );
    }

    [Fact]
    public void RaiseEvent_WithNullEvent_Throws() {
        // arrange
        var sut = new TestAggregate(Guid.NewGuid());

        // act & assert
        Assert.Throws<ArgumentNullException>(sut.RaiseNullEvent);
    }

    [Fact]
    public void ClearUncommittedEvents_RemovesTrackedEvents() {
        // arrange
        var sut = new TestAggregate(Guid.NewGuid());
        sut.SetName("Alpha");

        // act
        sut.ClearUncommittedEvents();

        // assert
        Assert.Empty(sut.UncommittedEvents);
    }

    [Fact]
    public void ClearUncommittedEvents_DoesNotAffectVersion() {
        // arrange
        var sut = new TestAggregate(Guid.NewGuid());
        sut.SetName("Alpha");

        // act
        sut.ClearUncommittedEvents();

        // assert
        Assert.Equal(1, sut.Version);
    }

    [Fact]
    public void UncommittedEvents_ReturnsSnapshot_NotLiveView() {
        // arrange
        var sut = new TestAggregate(Guid.NewGuid());
        sut.SetName("Alpha");

        // act
        var snapshot = sut.UncommittedEvents;
        sut.SetName("Beta");

        // assert
        Assert.Single(snapshot);
    }

    [Fact]
    public void LoadFromHistory_ReplaysEventsWithoutTrackingAsUncommitted() {
        // arrange
        var sut = new TestAggregate();
        var history = new IEvent[] { new NameSet("Alpha"), new NameSet("Beta") };

        // act
        sut.LoadFromHistory(history);

        // assert
        Assert.Multiple(
            () => Assert.Equal("Beta", sut.Name),
            () => Assert.Equal(2, sut.Version),
            () => Assert.Empty(sut.UncommittedEvents)
        );
    }

    [Fact]
    public void LoadFromHistory_WithNullHistory_Throws() {
        // arrange
        var sut = new TestAggregate();

        // act & assert
        Assert.Throws<ArgumentNullException>(() => sut.LoadFromHistory(null!));
    }
}
