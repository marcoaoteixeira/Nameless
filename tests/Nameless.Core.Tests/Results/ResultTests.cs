using Nameless.ObjectModel;

namespace Nameless.Results;

public class ResultTests {
    [Fact]
    public void WhenResultHasValue_ThenHasErrorMustBeFalse_AndValueShouldBeExpected() {
        // arrange
        const int Expected = 123;
        Result<int> result;

        // act
        result = Expected;

        // assert
        Assert.Multiple(() => {
            Assert.True(result.Success);
            Assert.Equal(Expected, result.Value);
        });
    }

    [Fact]
    public void WhenResultIsError_ThenHasErrorMustBeTrue_WhenErrorsContainsAtLeastOneError() {
        // arrange
        const string Message = "Error";
        var expected = Error.Failure(Message);
        Result<int> result;

        // act
        result = expected;

        // assert
        Assert.Multiple(() => {
            Assert.False(result.Success);
            Assert.Equal(expected, result.Errors[0]);
        });
    }

    [Fact]
    public void WhenResultIsValid_ThenAccessErrorThrowsInvalidOperationException() {
        // arrange
        const int Expected = 123;
        Result<int> result;

        // act
        result = Expected;

        // assert
        Assert.Multiple(() => {
            Assert.True(result.Success);
            Assert.Throws<InvalidOperationException>(() => _ = result.Errors);
        });
    }

    [Fact]
    public void WhenResultIsError_ThenAccessValueThrowsInvalidOperationException() {
        // arrange
        var expected = Error.Failure(message: "Error");
        Result<int> result;

        // act
        result = expected;

        // assert
        Assert.False(result.Success);
    }

    [Fact]
    public void WhenResultIsValid_ThenMatchShouldAccessSuccessActionWithReturningValue() {
        // arrange
        Result<int> result = 123;

        // act
        var match = result.Match(SuccessAction, FailureAction);

        // assert
        Assert.True(match);

        return;

        static bool SuccessAction(int value) {
            return true;
        }

        static bool FailureAction(Error[] error) {
            return false;
        }
    }

    [Fact]
    public async Task WhenResultIsValid_ThenMatchAsyncShouldAccessSuccessActionAsyncWithReturningValue() {
        // arrange
        Result<int> result = 123;

        // act
        var match = await result.Match(SuccessActionAsync, FailureActionAsync);

        // assert
        Assert.True(match);

        return;

        static Task<bool> SuccessActionAsync(int value) {
            return Task.FromResult(result: true);
        }

        static Task<bool> FailureActionAsync(Error[] error) {
            return Task.FromResult(result: false);
        }
    }

    [Fact]
    public void WhenResultIsError_ThenMatchShouldAccessFailureActionWithReturningValue() {
        // arrange
        Result<int> result = Error.Failure(message: "Error");

        // act
        var match = result.Match(SuccessAction, FailureAction);

        // assert
        Assert.False(match);

        return;

        static bool SuccessAction(int value) {
            return true;
        }

        static bool FailureAction(Error[] error) {
            return false;
        }
    }

    [Fact]
    public async Task WhenResultIsError_ThenMatchAsyncShouldAccessFailureActionAsyncWithReturningValue() {
        // arrange
        Result<int> result = Error.Failure(message: "Error");

        // act
        var match = await result.Match(SuccessActionAsync, FailureActionAsync);

        // assert
        Assert.False(match);

        return;

        static Task<bool> SuccessActionAsync(int value) {
            return Task.FromResult(result: true);
        }

        static Task<bool> FailureActionAsync(Error[] error) {
            return Task.FromResult(result: false);
        }
    }
    
    [Fact]
    public void WhenResult_ThenMultipleMatches() {
        // arrange
        Result<int> result = 123;

        // act
        var match = result.Match(SuccessAction, FailureAction)
                          .Match(FinalSuccessAction, FinalFailureAction);

        // assert
        Assert.False(match);

        return;

        static Result<bool> SuccessAction(int value) {
            return value % 2 == 0;
        }

        static Result<bool> FailureAction(Error[] error) {
            return true;
        }

        static bool FinalSuccessAction(bool value) {
            return value;
        }

        static bool FinalFailureAction(Error[] error) {
            return false;
        }
    }

    [Fact]
    public void Result_With_Three_Matches() {
        // arrange
        Result<int> result = 123;

        // act
        var match = result.Match(FirstSuccessAction, FirstFailureAction)
                          .Match(SecondSuccessAction, SecondFailureAction)
                          .Match(FinalSuccessAction, FinalFailureAction);

        // assert
        Assert.True(match);

        return;

        static Result<bool> FirstSuccessAction(int value) {
            return Error.Conflict(message: "Conflict");
        }

        static Result<bool> FirstFailureAction(Error[] error) {
            return true;
        }

        static Result<string> SecondSuccessAction(bool value) {
            return value.ToString();
        }

        static Result<string> SecondFailureAction(Error[] error) {
            return error;
        }

        static bool FinalSuccessAction(string value) {
            return value == "Hello world";
        }

        static bool FinalFailureAction(Error[] error) {
            return true;
        }
    }

    [Fact]
    public void WhenResultHasNullableValue_ThenHasErrorMustBeFalse() {
        // arrange
        Result<int?> result;

        // act
        result = (int?)null;

        // assert
        Assert.Multiple(() => {
            Assert.True(result.Success);
            Assert.Null(result.Value);
        });
    }
}