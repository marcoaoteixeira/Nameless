using Nameless.Testing.Tools.Attributes;

namespace Nameless.Reporting;

[UnitTest]
public class FaultExceptionTests {
    [Fact]
    public void WhenConstructed_WithReason_ThenMessageIsTheReason() {
        var exception = new FaultException("Something went wrong");

        Assert.Equal("Something went wrong", exception.Message);
    }

    [Fact]
    public void WhenConstructed_WithoutCode_ThenCodeIsNull() {
        var exception = new FaultException("Something went wrong");

        Assert.Null(exception.Code);
    }

    [Fact]
    public void WhenConstructed_WithCode_ThenCodeIsExposed() {
        var exception = new FaultException("Something went wrong", "ERR_001");

        Assert.Equal("ERR_001", exception.Code);
    }

    [Fact]
    public void WhenConstructed_ThenStackTraceIsNull() {
        var exception = new FaultException("Something went wrong");

        Assert.Null(exception.StackTrace);
    }
}
