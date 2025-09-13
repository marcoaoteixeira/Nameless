namespace Nameless.Search;

public class EmptySearchHitTests {
    [Fact]
    public void MakeSureEmptySearchHit_IsReallyEmpty() {
        // arrange

        // act
        var sut = new EmptySearchHit();

        // assert
        Assert.Multiple(() => {
            Assert.Empty(sut.DocumentID);
            Assert.Equal(expected: 0F, sut.Score);
            Assert.Null(sut.GetBoolean(fieldName: "boolean_field"));
            Assert.Null(sut.GetString(fieldName: "string_field"));
            Assert.Null(sut.GetByte(fieldName: "byte_field"));
            Assert.Null(sut.GetShort(fieldName: "short_field"));
            Assert.Null(sut.GetInteger(fieldName: "integer_field"));
            Assert.Null(sut.GetLong(fieldName: "long_field"));
            Assert.Null(sut.GetFloat(fieldName: "float_field"));
            Assert.Null(sut.GetDouble(fieldName: "double_field"));
            Assert.Null(sut.GetDateTimeOffset(fieldName: "datetimeoffset_field"));
            Assert.Null(sut.GetDateTime(fieldName: "datetime_field"));
        });
    }
}