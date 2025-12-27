using Nameless.Lucene.InlineData;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene;

[UnitTest]
public class FieldTests {
    [Theory]
    [ClassData<FieldInlineData>]
    public void WhenConstructing_WithValidParameters_ThenReturnsNewInstance(string name, object value, IndexableType type, FieldOptions options) {
        // arrange && act
        var actual = new Field(name, value, type, options);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(name, actual.Name);
            Assert.Equal(value, actual.Value);
            Assert.Equal(type, actual.Type);
            Assert.Equal(options, actual.Options);
        });
    }

    [Fact]
    public void WhenConstructing_WithNullName_ThenThrowsArgumentNullException() {
        // arrange && act
        var actual = Record.Exception(() => new Field(null!, "value", IndexableType.Boolean, FieldOptions.None));

        // assert
        Assert.IsType<ArgumentNullException>(actual);
    }

    [Fact]
    public void WhenConstructing_WithNullValue_ThenThrowsArgumentNullException() {
        // arrange && act
        var actual = Record.Exception(() => new Field("field", null!, IndexableType.Boolean, FieldOptions.None));

        // assert
        Assert.IsType<ArgumentNullException>(actual);
    }

    [Fact]
    public void WhenConstructing_WithValueThatDoNotMatchIndexableType_ThenThrowsInvalidOperationException() {
        // arrange && act
        var actual = Record.Exception(() => new Field("field", "value", IndexableType.Boolean, FieldOptions.None));

        // assert
        Assert.IsType<InvalidOperationException>(actual);
    }
}
