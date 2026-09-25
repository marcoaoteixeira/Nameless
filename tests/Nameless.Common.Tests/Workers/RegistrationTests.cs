using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Nameless.Reporting;

namespace Nameless.Workers;

public sealed class NoopWorker(IConfiguration configuration, IStatusReporter reporter)
    : PeriodicWorker(configuration, reporter, NullLogger.Instance) {
    public override Task DoWorkAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

[UnitTest]
public class PeriodicWorkersRegistrationTests {
    [Fact]
    public void WithPeriodicWorker_Generic_AddsType() {
        // arrange
        var sut = new PeriodicWorkersRegistration().WithUseAssemblyScan(false);

        // act
        var returned = sut.WithPeriodicWorker<NoopWorker>();

        // assert
        Assert.Multiple(
            () => Assert.Same(sut, returned),
            () => Assert.Contains(typeof(NoopWorker), sut.Workers)
        );
    }

    [Fact]
    public void WithPeriodicWorker_WithUnrelatedType_Throws() {
        // arrange
        var sut = new PeriodicWorkersRegistration();

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.WithPeriodicWorker(typeof(string)));
    }

    [Fact]
    public void WithPeriodicWorker_WithAbstractType_Throws() {
        // arrange
        var sut = new PeriodicWorkersRegistration();

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.WithPeriodicWorker(typeof(PeriodicWorker)));
    }

    [Fact]
    public void RegisterPeriodicWorkers_RegistersWorkersAsHostedServices() {
        // arrange
        var services = new ServiceCollection();

        // act
        var returned = services.RegisterPeriodicWorkers(r => r.WithUseAssemblyScan(false).WithPeriodicWorker<NoopWorker>());

        // assert
        Assert.Multiple(
            () => Assert.Same(services, returned),
            () => Assert.Contains(services, d => d.ServiceType == typeof(IHostedService) && d.ImplementationType == typeof(NoopWorker))
        );
    }

    [Fact]
    public void RegisterPeriodicWorkers_WithoutConfigure_DoesNotThrow() {
        // arrange
        var services = new ServiceCollection();

        // act
        var exception = Record.Exception(() => services.RegisterPeriodicWorkers());

        // assert
        Assert.Null(exception);
    }
}

[IntegrationTest]
public class PeriodicWorkerExecutionTests {
    [Fact]
    public async Task Worker_ReportsRunningIdleAndStoppedStatuses() {
        // arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["PeriodicWorkers:IsEnabled"] = "true",
                ["PeriodicWorkers:Interval"] = "00:00:00.020"
            })
            .Build();

        var messages = new List<string>();
        var reporter = new Mock<IStatusReporter>();
        reporter.Setup(r => r.Report(It.IsAny<string>(), It.IsAny<StatusLevel>(), It.IsAny<Dictionary<string, string>?>()))
                .Callback<string, StatusLevel, Dictionary<string, string>?>((m, _, _) => { lock (messages) { messages.Add(m); } });

        var worker = new NoopWorker(configuration, reporter.Object);

        // act
        await worker.StartAsync(TestContext.Current.CancellationToken);
        await Task.Delay(300, TestContext.Current.CancellationToken);
        await worker.StopAsync(TestContext.Current.CancellationToken);

        // assert
        string[] snapshot;
        lock (messages) { snapshot = [.. messages]; }

        Assert.Multiple(
            () => Assert.Contains(snapshot, m => m.Contains("is running")),
            () => Assert.Contains(snapshot, m => m.Contains("is idle")),
            () => Assert.Contains(snapshot, m => m.Contains("has stopped"))
        );
    }
}
