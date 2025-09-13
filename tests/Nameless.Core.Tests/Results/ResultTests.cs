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
            Assert.False(result.IsError);
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
            Assert.True(result.IsError);
            Assert.Equal(Message, result.AsError.Description);
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
            Assert.False(result.IsError);
            Assert.Throws<InvalidOperationException>(() => _ = result.AsError);
        });
    }

    [Fact]
    public void WhenResultIsError_ThenAccessValueThrowsInvalidOperationException() {
        // arrange
        var expected = Error.Failure(description: "Error");
        Result<int> result;

        // act
        result = expected;

        // assert
        Assert.True(result.IsError);
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

        bool SuccessAction(int value) {
            return true;
        }

        bool FailureAction(Error error) {
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

        Task<bool> SuccessActionAsync(int value) {
            return Task.FromResult(result: true);
        }

        Task<bool> FailureActionAsync(Error error) {
            return Task.FromResult(result: false);
        }
    }

    [Fact]
    public void WhenResultIsError_ThenMatchShouldAccessFailureActionWithReturningValue() {
        // arrange
        Result<int> result = Error.Failure(description: "Error");

        // act
        var match = result.Match(SuccessAction, FailureAction);

        // assert
        Assert.False(match);

        return;

        bool SuccessAction(int value) {
            return true;
        }

        bool FailureAction(Error error) {
            return false;
        }
    }

    [Fact]
    public async Task WhenResultIsError_ThenMatchAsyncShouldAccessFailureActionAsyncWithReturningValue() {
        // arrange
        Result<int> result = Error.Failure(description: "Error");

        // act
        var match = await result.Match(SuccessActionAsync, FailureActionAsync);

        // assert
        Assert.False(match);

        return;

        Task<bool> SuccessActionAsync(int value) {
            return Task.FromResult(result: true);
        }

        Task<bool> FailureActionAsync(Error error) {
            return Task.FromResult(result: false);
        }
    }

    [Fact]
    public void WhenResultIsValid_ThenMatchShouldAccessSuccessActionWithoutReturningValue() {
        // arrange
        Result<int> result = 123;
        object captured = null;

        // act
        result.Switch(SuccessAction, FailureAction);

        // assert
        Assert.True((bool)captured);

        return;

        void SuccessAction(int value) {
            captured = true;
        }

        void FailureAction(Error error) {
            captured = false;
        }
    }

    [Fact]
    public async Task WhenResultIsValid_ThenMatchAsyncShouldAccessSuccessActionAsyncWithoutReturningValue() {
        // arrange
        Result<int> result = 123;
        object captured = null;

        // act
        await result.Switch(SuccessActionAsync, FailureActionAsync);

        // assert
        Assert.True((bool)captured);

        return;

        Task SuccessActionAsync(int value) {
            captured = true;

            return Task.CompletedTask;
        }

        Task FailureActionAsync(Error error) {
            captured = false;

            return Task.CompletedTask;
        }
    }

    [Fact]
    public void WhenResultIsError_ThenMatchShouldAccessFailureActionWithoutReturningValue() {
        // arrange
        Result<int> result = Error.Failure(description: "Error");
        object captured = null;

        // act
        result.Switch(SuccessAction, FailureAction);

        // assert
        Assert.False((bool)captured);

        return;

        void SuccessAction(int value) {
            captured = true;
        }

        void FailureAction(Error error) {
            captured = false;
        }
    }

    [Fact]
    public async Task WhenResultIsError_ThenMatchAsyncShouldAccessFailureActionAsyncWithoutReturningValue() {
        // arrange
        Result<int> result = Error.Failure(description: "Error");
        object captured = null;

        // act
        await result.Switch(SuccessActionAsync, FailureActionAsync);

        // assert
        Assert.False((bool)captured);

        return;

        Task SuccessActionAsync(int value) {
            captured = true;

            return Task.CompletedTask;
        }

        Task FailureActionAsync(Error error) {
            captured = false;

            return Task.CompletedTask;
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

        Result<bool> SuccessAction(int value) {
            return value % 2 == 0;
        }

        Result<bool> FailureAction(Error error) {
            return true;
        }

        bool FinalSuccessAction(bool value) {
            return value;
        }

        bool FinalFailureAction(Error error) {
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

        Result<bool> FirstSuccessAction(int value) {
            return Error.Conflict(description: "Conflict");
        }

        Result<bool> FirstFailureAction(Error error) {
            return true;
        }

        Result<string> SecondSuccessAction(bool value) {
            return value.ToString();
        }

        Result<string> SecondFailureAction(Error error) {
            return error;
        }

        bool FinalSuccessAction(string value) {
            return value == "Hello world";
        }

        bool FinalFailureAction(Error error) {
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
            Assert.False(result.IsError);
            Assert.Null(result.Value);
        });
    }
}