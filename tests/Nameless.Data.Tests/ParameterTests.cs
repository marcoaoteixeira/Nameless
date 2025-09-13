using Nameless.Testing.Tools.Data;

namespace Nameless.Data;

public class ParameterTests {
    [Theory]
    [ClassData(typeof(StringNullEmptyWhiteSpaceExceptionInlineData))]
    public void WhenConstructing_WhenNameIsNullEmptyOrWhitespace_ThenThrowsException(string value, Type exceptionType) {
        Assert.Throws(exceptionType, () => new Parameter(value));
    }
}