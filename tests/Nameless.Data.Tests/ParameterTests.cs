using Nameless.Testing.Tools.Data;

namespace Nameless.Data;

public class ParameterTests {
    [Theory]
    [ClassData(typeof(PreventStringNullOrWhiteSpaceInlineData))]
    public void WhenConstructing_WhenNameIsNullEmptyOrWhitespace_ThenThrowsException(string value, Type exceptionType) {
        Assert.Throws(exceptionType, () => new Parameter(name: value));
    }
}
