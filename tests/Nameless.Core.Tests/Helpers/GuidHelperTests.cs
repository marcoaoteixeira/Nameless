namespace Nameless.Helpers;

public class GuidHelperTests {
    private const string STRING_VALUE = "ykBtEknUGE_Pw0Pshkajag";
    private readonly Guid _guidValue = Guid.Parse("126d40ca-d449-4f18-8fc3-43ec8646a36a");

    [Fact]
    public void Encode_WhenPassGuidValue_ThenReturnStringEncoded() {
        // arrange

        // act
        var actual = GuidHelper.Encode(_guidValue);

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);
            Assert.Equal(STRING_VALUE, actual);
        });
    }

    [Fact]
    public void Decode_WhenPassStringValue_ThenReturnGuidDecoded() {
        // arrange

        // act
        var actual = GuidHelper.Decode(STRING_VALUE);

        // assert
        Assert.Equal(_guidValue, actual);
    }
}