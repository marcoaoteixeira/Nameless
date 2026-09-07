using Moq;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Reporting;

[UnitTest]
public class StatusReporterExtensionsTests {
    [Fact]
    public void ReportInfo_CallsReport_WithInfoLevel() {
        var reporter = new Mock<IStatusReporter>();

        reporter.Object.ReportInfo("hello");

        reporter.Verify(r => r.Report("hello", StatusLevel.Info, null), Times.Once());
    }

    [Fact]
    public void ReportInfo_PassesMetadata() {
        var reporter = new Mock<IStatusReporter>();
        var metadata = new Dictionary<string, string> { ["k"] = "v" };

        reporter.Object.ReportInfo("hello", metadata);

        reporter.Verify(r => r.Report("hello", StatusLevel.Info, metadata), Times.Once());
    }

    [Fact]
    public void ReportWarn_CallsReport_WithWarningLevel() {
        var reporter = new Mock<IStatusReporter>();

        reporter.Object.ReportWarn("watch out");

        reporter.Verify(r => r.Report("watch out", StatusLevel.Warning, null), Times.Once());
    }

    [Fact]
    public void ReportWarn_PassesMetadata() {
        var reporter = new Mock<IStatusReporter>();
        var metadata = new Dictionary<string, string> { ["source"] = "io" };

        reporter.Object.ReportWarn("disk nearly full", metadata);

        reporter.Verify(r => r.Report("disk nearly full", StatusLevel.Warning, metadata), Times.Once());
    }

    [Fact]
    public void ReportError_CallsReport_WithErrorLevel() {
        var reporter = new Mock<IStatusReporter>();

        reporter.Object.ReportError("failed");

        reporter.Verify(r => r.Report("failed", StatusLevel.Error, null), Times.Once());
    }

    [Fact]
    public void ReportError_PassesMetadata() {
        var reporter = new Mock<IStatusReporter>();
        var metadata = new Dictionary<string, string> { ["code"] = "500" };

        reporter.Object.ReportError("internal error", metadata);

        reporter.Verify(r => r.Report("internal error", StatusLevel.Error, metadata), Times.Once());
    }

    [Fact]
    public void Fault_WithReasonOnly_CallsFault_WithNullCode() {
        var reporter = new Mock<IStatusReporter>();

        reporter.Object.Fault("something broke");

        reporter.Verify(r => r.Fault("something broke", null), Times.Once());
    }
}
