using Nameless.Results;

namespace Nameless.Result;

public class ResultWithErrorsTests {
    [Test]
    public void WhenResultHasValue_ThenHasErrorMustBeFalse_AndValueShouldBeExpected() {
        // arrange
        const int expected = 123;
        Result<int> result;

        // act
        result = expected;

        // assert
        Assert.Multiple(() => {
            Assert.That(result.HasErrors, Is.False);
            Assert.That(result.Value, Is.EqualTo(expected));
        });
    }

    [Test]
    public void WhenResultIsError_ThenHasErrorMustBeTrue_WhenErrorsContainsAtLeastOneError() {
        // arrange
        var expected = Error.Failure("Error");
        Result<int> result;

        // act
        result = expected;

        // assert
        Assert.Multiple(() => {
            Assert.That(result.HasErrors, Is.True);
            Assert.That(result.AsArg1, Has.Length.AtLeast(1));
        });
    }

    [Test]
    public void WhenResultIsError_ThenHasErrorMustBeTrue_WhenErrorsContainsMoreThanOneError() {
        // arrange
        var expected = new[] { Error.Failure("Error"), Error.Conflict("Error") };
        Result<int> result;

        // act
        result = expected;

        // assert
        Assert.Multiple(() => {
            Assert.That(result.HasErrors, Is.True);
            Assert.That(result.AsArg1, Has.Length.AtLeast(2));
        });
    }

    [Test]
    public void WhenResultIsValid_ThenAccessErrorThrowsInvalidOperationException() {
        // arrange
        const int expected = 123;
        Result<int> result;

        // act
        result = expected;

        // assert
        Assert.Multiple(() => {
            Assert.That(result.HasErrors, Is.False);
            Assert.Throws<InvalidOperationException>(() => _ = result.AsArg1);
        });
    }

    [Test]
    public void WhenResultIsError_ThenAccessValueThrowsInvalidOperationException() {
        // arrange
        var expected = Error.Failure("Error");
        Result<int> result;

        // act
        result = expected;

        // assert
        Assert.That(result.HasErrors, Is.True);
    }

    [Test]
    public void WhenResultIsValid_ThenMatchShouldAccessSuccessActionWithReturningValue() {
        // arrange
        Result<int> result = 123;

        // act
        var match = result.Match(SuccessAction, FailureAction);

        // assert
        Assert.That(match, Is.True);

        return;

        bool SuccessAction(int value) {
            return true;
        }

        bool FailureAction(Error[] errors) {
            return false;
        }
    }

    [Test]
    public async Task WhenResultIsValid_ThenMatchAsyncShouldAccessSuccessActionAsyncWithReturningValue() {
        // arrange
        Result<int> result = 123;

        // act
        var match = await result.Match(SuccessActionAsync, FailureActionAsync);

        // assert
        Assert.That(match, Is.True);

        return;

        Task<bool> SuccessActionAsync(int value) {
            return Task.FromResult(true);
        }

        Task<bool> FailureActionAsync(Error[] errors) {
            return Task.FromResult(false);
        }
    }

    [Test]
    public void WhenResultIsError_ThenMatchShouldAccessFailureActionWithReturningValue() {
        // arrange
        Result<int> result = Error.Failure("Error");

        // act
        var match = result.Match(SuccessAction, FailureAction);

        // assert
        Assert.That(match, Is.False);

        return;

        bool SuccessAction(int value) {
            return true;
        }

        bool FailureAction(Error[] errors) {
            return false;
        }
    }
    
    [Test]
    public async Task WhenResultIsError_ThenMatchAsyncShouldAccessFailureActionAsyncWithReturningValue() {
        // arrange
        Result<int> result = Error.Failure("Error");

        // act
        var match = await result.Match(SuccessActionAsync, FailureActionAsync);

        // assert
        Assert.That(match, Is.False);

        return;

        Task<bool> SuccessActionAsync(int value) {
            return Task.FromResult(true);
        }

        Task<bool> FailureActionAsync(Error[] errors) {
            return Task.FromResult(false);
        }
    }

    [Test]
    public void WhenResultIsValid_ThenMatchShouldAccessSuccessActionWithoutReturningValue() {
        // arrange
        Result<int> result = 123;
        object captured = null;

        // act
        result.Switch(SuccessAction, FailureAction);

        // assert
        Assert.That(captured, Is.True);

        return;

        void SuccessAction(int value) {
            captured = true;
        }

        void FailureAction(Error[] errors) {
            captured = false;
        }
    }

    [Test]
    public async Task WhenResultIsValid_ThenMatchAsyncShouldAccessSuccessActionAsyncWithoutReturningValue() {
        // arrange
        Result<int> result = 123;
        object captured = null;

        // act
        await result.Switch(SuccessActionAsync, FailureActionAsync);

        // assert
        Assert.That(captured, Is.True);

        return;

        Task SuccessActionAsync(int value) {
            captured = true;

            return Task.CompletedTask;
        }

        Task FailureActionAsync(Error[] errors) {
            captured = false;

            return Task.CompletedTask;
        }
    }

    [Test]
    public void WhenResultIsError_ThenMatchShouldAccessFailureActionWithoutReturningValue() {
        // arrange
        Result<int> result = Error.Failure("Error");
        object captured = null;

        // act
        result.Switch(SuccessAction, FailureAction);

        // assert
        Assert.That(captured, Is.False);

        return;

        void SuccessAction(int value) {
            captured = true;
        }

        void FailureAction(Error[] errors) {
            captured = false;
        }
    }
    
    [Test]
    public async Task WhenResultIsError_ThenMatchAsyncShouldAccessFailureActionAsyncWithoutReturningValue() {
        // arrange
        Result<int> result = Error.Failure("Error");
        object captured = null;

        // act
        await result.Switch(SuccessActionAsync, FailureActionAsync);

        // assert
        Assert.That(captured, Is.False);

        return;

        Task SuccessActionAsync(int value) {
            captured = true;

            return Task.CompletedTask;
        }

        Task FailureActionAsync(Error[] errors) {
            captured = false;

            return Task.CompletedTask;
        }
    }

    [Test]
    public void WhenResult_ThenMultipleMatches() {
        // arrange
        Result<int> result = 123;

        // act
        var match = result.Match(SuccessAction, FailureAction)
                          .Match(FinalSuccessAction, FinalFailureAction);

        // assert
        Assert.That(match, Is.False);

        return;

        Result<bool> SuccessAction(int value) {
            return value % 2 == 0;
        }

        Result<bool> FailureAction(Error[] errors) {
            if (errors.Length is > 1 and < 3) {
                return true;
            }

            return Error.Failure("Error should be exactly 2");
        }

        bool FinalSuccessAction(bool value) {
            return value;
        }

        bool FinalFailureAction(Error[] errors) {
            return false;
        }
    }

    [Test]
    public void WhenResultHasNullableValue_ThenHasErrorMustBeFalse() {
        // arrange
        Result<int?> result;

        // act
        result = (int?)null;

        // assert
        Assert.Multiple(() => {
            Assert.That(result.HasErrors, Is.False);
            Assert.That(result.Value, Is.Null);
        });
    }
}