using Lucene.Net.Documents;
using Nameless.Lucene.InlineData;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene;

[UnitTest]
public class SearchHitTests {
    private const string DOCUMENT_ID = "123456";

    private static LuceneDocument CreateLuceneDocument() {
        var document = new LuceneDocument {
            Fields = {
                new StringField(Document.RESERVED_ID_NAME, DOCUMENT_ID, LuceneField.Store.YES),
            }
        };

        var inlineData = new FieldInlineData();
        foreach (var line in inlineData) {
            var data = line.Data;

            LuceneField field = data.Item3 switch {
                IndexableType.Boolean => new Int32Field(data.Item1, (bool)data.Item2 ? 1 : 0, LuceneField.Store.NO),
                IndexableType.String => new StringField(data.Item1, (string)data.Item2, LuceneField.Store.NO),
                IndexableType.Byte => new Int32Field(data.Item1, (byte)data.Item2, LuceneField.Store.NO),
                IndexableType.Short => new Int32Field(data.Item1, (short)data.Item2, LuceneField.Store.NO),
                IndexableType.Integer => new Int32Field(data.Item1, (int)data.Item2, LuceneField.Store.NO),
                IndexableType.Long => new Int64Field(data.Item1, (long)data.Item2, LuceneField.Store.NO),
                IndexableType.Float => new SingleField(data.Item1, (float)data.Item2, LuceneField.Store.NO),
                IndexableType.Double => new DoubleField(data.Item1, (double)data.Item2, LuceneField.Store.NO),
                IndexableType.DateTimeOffset => new Int64Field(data.Item1, ((DateTimeOffset)data.Item2).ToUniversalTime().Ticks, LuceneField.Store.NO),
                IndexableType.DateTime => new Int64Field(data.Item1, ((DateTime)data.Item2).ToUniversalTime().Ticks, LuceneField.Store.NO),
                IndexableType.DateOnly => new Int64Field(data.Item1, new DateTime((DateOnly)data.Item2, TimeOnly.MinValue).Ticks, LuceneField.Store.NO),
                IndexableType.TimeOnly => new Int64Field(data.Item1, ((TimeOnly)data.Item2).Ticks, LuceneField.Store.NO),
                IndexableType.TimeSpan => new Int64Field(data.Item1, ((TimeSpan)data.Item2).Ticks, LuceneField.Store.NO),
                IndexableType.Enum => new StringField(data.Item1, data.Item2.ToString(), LuceneField.Store.NO),
                _ => throw new ArgumentOutOfRangeException()
            };

            document.Fields.Add(field);
        }

        return document;
    }

    private static IEnumerable<TheoryDataRow<string, object, IndexableType, FieldOptions>> GetFieldInlineData(IndexableType type) {
        return new FieldInlineData().Where(item => item.Data.Item3 == type);
    }

    [Fact]
    public void WhenConstructing_WithValidParameters_ThenReturnsNewInstance() {
        // arrange
        const float Score = 0.15F;
        var document = CreateLuceneDocument();

        // act
        var actual = new SearchHit(document, Score);

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);
            Assert.Equal(DOCUMENT_ID, actual.DocumentID);
            Assert.Equal(Score, actual.Score);
        });
    }

    [Fact]
    public void WhenGettingDateTimeOffset_ThenReturnsCorrectValue() {
        var inlineData = GetFieldInlineData(IndexableType.DateTimeOffset).ToArray();

        // arrange
        var document = CreateLuceneDocument();
        var sut = new SearchHit(document);

        // act
        var min = sut.GetDateTimeOffset(inlineData[0].Data.Item1);
        var max = sut.GetDateTimeOffset(inlineData[1].Data.Item1);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(inlineData[0].Data.Item2, min);
            Assert.Equal(inlineData[1].Data.Item2, max);
        });
    }

    [Fact]
    public void WhenGettingDateTime_ThenReturnsCorrectValue() {
        var inlineData = GetFieldInlineData(IndexableType.DateTime).ToArray();

        // arrange
        var document = CreateLuceneDocument();
        var sut = new SearchHit(document);

        // act
        var min = sut.GetDateTime(inlineData[0].Data.Item1);
        var max = sut.GetDateTime(inlineData[1].Data.Item1);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(inlineData[0].Data.Item2, min);
            Assert.Equal(inlineData[1].Data.Item2, max);
        });
    }

    [Fact]
    public void WhenGettingDateOnly_ThenReturnsCorrectValue() {
        var inlineData = GetFieldInlineData(IndexableType.DateOnly).ToArray();

        // arrange
        var document = CreateLuceneDocument();
        var sut = new SearchHit(document);

        // act
        var min = sut.GetDateOnly(inlineData[0].Data.Item1);
        var max = sut.GetDateOnly(inlineData[1].Data.Item1);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(inlineData[0].Data.Item2, min);
            Assert.Equal(inlineData[1].Data.Item2, max);
        });
    }

    [Fact]
    public void WhenGettingTimeOnly_ThenReturnsCorrectValue() {
        var inlineData = GetFieldInlineData(IndexableType.TimeOnly).ToArray();

        // arrange
        var document = CreateLuceneDocument();
        var sut = new SearchHit(document);

        // act
        var min = sut.GetTimeOnly(inlineData[0].Data.Item1);
        var max = sut.GetTimeOnly(inlineData[1].Data.Item1);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(inlineData[0].Data.Item2, min);
            Assert.Equal(inlineData[1].Data.Item2, max);
        });
    }

    [Fact]
    public void WhenGettingTimeSpan_ThenReturnsCorrectValue() {
        var inlineData = GetFieldInlineData(IndexableType.TimeSpan).ToArray();

        // arrange
        var document = CreateLuceneDocument();
        var sut = new SearchHit(document);

        // act
        var min = sut.GetTimeSpan(inlineData[0].Data.Item1);
        var max = sut.GetTimeSpan(inlineData[1].Data.Item1);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(inlineData[0].Data.Item2, min);
            Assert.Equal(inlineData[1].Data.Item2, max);
        });
    }

    [Fact]
    public void WhenGettingEnum_ThenReturnsCorrectValue() {
        var inlineData = GetFieldInlineData(IndexableType.Enum).ToArray();

        // arrange
        var document = CreateLuceneDocument();
        var sut = new SearchHit(document);

        // act
        var min = sut.GetEnum<DayOfWeek>(inlineData[0].Data.Item1);
        var max = sut.GetEnum<DayOfWeek>(inlineData[1].Data.Item1);
        var flag = sut.GetEnum<FieldOptions>(inlineData[2].Data.Item1);


        // assert
        Assert.Multiple(() => {
            Assert.Equal(inlineData[0].Data.Item2, min);
            Assert.Equal(inlineData[1].Data.Item2, max);

            Assert.NotNull(flag);
            Assert.True(flag.Value.HasFlag(FieldOptions.Store));
            Assert.True(flag.Value.HasFlag(FieldOptions.Sanitize));
            Assert.False(flag.Value.HasFlag(FieldOptions.Analyze));
            Assert.False(flag.Value.HasFlag(FieldOptions.None));
        });
    }
}
