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
            Assert.Equal(0F, sut.Score);
            Assert.Null(sut.GetBoolean("boolean_field"));
            Assert.Null(sut.GetString("string_field"));
            Assert.Null(sut.GetByte("byte_field"));
            Assert.Null(sut.GetShort("short_field"));
            Assert.Null(sut.GetInteger("integer_field"));
            Assert.Null(sut.GetLong("long_field"));
            Assert.Null(sut.GetFloat("float_field"));
            Assert.Null(sut.GetDouble("double_field"));
            Assert.Null(sut.GetDateTimeOffset("datetimeoffset_field"));
            Assert.Null(sut.GetDateTime("datetime_field"));
        });
    }
}
