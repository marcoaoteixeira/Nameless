using Shouldly;

namespace Nameless;

public class PreventArgumentNullStructTests {
    [Test]
    public void WhenNonNullParamValue_ThenDoNotThrows()
        // arrange & act & assert
        => Assert.DoesNotThrow(() => Prevent.Argument.Null<int>(paramValue: 1));

    [Test]
    public void WhenNullParamValue_ThenThrowsArgumentNullException()
        // arrange & act & assert
        => Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null<int>(paramValue: null));

    [Test]
    public void WhenNullParamValue_WithCustomMessage_ThenThrowsWithCustomMessage() {
        const string message = "Parameter is null";

        // arrange & act & assert
        Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null<int>(paramValue: null, message: message))
              .ShouldNotBeNull()
              .Message
              .ShouldContain(message);
    }

    [Test]
    public void WhenNullParamValue_WithCustomExceptionCreator_ThenThrowsCustomException()
        // arrange & act & assert
        => Assert.Throws<InvalidOperationException>(() => Prevent.Argument.Null<int>(paramValue: null, exceptionCreator: () => new InvalidOperationException()));

    [Test]
    public void WhenNullParamValue_WithOmittedParamName_ThenExceptionMessageContainsCallerParamName() {
        // arrange & act & assert
        int? value = null;

        Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null(paramValue: value))
              .ShouldNotBeNull()
              .Message
              .ShouldContain(nameof(value));
    }

    [Test]
    public void WhenNullParamValue_WithParamName_ThenExceptionMessageContainsParamName() {
        // arrange & act & assert
        const string paramName = "test_parameter";

        Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null<int>(paramValue: null, paramName: paramName))
              .ShouldNotBeNull()
              .Message
              .ShouldContain(paramName);
    }
}