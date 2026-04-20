using System.Runtime.InteropServices;
using System.Security;

namespace Nameless;

public class ExceptionExtensionsTests {
    // ─── IsFatal ────────────────────────────────────────────────────────────

    [Fact]
    public void IsFatal_WithRegularException_ReturnsFalse() {
        var ex = new Exception("regular");
        Assert.False(ex.IsFatal());
    }

    [Fact]
    public void IsFatal_WithInvalidOperationException_ReturnsFalse() {
        var ex = new InvalidOperationException("non-fatal");
        Assert.False(ex.IsFatal());
    }

    [Fact]
    public void IsFatal_WithOutOfMemoryException_ReturnsTrue() {
        var ex = new OutOfMemoryException();
        Assert.True(ex.IsFatal());
    }

    [Fact]
    public void IsFatal_WithAccessViolationException_ReturnsTrue() {
        var ex = new AccessViolationException();
        Assert.True(ex.IsFatal());
    }

    [Fact]
    public void IsFatal_WithSecurityException_ReturnsTrue() {
        var ex = new SecurityException();
        Assert.True(ex.IsFatal());
    }

    [Fact]
    public void IsFatal_WithSEHException_ReturnsTrue() {
        var ex = new SEHException();
        Assert.True(ex.IsFatal());
    }

    [Fact]
    public void IsFatal_WithAppDomainUnloadedException_ReturnsTrue() {
        var ex = new AppDomainUnloadedException();
        Assert.True(ex.IsFatal());
    }
}
