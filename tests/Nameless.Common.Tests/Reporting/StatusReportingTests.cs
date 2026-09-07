using System.Reactive.Linq;
using Moq;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Data;
using Nameless.Testing.Tools.Helpers;

namespace Nameless.Reporting;

[UnitTest]
public class StatusReportingTests {
    private static StatusReporting CreateSut(
        string serviceName = "TestService",
        string? channelKey = null,
        TimeProvider? timeProvider = null,
        int bufferSize = 10) {
        return new StatusReporting(
            serviceName,
            channelKey,
            timeProvider ?? TimeProvider.System,
            OptionsHelper.Create<StatusReportingOptions>(o => o.BufferSize = bufferSize));
    }

    // ─── Constructor ──────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_SetsServiceName() {
        var sut = CreateSut(serviceName: "MyWorker");

        Assert.Equal("MyWorker", sut.ServiceName);
    }

    [Fact]
    public void Constructor_SetsChannelKey() {
        var sut = CreateSut(channelKey: "file-001.csv");

        Assert.Equal("file-001.csv", sut.ChannelKey);
    }

    [Theory]
    [ClassData(typeof(StringNullEmptyWhiteSpaceTheoryData))]
    public void Constructor_WithInvalidServiceName_Throws(string? serviceName, Type exceptionType) {
        Assert.Throws(exceptionType, () => CreateSut(serviceName: serviceName!));
    }

    [Fact]
    public void Constructor_PublishesInitialIdleStatus() {
        var sut = CreateSut();
        var received = new List<StatusUpdate>();
        sut.Status.Subscribe(received.Add);

        Assert.Single(received);
        Assert.Equal("Idle", received[0].Message);
        Assert.Equal(StatusLevel.Info, received[0].Level);
    }

    // ─── Report ───────────────────────────────────────────────────────────────

    [Fact]
    public void Report_PublishesStatusUpdate() {
        var sut = CreateSut();
        var received = new List<StatusUpdate>();
        sut.Status.Subscribe(received.Add);

        sut.Report("Working", StatusLevel.Info, metadata: null);

        Assert.Equal(2, received.Count);
        Assert.Equal("Working", received[1].Message);
    }

    [Theory]
    [InlineData(StatusLevel.Info)]
    [InlineData(StatusLevel.Warning)]
    [InlineData(StatusLevel.Error)]
    public void Report_PublishesUpdate_WithCorrectLevel(StatusLevel level) {
        var sut = CreateSut();
        var received = new List<StatusUpdate>();
        sut.Status.Subscribe(received.Add);

        sut.Report("msg", level, metadata: null);

        Assert.Equal(level, received[1].Level);
    }

    [Fact]
    public void Report_UsesTimeProvider_ForTimestamp() {
        var fixedTime = new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero);
        var timeProvider = new Mock<TimeProvider>();
        timeProvider.Setup(tp => tp.GetUtcNow()).Returns(fixedTime);

        var sut = CreateSut(timeProvider: timeProvider.Object);
        var received = new List<StatusUpdate>();
        sut.Status.Subscribe(received.Add);

        sut.Report("Working", StatusLevel.Warning, null);

        Assert.Equal(fixedTime, received[1].Timestamp);
    }

    [Fact]
    public void Report_AfterComplete_IsNoOp() {
        var sut = CreateSut();
        var received = new List<StatusUpdate>();
        sut.Status.Subscribe(received.Add, () => { });

        sut.Complete();
        var countBeforeReport = received.Count;
        sut.Report("Should not appear", StatusLevel.Info, null);

        Assert.Equal(countBeforeReport, received.Count);
    }

    [Fact]
    public void Report_AfterFault_IsNoOp() {
        var sut = CreateSut();
        var received = new List<StatusUpdate>();
        sut.Status.Subscribe(received.Add, _ => { });

        sut.Fault("boom", code: null);
        var countBeforeFaultReport = received.Count;
        sut.Report("Should not appear", StatusLevel.Info, null);

        Assert.Equal(countBeforeFaultReport, received.Count);
    }

    // ─── Complete ─────────────────────────────────────────────────────────────

    [Fact]
    public void Complete_SignalsOnCompleted() {
        var sut = CreateSut();
        var completed = false;
        sut.Status.Subscribe(_ => { }, () => completed = true);

        sut.Complete();

        Assert.True(completed);
    }

    // ─── Fault ────────────────────────────────────────────────────────────────

    [Fact]
    public void Fault_WithReasonAndCode_SignalsOnError_WithFaultException() {
        var sut = CreateSut();
        Exception? captured = null;
        sut.Status.Subscribe(_ => { }, ex => captured = ex);

        sut.Fault("Something went wrong", "ERR_001");

        var fault = Assert.IsType<FaultException>(captured);
        Assert.Equal("Something went wrong", fault.Message);
        Assert.Equal("ERR_001", fault.Code);
    }

    [Fact]
    public void Fault_WithException_SignalsOnError_WithThatSameException() {
        var sut = CreateSut();
        var expected = new InvalidOperationException("outer");
        Exception? captured = null;
        sut.Status.Subscribe(_ => { }, ex => captured = ex);

        sut.Fault(expected);

        Assert.Same(expected, captured);
    }

    // ─── Subscriber count / Idle ──────────────────────────────────────────────

    [Fact]
    public void Status_NewSubscriber_AfterComplete_ReceivesTerminalSignal() {
        var sut = CreateSut();
        sut.Complete();

        var completed = false;
        sut.Status.Subscribe(_ => { }, () => completed = true);

        Assert.True(completed);
    }

    // ─── Replay ───────────────────────────────────────────────────────────────

    [Fact]
    public void Status_NewSubscriber_ReceivesBufferedHistory() {
        var sut = CreateSut(bufferSize: 5);
        sut.Report("A", StatusLevel.Info, null);
        sut.Report("B", StatusLevel.Warning, null);

        var received = new List<StatusUpdate>();
        sut.Status.Subscribe(received.Add);

        Assert.Equal(3, received.Count); // Idle + A + B
        Assert.Equal("Idle", received[0].Message);
        Assert.Equal("A", received[1].Message);
        Assert.Equal("B", received[2].Message);
    }

    // ─── Dispose ──────────────────────────────────────────────────────────────

    [Fact]
    public void Report_AfterDispose_ThrowsObjectDisposedException() {
        var sut = CreateSut();
        sut.Dispose();

        Assert.Throws<ObjectDisposedException>(() => sut.Report("x", StatusLevel.Info, null));
    }

    [Fact]
    public void Complete_AfterDispose_ThrowsObjectDisposedException() {
        var sut = CreateSut();
        sut.Dispose();

        Assert.Throws<ObjectDisposedException>(sut.Complete);
    }

    [Fact]
    public void Fault_AfterDispose_ThrowsObjectDisposedException() {
        var sut = CreateSut();
        sut.Dispose();

        Assert.Throws<ObjectDisposedException>(() => sut.Fault("x", null));
    }

    [Fact]
    public void Dispose_IsIdempotent() {
        var sut = CreateSut();
        sut.Dispose();

        var ex = Record.Exception(sut.Dispose);

        Assert.Null(ex);
    }
}
