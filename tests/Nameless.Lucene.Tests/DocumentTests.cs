using Nameless.Lucene.InlineData;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene;

[UnitTest]
public class DocumentTests {
    [Fact]
    public void WhenConstructing_WhenProvidingIdentifier_ThenDocumentHasIdentifierField() {
        // arrange
        var expected = Guid.CreateVersion7().ToString("N");
        var document = new Document(expected);

        // act
        var actual = document.SingleOrDefault(field => field.Name.Equals(Document.RESERVED_ID_NAME));

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);
            Assert.Equal(expected, actual.Value);
        });
    }

    [Theory]
    [ClassData<FieldInlineData>]
    public void WhenSettingFieldValue_ThenDocumentMustContainNewField(string name, object value, IndexableType type, FieldOptions opts) {
        // arrange
        var sut = new Document("ID");

        // act
        switch (type) {
            case IndexableType.Boolean:
                sut.Set(name, (bool)value, opts);
                break;
            case IndexableType.String:
                sut.Set(name, (string)value, opts);
                break;
            case IndexableType.Byte:
                sut.Set(name, (byte)value, opts);
                break;
            case IndexableType.Short:
                sut.Set(name, (short)value, opts);
                break;
            case IndexableType.Integer:
                sut.Set(name, (int)value, opts);
                break;
            case IndexableType.Long:
                sut.Set(name, (long)value, opts);
                break;
            case IndexableType.Float:
                sut.Set(name, (float)value, opts);
                break;
            case IndexableType.Double:
                sut.Set(name, (double)value, opts);
                break;
            case IndexableType.DateTimeOffset:
                sut.Set(name, (DateTimeOffset)value, opts);
                break;
            case IndexableType.DateTime:
                sut.Set(name, (DateTime)value, opts);
                break;
            case IndexableType.DateOnly:
                sut.Set(name, (DateOnly)value, opts);
                break;
            case IndexableType.TimeOnly:
                sut.Set(name, (TimeOnly)value, opts);
                break;
            case IndexableType.TimeSpan:
                sut.Set(name, (TimeSpan)value, opts);
                break;
            case IndexableType.Enum:
                sut.Set(name, (Enum)value, opts);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        // assert
        var actual = sut.SingleOrDefault(field => field.Name == name);

        Assert.Multiple(() => {
            Assert.NotNull(actual);
            Assert.Equal(name, actual.Name);
            Assert.Equal(value, actual.Value);
            Assert.Equal(type, actual.Type);
            Assert.Equal(opts, actual.Options);
        });
    }

    [Fact]
    public void WhenSettingField_WhenNameIsReservedDocumentIDName_ThenThrowsInvalidOperationException() {
        // arrange
        var sut = new Document("ID");

        // act
        var actual = Record.Exception(() => sut.Set(Document.RESERVED_ID_NAME, "ID", FieldOptions.None));

        // assert
        Assert.IsType<InvalidOperationException>(actual);
    }
}
