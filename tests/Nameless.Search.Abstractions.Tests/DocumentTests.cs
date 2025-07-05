using System.Collections;
using Nameless.Testing.Tools.Data;

namespace Nameless.Search;

public class DocumentTests {
    [Fact]
    public void WhenInitialize_WithDocumentID_ThenRetrieveCorrectPropertyValue() {
        // arrange
        const string DocumentID = "80dd1a57-459c-4cd6-b9db-8660223155b2";

        // act
        var sut = new Document(DocumentID);

        // assert
        Assert.Equal(DocumentID, sut.ID);
    }

    [Theory]
    [ClassData(typeof(StringNullEmptyWhiteSpaceExceptionInlineData))]
    public void WhenInitialize_WithInvalidDocumentID_ThenThrowsException(string documentID, Type exceptionType) {
        // arrange

        // act
        var exception = Record.Exception(() => new Document(documentID));

        // assert
        Assert.IsType(exceptionType, exception);
    }

    [Fact]
    public void WhenSettingField_WithValidInformation_ThenFieldMustBePresent() {
        // arrange
        var sut = new Document("e4bc7536-339f-4632-96d8-87c98325bcd9");

        // Total of 10 field types, see IndexableType.
        var fields = new Tuple<string, object, IndexableType>[] {
            new ("BoolValue", true, IndexableType.Boolean),
            new ("StringValue", "StringValue", IndexableType.String),
            new ("ByteValue", (byte)123, IndexableType.Byte),
            new ("ShortValue", (short)456, IndexableType.Short),
            new ("IntValue", 789, IndexableType.Integer),
            new ("LongValue", 987L, IndexableType.Long),
            new ("FloatValue", 654F, IndexableType.Float),
            new ("DoubleValue", 321D, IndexableType.Double),
            new ("DateTimeOffset", DateTimeOffset.Now, IndexableType.DateTimeOffset),
            new ("DateTime", DateTime.Now, IndexableType.DateTime),
        };

        // act
        sut.Set(fields[0].Item1, (bool)fields[0].Item2);
        sut.Set(fields[1].Item1, (string)fields[1].Item2);
        sut.Set(fields[2].Item1, (byte)fields[2].Item2);
        sut.Set(fields[3].Item1, (short)fields[3].Item2);
        sut.Set(fields[4].Item1, (int)fields[4].Item2);
        sut.Set(fields[5].Item1, (long)fields[5].Item2);
        sut.Set(fields[6].Item1, (float)fields[6].Item2);
        sut.Set(fields[7].Item1, (double)fields[7].Item2);
        sut.Set(fields[8].Item1, (DateTimeOffset)fields[8].Item2);
        sut.Set(fields[9].Item1, (DateTime)fields[9].Item2);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(fields.Length, sut.Count() - 1); // skip DocumentID field.
            foreach (var field in fields) {
                var documentField = sut.SingleOrDefault(item => item.Name == field.Item1);

                Assert.NotNull(documentField);
                Assert.Equal(documentField.Name, field.Item1);
                Assert.Equal(documentField.Value, field.Item2);
                Assert.Equal(documentField.Type, field.Item3);
            }
        });
    }

    [Fact]
    public void WhenGetEnumeratorNonGeneric_ThenReturnsEnumerator() {
        // arrange
        const string DocumentID = "80dd1a57-459c-4cd6-b9db-8660223155b2";

        // act
        var sut = new Document(DocumentID);
        var enumerator = ((IEnumerable)sut).GetEnumerator();

        // assert
        Assert.True(enumerator.MoveNext()); // has field document ID

        if (enumerator is IDisposable disposable) {
            disposable.Dispose();
        }
    }
}
