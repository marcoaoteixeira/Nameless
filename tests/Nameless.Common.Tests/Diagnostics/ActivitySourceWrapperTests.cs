using System.Diagnostics;
using Nameless.Diagnostics.ActivitySource;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Diagnostics;

[UnitTest]
public class ActivitySourceWrapperTests {
    // Register a listener that accepts all activities so that StartActivity
    // returns a real Activity (not null) during the test.
    private static ActivityListener CreateAcceptAllListener() {
        return new ActivityListener {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
        };
    }

    [Fact]
    public void StartActivity_ReturnsIActivity() {
        // arrange
        using var innerSource = new System.Diagnostics.ActivitySource("test.source.start");
        using var listener = CreateAcceptAllListener();
        System.Diagnostics.ActivitySource.AddActivityListener(listener);

        var sut = new ActivitySourceWrapper(innerSource);

        // act
        using var activity = sut.StartActivity("op", ActivityKind.Internal, parentContext: null);

        // assert
        Assert.NotNull(activity);
    }

    [Fact]
    public void Dispose_FiresOnDisposeEvent() {
        // arrange
        using var innerSource = new System.Diagnostics.ActivitySource("test.source.dispose");
        var sut = new ActivitySourceWrapper(innerSource);

        IActivitySource? capturedSource = null;
        sut.OnDispose += src => capturedSource = src;

        // act
        sut.Dispose();

        // assert
        Assert.NotNull(capturedSource);
        Assert.Same(sut, capturedSource);
    }

    [Fact]
    public void Dispose_CalledTwice_DoesNotThrow() {
        // arrange
        using var innerSource = new System.Diagnostics.ActivitySource("test.source.double-dispose");
        var sut = new ActivitySourceWrapper(innerSource);

        sut.Dispose(); // first call

        // act
        var exception = Record.Exception(sut.Dispose); // second call

        // assert
        Assert.Null(exception);
    }
}
