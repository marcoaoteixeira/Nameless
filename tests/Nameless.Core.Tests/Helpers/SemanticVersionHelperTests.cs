using Nameless.Instrumentation;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Helpers;

[UnitTest]
public class SemanticVersionHelperTests {
    [Theory]
    [ClassData<SemanticVersionTheoryData>]
    public void WhenParse_ThenRetrieveCorrectVersion(string version, Version expected) {
        // act
        var actual = SemanticVersionHelper.TryParse(version, out var output);

        // assert
        Assert.Multiple(() => {
            Assert.True(actual);
            Assert.Equal(expected, output);
        });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid")]
    [InlineData("v1.2.3.4")]
    public void WhenParse_WhenValueIsInvalid_ThenReturnsFalse(string version) {
        // act
        var actual = SemanticVersionHelper.TryParse(version, out var output);

        // assert
        Assert.Multiple(() => {
            Assert.False(actual);
            Assert.Null(output);
        });
    }
}