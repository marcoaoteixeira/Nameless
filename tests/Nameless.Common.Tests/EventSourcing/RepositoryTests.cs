using Moq;
using Nameless.EventSourcing.UpCasting;
using Nameless.Mediator;
using Nameless.Mediator.Events;
using Nameless.Resilience;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.EventSourcing;

[UnitTest]
public class RepositoryTests {
    [EventType("test.nameset", 1)]
    private sealed record NameSet(string Name) : IEvent;

    public sealed class TestAggregate : AggregateRoot<Guid> {
        public string? Name { get; private set; }

        public void SetName(string name) => RaiseEvent(new NameSet(name));

        protected override void When(IEvent @event) {
            if (@event is NameSet nameSet) {
                Name = nameSet.Name;
            }
        }
    }

    private sealed record RenameCommand(string NewName);

    private static IEventSerializer CreateSerializer() {
        var catalog = new EventTypeCatalog([typeof(NameSet)]);
        return new EventSerializer(catalog, upCasters: []);
    }

    private static IRetryPipelineFactory CreateRetryPipelineFactory() {
        var logger = new LoggerMocker<RetryPipelineFactory>().WithAnyLogLevel().Build();
        return new RetryPipelineFactory(logger);
    }

    private static Repository<TestAggregate> CreateSut(
        IEventStore eventStore,
        IMediator? mediator = null,
        IEventSerializer? eventSerializer = null) {
        return new Repository<TestAggregate>(
            eventSerializer ?? CreateSerializer(),
            eventStore,
            mediator ?? new Mock<IMediator>().Object,
            CreateRetryPipelineFactory()
        );
    }

    private static EventEnvelope CreateEnvelope(string streamID, int version, IEvent @event, IEventSerializer serializer) {
        var metadata = serializer.Serialize(@event);

        return new EventEnvelope {
            EventID = Guid.NewGuid(),
            StreamID = streamID,
            AggregateType = nameof(TestAggregate),
            AggregateID = Guid.NewGuid(),
            Version = version,
            GlobalSequence = version,
            EventType = metadata.EventType,
            EventSchemaVersion = metadata.SchemaVersion,
            Payload = metadata.Payload,
            CausedBy = Guid.NewGuid(),
            CorrelationID = Guid.NewGuid(),
            OccurredAt = DateTimeOffset.UtcNow
        };
    }

