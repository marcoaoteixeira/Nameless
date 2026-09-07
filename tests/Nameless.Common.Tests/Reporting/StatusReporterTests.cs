using Moq;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Helpers;

namespace Nameless.Reporting;

[UnitTest]
public class StatusReporterTests {
    private static (StatusReporter<SampleService> sut, Mock<IStatusReporter> inner) CreateSut() {
        var innerMock = new Mock<IStatusReporter>();
        var hubMock = new Mock<IStatusReportingHub>();
        hubMock
            .Setup(h => h.GetReporter(typeof(SampleService), null))
            .Returns(innerMock.Object);

        return (new StatusReporter<SampleService>(hubMock.Object), innerMock);
    }

    [Fact]
    public void Report_DelegatesToInnerReporter() {
        var (sut, inner) = CreateSut();
        var metadata = new Dictionary<string, string> { ["k"] = "v" };

        sut.Report("msg", StatusLevel.Warning, metadata);

        inner.Verify(r => r.Report("msg", StatusLevel.Warning, metadata), Times.Once());
    }

    [Fact]
    public void Complete_DelegatesToInnerReporter() {
        var (sut, inner) = CreateSut();

        sut.Complete();

        inner.Verify(r => r.Complete(), Times.Once());
    }

    [Fact]
    public void Fault_WithReasonAndCode_DelegatesToInnerReporter() {
        var (sut, inner) = CreateSut();

        sut.Fault("reason", "CODE");

        inner.Verify(r => r.Fault("reason", "CODE"), Times.Once());
    }

    [Fact]
    public void Fault_WithException_DelegatesToInnerReporter() {
        var (sut, inner) = CreateSut();
        var ex = new InvalidOperationException("oops");

        sut.Fault(ex);

        inner.Verify(r => r.Fault(ex), Times.Once());
    }

    [Fact]
    public void Dispose_CallsDispose_OnInnerReporter_WhenIDisposable() {
        var hub = new StatusReportingHub(
            TimeProvider.System,
            OptionsHelper.Create<StatusReportingOptions>()
        );
        var sut = new StatusReporter<SampleService>(hub);

        sut.Dispose();

        // Inner StatusReporting is disposed; Report now throws ObjectDisposedException
        Assert.Throws<ObjectDisposedException>(() => sut.Report("x", StatusLevel.Info, null));
    }

    // ─── test doubles ─────────────────────────────────────────────────────────

    private class SampleService { }
}
