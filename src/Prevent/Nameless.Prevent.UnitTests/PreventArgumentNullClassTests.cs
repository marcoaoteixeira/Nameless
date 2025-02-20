using Shouldly;

namespace Nameless;

public class PreventArgumentNullClassTests {
    [Test]
    public void WhenNonNullParamValue_ThenDoNotThrows()
        // arrage & act & assert
        => Assert.DoesNotThrow(() => Prevent.Argument.Null<object>(paramValue: 1));

    [Test]
    public void WhenNullParamValue_ThenThrowsArgumentNullException()
        // arrage & act & assert
        => Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null<object>(paramValue: null));

    [Test]
    public void WhenNullParamValue_WithCustomMessage_ThenThrowsWithCustomMessage() {
        const string message = "Parameter is null";

        // arrage & act & assert
        Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null<object>(paramValue: null, message: message))
              .ShouldNotBeNull()
              .Message
              .ShouldContain(message);
    }

    [Test]
    public void WhenNullParamValue_WithCustomExceptionCreator_ThenThrowsCustomException()
        // arrage & act & assert
        => Assert.Throws<InvalidOperationException>(() => Prevent.Argument.Null<object>(paramValue: null, exceptionCreator: () => new InvalidOperationException()));

    [Test]
    public void WhenNullParamValue_WithOmittedParamName_ThenExceptionMessageContainsCallerParamName() {
        // arrage & act & assert
        object user = null;

        Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null(paramValue: user))
              .ShouldNotBeNull()
              .Message
              .ShouldContain(nameof(user));
    }

    [Test]
    public void WhenNullParamValue_WithParamName_ThenExceptionMessageContainsParamName() {
        // arrage & act & assert
        object user = null;
        const string paramName = "test_parameter";

        Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null(paramValue: user, paramName: paramName))
              .ShouldNotBeNull()
              .Message
              .ShouldContain(paramName);
    }
}