    [Fact]
    public async Task LoadAsync_WithNoEvents_ReturnsNull() {
        // arrange
        var eventStoreMock = new Mock<IEventStore>();
        eventStoreMock.Setup(s => s.ReadStreamAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync((IReadOnlyList<EventEnvelope>)[]);

        var sut = CreateSut(eventStoreMock.Object);

        // act
        var aggregate = await sut.LoadAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        // assert
        Assert.Null(aggregate);
    }

    [Fact]
    public async Task LoadAsync_WithEvents_RebuildsAggregate() {
        // arrange
        var serializer = CreateSerializer();
        var aggregateID = Guid.NewGuid();
        var streamID = $"{nameof(TestAggregate)}-{aggregateID}";

        var envelope = CreateEnvelope(streamID, 1, new NameSet("Alpha"), serializer);

        var eventStoreMock = new Mock<IEventStore>();
        eventStoreMock.Setup(s => s.ReadStreamAsync(streamID, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((IReadOnlyList<EventEnvelope>)[envelope]);

        var sut = CreateSut(eventStoreMock.Object, eventSerializer: serializer);

        // act
        var aggregate = await sut.LoadAsync(aggregateID, TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => Assert.NotNull(aggregate),
            () => Assert.Equal("Alpha", aggregate!.Name),
            () => Assert.Equal(1, aggregate!.Version),
            () => Assert.Empty(aggregate!.UncommittedEvents)
        );
    }

    [Fact]
    public async Task SaveAsync_WithUncommittedEvents_AppendsAndPublishes() {
        // arrange
        var eventStoreMock = new Mock<IEventStore>();
        eventStoreMock.Setup(s => s.AppendAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid?>(),
                It.IsAny<int>(), It.IsAny<IReadOnlyList<IEvent>>(), It.IsAny<Guid>(), It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

        var sut = CreateSut(eventStoreMock.Object, mediatorMock.Object);

        var aggregate = new TestAggregate();
        aggregate.SetName("Alpha");

        var actor = new ActorContext(Guid.NewGuid());

        // act
        await sut.SaveAsync(aggregate, actor, Guid.NewGuid(), TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => eventStoreMock.Verify(s => s.AppendAsync(
                It.IsAny<string>(), nameof(TestAggregate), aggregate.ID, actor.TenantID,
                0, It.Is<IReadOnlyList<IEvent>>(e => e.Count == 1), actor.UserID, It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()), Times.Once),
            () => mediatorMock.Verify(m => m.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()), Times.Once),
            () => Assert.Empty(aggregate.UncommittedEvents)
        );
    }

    [Fact]
    public async Task SaveAsync_WithNoUncommittedEvents_DoesNothing() {
        // arrange
        var eventStoreMock = new Mock<IEventStore>();
        var mediatorMock = new Mock<IMediator>();
        var sut = CreateSut(eventStoreMock.Object, mediatorMock.Object);

        var aggregate = new TestAggregate();
        var actor = new ActorContext(Guid.NewGuid());

        // act
        await sut.SaveAsync(aggregate, actor, Guid.NewGuid(), TestContext.Current.CancellationToken);

        // assert
        eventStoreMock.Verify(s => s.AppendAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid?>(),
            It.IsAny<int>(), It.IsAny<IReadOnlyList<IEvent>>(), It.IsAny<Guid>(), It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SaveAsync_WithNullAggregate_Throws() {
        // arrange
        var sut = CreateSut(new Mock<IEventStore>().Object);

        // act & assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sut.SaveAsync(null!, new ActorContext(Guid.NewGuid()), Guid.NewGuid(), TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    public async Task SaveAsync_WithNullActor_Throws() {
        // arrange
        var sut = CreateSut(new Mock<IEventStore>().Object);

        // act & assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sut.SaveAsync(new TestAggregate(), null!, Guid.NewGuid(), TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    public async Task ExecuteAsync_HappyPath_LoadsAppliesAppendsAndPublishes() {
        // arrange
        var serializer = CreateSerializer();
        var aggregateID = Guid.NewGuid();
        var streamID = $"{nameof(TestAggregate)}-{aggregateID}";

        var envelope = CreateEnvelope(streamID, 1, new NameSet("Alpha"), serializer);

        var eventStoreMock = new Mock<IEventStore>();
        eventStoreMock.Setup(s => s.ReadStreamAsync(streamID, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((IReadOnlyList<EventEnvelope>)[envelope]);
        eventStoreMock.Setup(s => s.AppendAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid?>(),
                It.IsAny<int>(), It.IsAny<IReadOnlyList<IEvent>>(), It.IsAny<Guid>(), It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

        var sut = CreateSut(eventStoreMock.Object, mediatorMock.Object, serializer);

        // act
        await sut.ExecuteAsync(
            aggregateID,
            new RenameCommand("Beta"),
            apply: (aggregate, command) => aggregate.SetName(command.NewName),
            actor: new ActorContext(Guid.NewGuid()),
            correlationID: Guid.NewGuid(),
            policy: null,
            TestContext.Current.CancellationToken
        );

        // assert
        mediatorMock.Verify(m => m.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_AggregateNotFound_ThrowsInvalidOperationException() {
        // arrange
        var eventStoreMock = new Mock<IEventStore>();
        eventStoreMock.Setup(s => s.ReadStreamAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync((IReadOnlyList<EventEnvelope>)[]);

        var sut = CreateSut(eventStoreMock.Object);

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.ExecuteAsync(
                Guid.NewGuid(),
                new RenameCommand("Beta"),
                apply: (aggregate, command) => aggregate.SetName(command.NewName),
                actor: new ActorContext(Guid.NewGuid()),
                correlationID: Guid.NewGuid(),
                policy: null,
                TestContext.Current.CancellationToken
            )
        );
    }

    [Fact]
    public async Task ExecuteAsync_PolicyDenies_ThrowsUnauthorizedActionException() {
        // arrange
        var serializer = CreateSerializer();
        var aggregateID = Guid.NewGuid();
        var streamID = $"{nameof(TestAggregate)}-{aggregateID}";

        var envelope = CreateEnvelope(streamID, 1, new NameSet("Alpha"), serializer);

        var eventStoreMock = new Mock<IEventStore>();
        eventStoreMock.Setup(s => s.ReadStreamAsync(streamID, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((IReadOnlyList<EventEnvelope>)[envelope]);

        var sut = CreateSut(eventStoreMock.Object, eventSerializer: serializer);

        var policyMock = new Mock<IAuthorizationPolicy<TestAggregate>>();
        policyMock.Setup(p => p.CanApply(It.IsAny<TestAggregate>(), It.IsAny<object>(), It.IsAny<ActorContext>()))
                  .Returns(false);

        // act & assert
        await Assert.ThrowsAsync<UnauthorizedActionException>(
            () => sut.ExecuteAsync(
                aggregateID,
                new RenameCommand("Beta"),
                apply: (aggregate, command) => aggregate.SetName(command.NewName),
                actor: new ActorContext(Guid.NewGuid()),
                correlationID: Guid.NewGuid(),
                policy: policyMock.Object,
                TestContext.Current.CancellationToken
            )
        );
    }

    [Fact]
    public async Task ExecuteAsync_RetriesOnConcurrencyConflict_ThenSucceeds() {
        // arrange
        var serializer = CreateSerializer();
        var aggregateID = Guid.NewGuid();
        var streamID = $"{nameof(TestAggregate)}-{aggregateID}";

        var envelope = CreateEnvelope(streamID, 1, new NameSet("Alpha"), serializer);

        var eventStoreMock = new Mock<IEventStore>();
        eventStoreMock.Setup(s => s.ReadStreamAsync(streamID, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((IReadOnlyList<EventEnvelope>)[envelope]);

        var appendCallCount = 0;
        eventStoreMock.Setup(s => s.AppendAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid?>(),
                It.IsAny<int>(), It.IsAny<IReadOnlyList<IEvent>>(), It.IsAny<Guid>(), It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .Returns(() => {
                appendCallCount++;
                return appendCallCount == 1
                    ? throw new ConcurrencyConflictException(streamID, expectedVersion: 1)
                    : Task.CompletedTask;
            });

        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

        var sut = CreateSut(eventStoreMock.Object, mediatorMock.Object, serializer);

        // act
        await sut.ExecuteAsync(
            aggregateID,
            new RenameCommand("Beta"),
            apply: (aggregate, command) => aggregate.SetName(command.NewName),
            actor: new ActorContext(Guid.NewGuid()),
            correlationID: Guid.NewGuid(),
            policy: null,
            TestContext.Current.CancellationToken
        );

        // assert
        Assert.Equal(2, appendCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullCommand_Throws() {
        // arrange
        var sut = CreateSut(new Mock<IEventStore>().Object);

        // act & assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sut.ExecuteAsync<RenameCommand>(
                Guid.NewGuid(), null!, (_, _) => { }, new ActorContext(Guid.NewGuid()), Guid.NewGuid(), null,
                TestContext.Current.CancellationToken
            )
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithNullApply_Throws() {
        // arrange
        var sut = CreateSut(new Mock<IEventStore>().Object);

        // act & assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sut.ExecuteAsync(
                Guid.NewGuid(), new RenameCommand("Beta"), apply: null!, new ActorContext(Guid.NewGuid()), Guid.NewGuid(), null,
                TestContext.Current.CancellationToken
            )
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithNullActor_Throws() {
        // arrange
        var sut = CreateSut(new Mock<IEventStore>().Object);

        // act & assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sut.ExecuteAsync(
                Guid.NewGuid(), new RenameCommand("Beta"), (aggregate, command) => aggregate.SetName(command.NewName),
                null!, Guid.NewGuid(), null, TestContext.Current.CancellationToken
            )
        );
    }

    [Fact]
    public void Constructor_WithNullDependency_Throws() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => new Repository<TestAggregate>(
            null!, new Mock<IEventStore>().Object, new Mock<IMediator>().Object, CreateRetryPipelineFactory()
        ));
    }
}
