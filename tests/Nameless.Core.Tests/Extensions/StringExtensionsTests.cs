using System.Text;

namespace Nameless;

public class StringExtensionsTests {
    // Diacritics, often loosely called 'accents', are the various little dots and
    // squiggles which, in many languages, are written above, below or on top of
    // certain letters of the alphabet to indicate something about their
    // pronunciation.
    [Fact]
    public void RemoveDiacritics_Should_Remove_Diacritics_From_String() {
        // arrange
        var value = "â-ê-î-ô-û-ŵ-ŷ-ä-ë-ï-ö-ü-ẅ-ÿ-á-é-í-ó-ú-ẃ-ý-à-è-ì-ò-ù-ẁ-ỳ-ã-ç-õ-ñ";
        var expected = "a-e-i-o-u-w-y-a-e-i-o-u-w-y-a-e-i-o-u-w-y-a-e-i-o-u-w-y-a-c-o-n";
        // act

        var actual = value.RemoveDiacritics();

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Repeat_Should_Repeat_String_X_Times() {
        // arrange
        var value = "Test";
        var times = 5;
        var expected = "TestTestTestTestTest";

        // act
        var actual = value.Repeat(times);

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Repeat_Should_Not_Repeat_If_Times_Negative() {
        // arrange
        var value = "Test";
        var times = -5;
        var expected = "Test";

        // act
        var actual = value.Repeat(times);

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToStream_Should_Create_A_MemoryStream_From_A_String() {
        // arrange
        const string Value = "This is a Test";

        // act
        using var actual = Value.ToMemoryStream();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);
            Assert.NotEmpty(actual.ToArray());
            Assert.IsType<MemoryStream>(actual);
        });
    }

    [Fact]
    public void CamelFriendly_Should_Create_A_Space_Separated_String_From_A_Camel_String_Value() {
        // arrange
        const string Value = "ThisIsATest";
        const string Expected = "This Is A Test";

        // act
        var actual = Value.CamelFriendly();

        // arrange
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void Ellipsize_Should_Return_A_Portion_Of_A_String_Ending_With_Ellipsis() {
        // arrange
        const string Value = "This Is A Test";
        const string Ellipsis = "...";
        const string Expected = $"This Is{Ellipsis}";
        const int Count = 8;

        // act
        var actual = Value.Ellipsize(Count, Ellipsis, false);

        // arrange
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void FromHexToByteArray_Should_Return_Array_Of_Bytes() {
        // arrange
        const string Expected = "Test Is A Test";
        var array = Expected.ToCharArray()
                            .Select(item => ((int)item).ToString("X2"));
        var value = string.Join(string.Empty, array);

        // act
        var actual = value.ToHexByteArray();

        // assert
        Assert.Equal(Expected, Encoding.UTF8.GetString(actual));
    }

    [Fact]
    public void ReplaceAll_Use_Dictionary_To_Replace_Values_Inside_Text() {
        // arrange
        var replacements = new Dictionary<string, string> { { "cat", "dog" }, { "meow", "bark" } };
        const string Phrase = "The cat goes meow";
        const string Expected = "The dog goes bark";

        // act
        var actual = Phrase.ReplaceAll(replacements);

        // assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void ToBase64_Is_Just_A_Syntax_Sugar_For_Convert_ToBase64String() {
        // arrange
        const string Expected = "VGhpcyBpcyBhIHRlc3Q="; // This is a test
        const string Phrase = "This is a test";

        // act
        var actual = Phrase.ToBase64();

        // assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void ToBase64_Using_Different_Encoding() {
        // arrange
        const string Expected = "VGhpcyBpcyBhIHRlc3Q="; // This is a test
        const string Phrase = "This is a test";

        // act
        var actual = Phrase.ToBase64(Encoding.ASCII);

        // assert
        Assert.Equal(Expected, actual);
    }
}