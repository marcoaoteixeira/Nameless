namespace Nameless.Validation;

public class ValidationErrorTests {
    [Fact]
    public void WhenInitialize_WithValidArguments_ThenRetrieveCorrectValuesFromProperties() {
        // arrange
        const string Message = "Error Message";
        const string ErrorCode = "Error Code";
        const string MemberName = "Member Name";

        // act
        var sut = new ValidationError(Message, ErrorCode, MemberName);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(Message, sut.Message);
            Assert.Equal(ErrorCode, sut.ErrorCode);
            Assert.Equal(MemberName, sut.MemberName);
        });
    }
}
