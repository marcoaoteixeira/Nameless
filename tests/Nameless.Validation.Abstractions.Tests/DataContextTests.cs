using System.Collections;
using Nameless.Testing.Tools.Data;

namespace Nameless.Validation;

public class DataContextTests {
    [Fact]
    public void WhenInitializing_WhenUsingCollectionInitializer_ThenRetrieveAddedDataContext() {
        // arrange
        const string Key = "Key";
        object value = 123;

        // act
        DataContext sut = [new KeyValuePair<string, object>(Key, value)];

        // assert
        Assert.Contains(sut, item => item.Key == Key && item.Value == value);
    }

    [Theory]
    [ClassData(typeof(StringNullEmptyWhiteSpaceExceptionInlineData))]
    public void WhenInitializingUsingCollectionInitializer_WhenKeyIsNullEmptyOrWhitespace_ThenThrowsException(
        string key, Type exceptionType) {
        // arrange

        // act
        var actual = Record.Exception(() => {
            DataContext _ = [new KeyValuePair<string, object>(key, value: 123)];
        });

        // assert
        Assert.IsType(exceptionType, actual);
    }

    [Theory]
    [ClassData(typeof(StringNullEmptyWhiteSpaceExceptionInlineData))]
    public void WhenAddingValue_WhenKeyIsNullEmptyOrWhitespace_ThenThrowsException(string key, Type exceptionType) {
        // arrange

        // act
        var actual = Record.Exception(() => new DataContext().Add(new KeyValuePair<string, object>(key, value: 123)));

        // assert
        Assert.IsType(exceptionType, actual);
    }

    [Fact]
    public void WhenGettingValueThroughIndexer_WhenKeyExists_ThenReturnsValue() {
        // arrange
        const string Key = nameof(Key);
        const int Value = 123;
        var sut = new DataContext { [Key] = Value };

        // act
        var actual = sut[Key];

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGettingValueThroughIndexer_WhenKeyDoNotExists_ThenReturnsNull() {
        // arrange
        const string UnknownKey = nameof(UnknownKey);
        const string Key = nameof(Key);
        const int Value = 123;
        var sut = new DataContext { [Key] = Value };

        // act
        var actual = sut[UnknownKey];

        // assert
        Assert.Null(actual);
    }

    [Fact]
    public void WhenEnumerating_WhenThereAreItems_ThenCanMoveNext() {
        // arrange
        var sut = new DataContext { [key: "X"] = 1, [key: "Y"] = 2, [key: "Z"] = 3 };

        // act
        using var actual = sut.GetEnumerator();

        // assert
        Assert.True(actual.MoveNext());
    }

    [Fact]
    public void WhenEnumeratingWithNonGenericEnumerator_WhenThereAreItems_ThenCanMoveNext() {
        // arrange
        var sut = new DataContext { [key: "X"] = 1, [key: "Y"] = 2, [key: "Z"] = 3 };

        // act
        var actual = ((IEnumerable)sut).GetEnumerator();

        // assert
        Assert.True(actual.MoveNext());

        if (actual is IDisposable disposable) {
            disposable.Dispose();
        }
    }
}