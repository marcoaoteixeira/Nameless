using Shouldly;

namespace Nameless;

public class PreventArgumentNullOrEmptyTests {
    private readonly IEnumerable<object> _nullEnumerable = null;
    private readonly IEnumerable<object> _emptyEnumerable = [];
    private readonly IEnumerable<object> _nonEmptyEnumerable = [1, 2, 3];

    [Test]
    public void WhenNonNullOrEmptyParamValue_ThenDoNotThrows()
        // arrage & act & assert
        => Assert.DoesNotThrow(() => Prevent.Argument.NullOrEmpty(paramValue: _nonEmptyEnumerable));

    [Test]
    public void WhenNullParamValue_ThenThrowsArgumentNullException()
        // arrage & act & assert
        => Assert.Throws<ArgumentNullException>(() => Prevent.Argument.NullOrEmpty(paramValue: _nullEnumerable));

    [Test]
    public void WhenEmptyParamValue_ThenThrowsArgumentException()
        // arrage & act & assert
        => Assert.Throws<ArgumentException>(() => Prevent.Argument.NullOrEmpty(paramValue: _emptyEnumerable));

    [Test]
    public void WhenNullParamValue_WithCustomMessage_ThenThrowsWithCustomMessage() {
        const string message = "Parameter is null";

        // arrage & act & assert
        Assert.Throws<ArgumentNullException>(() => Prevent.Argument.NullOrEmpty(paramValue: _nullEnumerable, message: message))
              .ShouldNotBeNull()
              .Message
              .ShouldContain(message);
    }

    [Test]
    public void WhenEmptyParamValue_WithCustomMessage_ThenThrowsWithCustomMessage() {
        const string message = "Parameter is empty";

        // arrage & act & assert
        Assert.Throws<ArgumentException>(() => Prevent.Argument.NullOrEmpty(paramValue: _emptyEnumerable, message: message))
              .ShouldNotBeNull()
              .Message
              .ShouldContain(message);
    }

    [Test]
    public void WhenNullParamValue_WithCustomExceptionCreator_ThenThrowsCustomException()
        // arrage & act & assert
        => Assert.Throws<InvalidOperationException>(() => Prevent.Argument.NullOrEmpty(paramValue: _nullEnumerable, exceptionCreator: () => new InvalidOperationException()));

    [Test]
    public void WhenEmptyParamValue_WithCustomExceptionCreator_ThenThrowsCustomException()
        // arrage & act & assert
        => Assert.Throws<InvalidOperationException>(() => Prevent.Argument.NullOrEmpty(paramValue: _emptyEnumerable, exceptionCreator: () => new InvalidOperationException()));

    [Test]
    public void WhenNullParamValue_WithOmittedParamName_ThenExceptionMessageContainsCallerParamName() {
        // arrage & act & assert
        Assert.Throws<ArgumentNullException>(() => Prevent.Argument.NullOrEmpty(paramValue: _nullEnumerable))
              .ShouldNotBeNull()
              .Message
              .ShouldContain(nameof(_nullEnumerable));
    }

    [Test]
    public void WhenEmptyParamValue_WithOmittedParamName_ThenExceptionMessageContainsCallerParamName() {
        // arrage & act & assert
        Assert.Throws<ArgumentException>(() => Prevent.Argument.NullOrEmpty(paramValue: _emptyEnumerable))
              .ShouldNotBeNull()
              .Message
              .ShouldContain(nameof(_emptyEnumerable));
    }

    [Test]
    public void WhenNullParamValue_WithParamName_ThenExceptionMessageContainsParamName() {
        // arrage & act & assert
        const string paramName = "test_parameter";

        Assert.Throws<ArgumentNullException>(() => Prevent.Argument.NullOrEmpty(paramValue: _nullEnumerable, paramName: paramName))
              .ShouldNotBeNull()
              .Message
              .ShouldContain(paramName);
    }

    [Test]
    public void WhenEmptyParamValue_WithParamName_ThenExceptionMessageContainsParamName() {
        // arrage & act & assert
        const string paramName = "test_parameter";

        Assert.Throws<ArgumentException>(() => Prevent.Argument.NullOrEmpty(paramValue: _emptyEnumerable, paramName: paramName))
              .ShouldNotBeNull()
              .Message
              .ShouldContain(paramName);
    }
}