namespace Nameless.ObjectModel;

[UnitTest]
public class ErrorCollectionExtensionsTests {
    [Fact]
    public void OperationCanceled_ReturnsFailureCarryingException() {
        // arrange
        var exception = new OperationCanceledException();

        // act
        var actual = Error.OperationCanceled(exception);

        // assert
        Assert.Multiple(
            () => Assert.Equal(ErrorType.Failure, actual.Type),
            () => Assert.Same(exception, actual.Exception)
        );
    }

    [Fact]
    public void OperationCanceled_WithoutException_ReturnsFailure() {
        // act
        var actual = Error.OperationCanceled();

        // assert
        Assert.Null(actual.Exception);
    }

    [Fact]
    public void Flatten_JoinsEveryErrorFlattenedForm() {
        // arrange
        Error[] errors = [Error.Validation("bad", "E1"), Error.Missing("gone")];

        // act
        var actual = errors.Flatten();

        // assert
        Assert.Equal("[Validation] (E1) bad | [Missing] gone", actual);
    }

    [Fact]
    public void ToDictionary_GroupsMessagesByCode() {
        // arrange
        Error[] errors = [Error.Validation("a", "E1"), Error.Validation("b", "E1"), Error.Failure("c")];

        // act
        var actual = errors.ToDictionary();

        // assert
        Assert.Multiple(
            () => Assert.Equal(["a", "b"], actual["E1"]),
            () => Assert.Equal(["c"], actual[string.Empty])
        );
    }

    [Fact]
    public void Error_DefaultConstructor_Throws() {
        // act & assert
        Assert.Throws<InvalidOperationException>(() => new Error());
    }

    [Fact]
    public void Error_Factories_SetTypeAndCode() {
        // act & assert
        Assert.Multiple(
            () => Assert.Equal(ErrorType.Conflict, Error.Conflict("m", "c").Type),
            () => Assert.Equal(ErrorType.Forbidden, Error.Forbidden("m").Type),
            () => Assert.Equal(ErrorType.Unauthorized, Error.Unauthorized("m").Type),
            () => Assert.Equal("c", Error.Conflict("m", "c").Code)
        );
    }
}
