using Newtonsoft.Json.Linq;

namespace Nameless.Result;

public class ResultTests {
    [Test]
    public void WhenResultHasValue_ThenSucceededMustBeTrue_AndValueShouldBeExpected() {
        // arrange
        const int expected = 123;
        Result<int> result;

        // act
        result = expected;

        // assert
        Assert.Multiple(() => {
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Value, Is.EqualTo(expected));
        });
    }

    [Test]
    public void WhenResultIsError_ThenSucceededMustBeFalse_WhenErrorsContainsAtLeastOneError() {
        // arrange
        var expected = Error.Failure("Error");
        Result<int> result;

        // act
        result = expected;

        // assert
        Assert.Multiple(() => {
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Errors, Has.Length.AtLeast(1));
        });
    }

    [Test]
    public void WhenResultIsError_ThenSucceededMustBeFalse_WhenErrorsContainsMoreThanOneError() {
        // arrange
        var expected = new[] { Error.Failure("Error"), Error.Conflict("Error") };
        Result<int> result;

        // act
        result = expected;

        // assert
        Assert.Multiple(() => {
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Errors, Has.Length.AtLeast(2));
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
            Assert.That(result.Succeeded, Is.True);
            Assert.Throws<InvalidOperationException>(() => _ = result.Errors);
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
        Assert.Multiple(() => {
            Assert.That(result.Succeeded, Is.False);
            Assert.Throws<InvalidOperationException>(() => _ = result.Value);
        });
    }

    [Test]
    public void WhenResultIsValid_ThenMatchShouldAccessSuccessActionWithReturningValue() {
        // arrange
        var result = (Result<int>)123;

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
        var result = (Result<int>)123;

        // act
        var match = await result.MatchAsync(SuccessActionAsync, FailureActionAsync);

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
        var result = (Result<int>)Error.Failure("Error");

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
        var result = (Result<int>)Error.Failure("Error");

        // act
        var match = await result.MatchAsync(SuccessActionAsync, FailureActionAsync);

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
        var result = (Result<int>)123;
        object captured = null;

        // act
        result.Match(SuccessAction, FailureAction);

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
        var result = (Result<int>)123;
        object captured = null;

        // act
        await result.MatchAsync(SuccessActionAsync, FailureActionAsync);

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
        var result = (Result<int>)Error.Failure("Error");
        object captured = null;

        // act
        result.Match(SuccessAction, FailureAction);

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
        var result = (Result<int>)Error.Failure("Error");
        object captured = null;

        // act
        await result.MatchAsync(SuccessActionAsync, FailureActionAsync);

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
        var result = (Result<int>)123;

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
}
