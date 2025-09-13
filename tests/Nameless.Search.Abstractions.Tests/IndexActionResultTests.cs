using Nameless.Testing.Tools.Data;

namespace Nameless.Search;

public class IndexActionResultTests {
    [Fact]
    public void WhenSuccess_ThenProvideTotalDocumentsAffected() {
        // arrange
        const int TotalDocumentsAffected = 100;

        // act
        var sut = IndexActionResult.Success(TotalDocumentsAffected);

        // assert
        Assert.Equal(TotalDocumentsAffected, sut.TotalDocumentsAffected);
    }

    [Fact]
    public void WhenSuccess_WhenTotalDocumentsAffectedIsNegative_ThenThrowsException() {
        // arrange
        const int TotalDocumentsAffected = -1;

        // act
        var exception = Record.Exception(() => IndexActionResult.Success(TotalDocumentsAffected));

        // assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    [Fact]
    public void WhenFailure_ThenProvidesErrorMessage() {
        // arrange
        const string ErrorMessage = "Error Message";

        // act
        var sut = IndexActionResult.Failure(ErrorMessage);

        // assert
        Assert.Equal(ErrorMessage, sut.Error);
    }

    [Theory]
    [ClassData(typeof(StringNullEmptyWhiteSpaceExceptionInlineData))]
    public void WhenFailure_WhenErrorNotProvided_ThenThrowsException(string errorMessage, Type exceptionType) {
        // arrange

        // act
        var exception = Record.Exception(() => IndexActionResult.Failure(errorMessage));

        // assert
        Assert.IsType(exceptionType, exception);
    }
}