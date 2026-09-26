using System.Text.Json.Nodes;
using Nameless.EventSourcing.Projections;
using Nameless.EventSourcing.UpCasting;
using Nameless.Mediator.Events;

namespace Nameless.EventSourcing.Registration;

[UnitTest]
public class EventSourcingRegistrationTests {
    private sealed class FakeUpCaster : IEventUpCaster {
        public string EventType => "fake";
        public int FromVersion => 1;
        public JsonNode Upcast(JsonNode payload) => payload;
    }

    private sealed class FakeProjection : IProjection {
        public Task ResetAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        public Task ApplyAsync(EventEnvelope envelope, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    [EventType("fake.event", 1)]
    private sealed record FakeEvent : IEvent;

    private static EventSourcingRegistration CreateSut() {
        return new EventSourcingRegistration().WithUseAssemblyScan(false);
    }

    [Fact]
    public void RegisterUpCaster_Generic_AddsType() {
        // arrange
        var sut = CreateSut();

        // act
        sut.RegisterUpCaster<FakeUpCaster>();

        // assert
        Assert.Contains(typeof(FakeUpCaster), sut.UpCasters);
    }

    [Fact]
    public void RegisterUpCaster_WithTypeNotAssignableFromIEventUpCaster_Throws() {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.RegisterUpCaster(typeof(string)));
    }

    [Fact]
    public void RegisterProjection_Generic_AddsType() {
        // arrange
        var sut = CreateSut();

        // act
        sut.RegisterProjection<FakeProjection>();

        // assert
        Assert.Contains(typeof(FakeProjection), sut.Projections);
    }

    [Fact]
    public void RegisterProjection_WithTypeNotAssignableFromIProjection_Throws() {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.RegisterProjection(typeof(string)));
    }

    [Fact]
    public void RegisterEvent_WithEventTypeAttribute_AddsType() {
        // arrange
        var sut = CreateSut();

        // act
        sut.RegisterEvent(typeof(FakeEvent));

        // assert
        Assert.Contains(typeof(FakeEvent), sut.Events);
    }

    [Fact]
    public void RegisterEvent_WithoutEventTypeAttribute_IsIgnored() {
        // arrange
        var sut = CreateSut();

        // act
        sut.RegisterEvent(typeof(UndecoratedFakeEvent));

        // assert
        Assert.DoesNotContain(typeof(UndecoratedFakeEvent), sut.Events);
    }

    private sealed record UndecoratedFakeEvent : IEvent;

    [Fact]
    public void RegisterEvent_WithTypeNotAssignableFromIEvent_Throws() {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.RegisterEvent(typeof(string)));
    }
}
