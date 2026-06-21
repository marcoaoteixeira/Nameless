using System.Diagnostics;
using Nameless.Diagnostics;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Diagnostics;

[UnitTest]
public class ActivityWrapperTests {
    // Registers a listener that accepts all activities so that ActivitySource.StartActivity
    // returns a real Activity rather than null.
    private static ActivityListener CreateAcceptAllListener() {
        return new ActivityListener {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
        };
    }

    // Creates a real Activity via ActivitySource so that ActivityWrapper has a
    // non-null inner instance to work with.
    private static Activity CreateRealActivity(ActivitySource source, string operationName = "test-op") {
        var activity = source.StartActivity(operationName);

        // If StartActivity returns null (no listener registered), the test setup is wrong.
        Assert.NotNull(activity);

        return activity;
    }

    [Fact]
    public void SetTag_DoesNotThrow() {
        // arrange
        using var source = new ActivitySource("test.wrapper.settag");
        using var listener = CreateAcceptAllListener();
        ActivitySource.AddActivityListener(listener);

        using var innerActivity = CreateRealActivity(source);
        var sut = new ActivityWrapper(innerActivity);

        // act
        var exception = Record.Exception(() => sut.SetTag("key", "value"));

        // assert
        Assert.Null(exception);

        sut.Dispose();
    }

    [Fact]
    public void AddException_DoesNotThrow() {
        // arrange
        using var source = new ActivitySource("test.wrapper.addexception");
        using var listener = CreateAcceptAllListener();
        ActivitySource.AddActivityListener(listener);

        using var innerActivity = CreateRealActivity(source);
        var sut = new ActivityWrapper(innerActivity);

        // act
        var exception = Record.Exception(() => sut.AddException(new InvalidOperationException("boom")));

        // assert
        Assert.Null(exception);

        sut.Dispose();
    }

    [Fact]
    public void SetStatus_WithOkStatus_DoesNotThrow() {
        // arrange
        using var source = new ActivitySource("test.wrapper.setstatus");
        using var listener = CreateAcceptAllListener();
        ActivitySource.AddActivityListener(listener);

        using var innerActivity = CreateRealActivity(source);
        var sut = new ActivityWrapper(innerActivity);

        // act
        var exception = Record.Exception(() => sut.SetStatus(ActivityStatusCode.Ok, description: null));

        // assert
        Assert.Null(exception);

        sut.Dispose();
    }

    [Fact]
    public void Dispose_DoesNotThrow() {
        // arrange
        using var source = new ActivitySource("test.wrapper.dispose");
        using var listener = CreateAcceptAllListener();
        ActivitySource.AddActivityListener(listener);

        var innerActivity = CreateRealActivity(source);
        var sut = new ActivityWrapper(innerActivity);

        // act
        var exception = Record.Exception(sut.Dispose);

        // assert
        Assert.Null(exception);
    }
}
