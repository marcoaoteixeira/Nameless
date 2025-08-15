namespace Nameless.Search;
public class PreventExtensionsTests {
    [Theory]
    [ClassData(typeof(IndexableTypeInlineData))]
    public void NullOrNonMatchingType_ShouldThrowException_WhenValueTypeDoNotMatchIndexableType(IndexableType indexableType) {
        // arrange

        // we do not have a type for enums.
        // this should throw ex.
        object value = DayOfWeek.Monday;

        // act
        var exception = Record.Exception(() => Guard.Against.NullOrNonMatchingType(value, indexableType, nameof(value)));

        // assert
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Theory]
    [ClassData(typeof(SampleValueIndexableTypeInlineData))]
    public void NullOrNonMatchingType_ShouldNotThrowException_WhenValueTypeIsIndexableType(object value, IndexableType indexableType) {
        // arrange

        // act
        var actual = Guard.Against.NullOrNonMatchingType(value, indexableType, nameof(value));

        // assert
        Assert.NotNull(actual);
    }

    [Fact]
    public void NullOrNonMatchingType_ShouldThrowException_WhenValueIsNull() {
        // arrange
        object value = null;

        // act
        var exception = Record.Exception(() => Guard.Against.NullOrNonMatchingType(value, IndexableType.Boolean, nameof(value)));

        // assert
        Assert.IsType<ArgumentNullException>(exception);
    }
}
