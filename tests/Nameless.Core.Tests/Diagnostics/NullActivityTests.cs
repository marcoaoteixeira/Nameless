using System.Diagnostics;

namespace Nameless.Diagnostics;

public class NullActivityTests {
    // --- Instance singleton ---

    [Fact]
    public void Instance_ReturnsSameSingleton() {
        // act & assert
        Assert.Same(NullActivity.Instance, NullActivity.Instance);
    }

    // --- SetTag ---

    [Fact]
    public void SetTag_ReturnsSameInstance() {
        // act
        var result = NullActivity.Instance.SetTag("key", "value");

        // assert
        Assert.Same(NullActivity.Instance, result);
    }

    // --- AddException ---

    [Fact]
    public void AddException_ReturnsSameInstance() {
        // act
        var result = NullActivity.Instance.AddException(new Exception("test"));

        // assert
        Assert.Same(NullActivity.Instance, result);
    }

    // --- SetStatus ---

    [Fact]
    public void SetStatus_ReturnsSameInstance() {
        // act
        var result = NullActivity.Instance.SetStatus(ActivityStatusCode.Ok, "OK");

        // assert
        Assert.Same(NullActivity.Instance, result);
    }

    // --- Dispose ---

    [Fact]
    public void Dispose_DoesNotThrow() {
        // act & assert (no exception)
        NullActivity.Instance.Dispose();
    }
}
