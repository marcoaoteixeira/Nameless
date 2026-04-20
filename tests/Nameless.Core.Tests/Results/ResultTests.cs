using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless;

public class ResultTests {
    // ─── Parameterless constructor ───────────────────────────────────────────

    [Fact]
    public void DefaultConstructor_ThrowsInvalidOperationException() {
        // act & assert
        Assert.Throws<InvalidOperationException>(() => new Result<string>());
    }

    // ─── Implicit conversion from T ─────────────────────────────────────────

    [Fact]
    public void ImplicitConversion_FromValue_SetsSuccessTrue() {
        // act
        Result<string> result = "hello";

        // assert
        Assert.True(result.Success);
    }

    [Fact]
    public void ImplicitConversion_FromValue_ValueReturnsValue() {
        // act
        Result<string> result = "hello";

        // assert
        Assert.Equal("hello", result.Value);
    }

    [Fact]
    public void Value_OnFailureResult_ThrowsInvalidOperationException() {
        // arrange
        Result<string> result = new[] { Error.Validation("err") };

        // act & assert
        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }

    // ─── Implicit conversion from Error[] ───────────────────────────────────

    [Fact]
    public void ImplicitConversion_FromErrorArray_SetsSuccessFalse() {
        // arrange
        var errors = new[] { Error.Validation("e1"), Error.Missing("e2") };

        // act
        Result<string> result = errors;

        // assert
        Assert.False(result.Success);
    }

    [Fact]
    public void ImplicitConversion_FromErrorArray_ErrorsReturnsErrors() {
        // arrange
        var errors = new[] { Error.Validation("e1"), Error.Missing("e2") };

        // act
        Result<string> result = errors;

        // assert
        Assert.Equal(2, result.Errors.Length);
    }

    // ─── Implicit conversion from Error ─────────────────────────────────────

    [Fact]
    public void ImplicitConversion_FromSingleError_SetsSuccessFalse() {
        // act
        Result<string> result = Error.Failure("oops");

        // assert
        Assert.False(result.Success);
    }

    [Fact]
    public void ImplicitConversion_FromSingleError_ContainsOneError() {
        // act
        Result<string> result = Error.Failure("oops");

        // assert
        Assert.Single(result.Errors);
    }

    [Fact]
    public void Errors_OnSuccessResult_ThrowsInvalidOperationException() {
        // arrange
        Result<string> result = "ok";

        // act & assert
        Assert.Throws<InvalidOperationException>(() => _ = result.Errors);
    }

    // ─── Match(Action, Action) ───────────────────────────────────────────────

    [Fact]
    public void Match_Action_WhenSuccess_CallsOnSuccess() {
        // arrange
        Result<string> result = "hello";
        var successCalled = false;
        var failureCalled = false;

        // act
        result.Match(
            _ => { successCalled = true; },
            _ => { failureCalled = true; }
        );

        // assert
        Assert.Multiple(() => {
            Assert.True(successCalled);
            Assert.False(failureCalled);
        });
    }

    [Fact]
    public void Match_Action_WhenFailure_CallsOnFailure() {
        // arrange
        Result<string> result = Error.Validation("err");
        var successCalled = false;
        var failureCalled = false;

        // act
        result.Match(
            _ => { successCalled = true; },
            _ => { failureCalled = true; }
        );

        // assert
        Assert.Multiple(() => {
            Assert.False(successCalled);
            Assert.True(failureCalled);
        });
    }

    // ─── Match(Func, Func) ───────────────────────────────────────────────────

    [Fact]
    public void Match_Func_WhenSuccess_ReturnsOnSuccessResult() {
        // arrange
        Result<string> result = "hello";

        // act
        var output = result.Match(
            onSuccess: v => $"ok:{v}",
            onFailure: _ => "fail"
        );

        // assert
        Assert.Equal("ok:hello", output);
    }

    [Fact]
    public void Match_Func_WhenFailure_ReturnsOnFailureResult() {
        // arrange
        Result<string> result = Error.Missing("not found");

        // act
        var output = result.Match(
            onSuccess: _ => "ok",
            onFailure: errs => $"fail:{errs.Length}"
        );

        // assert
        Assert.Equal("fail:1", output);
    }
}
