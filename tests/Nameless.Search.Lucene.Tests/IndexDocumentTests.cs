using Nameless.Search.Lucene.Fixtures;

namespace Nameless.Search.Lucene;

public class DocumentTests {
    [Fact]
    public void WhenCreating_ThenThereShouldBeAFieldCalledID() {
        // arrange
        const string ID = "123";
        var sut = new Document(ID);

        // act
        var field = sut.FirstOrDefault();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(field);
            Assert.Equal(nameof(ISearchHit.DocumentID), field.Name);
            Assert.Equal(ID, field.Value);
            Assert.Equal(IndexableType.String, field.Type);
            Assert.Equal(FieldOptions.Store, field.Options);
        });
    }

    [Theory]
    [ClassData(typeof(FieldOptionsInlineData))]
    public void WhenSetDateTimeOffsetField_ThenStoreTheCorrectFieldAndMetadata(FieldOptions option) {
        // arrange
        const string ID = "123";
        const string FieldName = "Field";
        var value = new DateTimeOffset(year: 2000, month: 1, day: 1, hour: 12, minute: 30, second: 0, TimeSpan.Zero);

        var sut = new Document(ID);

        // act
        var field = sut.Set(FieldName, value, option).LastOrDefault();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(field);
            Assert.Equal(FieldName, field.Name);
            Assert.Equal(value, field.Value);
            Assert.Equal(IndexableType.DateTimeOffset, field.Type);
            Assert.Equal(option, field.Options);
        });
    }

    [Theory]
    [ClassData(typeof(FieldOptionsInlineData))]
    public void WhenSetStringField_ThenStoreTheCorrectFieldAndMetadata(FieldOptions option) {
        // arrange
        const string ID = "123";
        const string FieldName = "Field";
        const string Value = "this is a test";

        var sut = new Document(ID);

        // act
        var field = sut.Set(FieldName, Value, option).LastOrDefault();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(field);
            Assert.Equal(FieldName, field.Name);
            Assert.Equal(Value, field.Value);
            Assert.Equal(IndexableType.String, field.Type);
            Assert.Equal(option, field.Options);
        });
    }

    [Theory]
    [ClassData(typeof(FieldOptionsInlineData))]
    public void WhenSetBooleanField_ThenStoreTheCorrectFieldAndMetadata(FieldOptions option) {
        // arrange
        const string ID = "123";
        const string FieldName = "Field";
        const bool Value = true;

        var sut = new Document(ID);

        // act
        var field = sut.Set(FieldName, Value, option).LastOrDefault();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(field);
            Assert.Equal(FieldName, field.Name);
            Assert.Equal(Value, field.Value);
            Assert.Equal(IndexableType.Boolean, field.Type);
            Assert.Equal(option, field.Options);
        });
    }

    [Theory]
    [ClassData(typeof(FieldOptionsInlineData))]
    public void WhenSetDoubleField_ThenStoreTheCorrectFieldAndMetadata(FieldOptions option) {
        // arrange
        const string ID = "123";
        const string FieldName = "Field";
        const double Value = 987.654D;

        var sut = new Document(ID);

        // act
        var field = sut.Set(FieldName, Value, option).LastOrDefault();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(field);
            Assert.Equal(FieldName, field.Name);
            Assert.Equal(Value, field.Value);
            Assert.Equal(IndexableType.Double, field.Type);
            Assert.Equal(option, field.Options);
        });
    }

    [Theory]
    [ClassData(typeof(FieldOptionsInlineData))]
    public void WhenSetIntegerField_ThenStoreTheCorrectFieldAndMetadata(FieldOptions option) {
        // arrange
        const string ID = "123";
        const string FieldName = "Field";
        const int Value = 10001;

        var sut = new Document(ID);

        // act
        var field = sut.Set(FieldName, Value, option).LastOrDefault();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(field);
            Assert.Equal(FieldName, field.Name);
            Assert.Equal(Value, field.Value);
            Assert.Equal(IndexableType.Integer, field.Type);
            Assert.Equal(option, field.Options);
        });
    }

    [Fact]
    public void WhenSetField_ShouldSetOptionsAsFlag() {
        // arrange
        const string ID = "123";
        const string FieldName = "Field";
        const int Value = 5000;
        const FieldOptions Options = FieldOptions.Store | FieldOptions.Analyze;

        var sut = new Document(ID);

        // act
        var field = sut.Set(FieldName, Value, Options).LastOrDefault();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(field);
            Assert.Equal(FieldName, field.Name);
            Assert.Equal(Value, field.Value);
            Assert.Equal(IndexableType.Integer, field.Type);
            Assert.True(field.Options.HasFlag(FieldOptions.Store));
            Assert.True(field.Options.HasFlag(FieldOptions.Analyze));
        });
    }
}