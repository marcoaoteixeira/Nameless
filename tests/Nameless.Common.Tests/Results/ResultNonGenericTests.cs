using Nameless.ObjectModel;

namespace Nameless.Results;

[UnitTest]
public class ResultNonGenericTests {
    [Fact]
    public void ImplicitFromError_CreatesFailedResult() {
        // act
        Result sut = Error.Failure("boom");

        // assert
        Assert.Multiple(
            () => Assert.False(sut.Success),
            () => Assert.Single(sut.Errors)
        );
    }

    [Fact]
    public void ImplicitFromErrors_WithEmptyArray_CreatesSuccessfulResult() {
        // act
        Result sut = Array.Empty<Error>();

        // assert
        Assert.True(sut.Success);
    }

    [Fact]
    public void Errors_WhenSuccessful_Throws() {
        // arrange
        Result sut = Array.Empty<Error>();

        // act & assert
        Assert.Throws<InvalidOperationException>(() => sut.Errors);
    }

    [Fact]
    public void Match_Action_InvokesOnSuccess() {
        // arrange
        Result sut = Array.Empty<Error>();
        var invoked = "";

        // act
        sut.Match(() => invoked = "success", _ => invoked = "failure");

        // assert
        Assert.Equal("success", invoked);
    }

    [Fact]
    public void Match_Action_InvokesOnFailureWithErrors() {
        // arrange
        Result sut = Error.Failure("boom");
        Error[]? received = null;

        // act
        sut.Match(() => { }, errors => received = errors);

        // assert
        Assert.Single(received!);
    }

    [Fact]
    public void Match_Func_ReturnsBranchValue() {
        // arrange
        Result success = Array.Empty<Error>();
        Result failure = Error.Failure("boom");

        // act & assert
        Assert.Multiple(
            () => Assert.Equal("ok", success.Match(() => "ok", _ => "ko")),
            () => Assert.Equal("ko", failure.Match(() => "ok", _ => "ko"))
        );
    }

    [Fact]
    public void ParameterlessConstructors_Throw() {
        // act & assert
        Assert.Multiple(
            () => Assert.Throws<InvalidOperationException>(() => new Result()),
            () => Assert.Throws<InvalidOperationException>(() => new Result<int>())
        );
    }

    [Fact]
    public void GenericResult_ImplicitFromError_HasErrorsAndNoValue() {
        // act
        Result<int> sut = Error.Missing("gone");

        // assert
        Assert.Multiple(
            () => Assert.False(sut.Success),
            () => Assert.Throws<InvalidOperationException>(() => sut.Value)
        );
    }

    [Fact]
    public void GenericResult_ImplicitFromValue_IsSuccessful() {
        // act
        Result<int> sut = 42;

        // assert
        Assert.Multiple(
            () => Assert.True(sut.Success),
            () => Assert.Equal(42, sut.Value),
            () => Assert.Throws<InvalidOperationException>(() => sut.Errors)
        );
    }

    [Fact]
    public void GenericResult_Match_ChoosesBranch() {
        // arrange
        Result<int> success = 5;
        Result<int> failure = new[] { Error.Failure("x") };
        var seen = 0;

        // act
        success.Match(value => seen = value, _ => seen = -1);
        var text = failure.Match(value => "v", errors => $"e{errors.Length}");

        // assert
        Assert.Multiple(
            () => Assert.Equal(5, seen),
            () => Assert.Equal("e1", text),
            () => Assert.Equal(6, success.Match(v => v + 1, _ => 0))
        );
    }

    [Fact]
    public void GenericResult_Match_Action_InvokesFailureBranch() {
        // arrange
        Result<int> failure = Error.Failure("x");
        var count = 0;

        // act
        failure.Match(_ => { }, errors => count = errors.Length);

        // assert
        Assert.Equal(1, count);
    }
}
