using System.Text;

namespace Nameless.Extensions;

public class StringExtensionsTests {
    // ─── RemoveDiacritics ────────────────────────────────────────────────────

    [Fact]
    public void RemoveDiacritics_Cafe_ReturnsCafe() {
        // act
        var result = "café".RemoveDiacritics();

        // assert
        Assert.Equal("cafe", result);
    }

    [Fact]
    public void RemoveDiacritics_Naive_ReturnsNaive() {
        // act
        var result = "naïve".RemoveDiacritics();

        // assert
        Assert.Equal("naive", result);
    }

    // ─── Repeat ──────────────────────────────────────────────────────────────

    [Fact]
    public void Repeat_WithZeroTimes_ReturnsOriginal() {
        // act
        var result = "ab".Repeat(0);

        // assert
        Assert.Equal("ab", result);
    }

    [Fact]
    public void Repeat_WithNegativeTimes_ReturnsOriginal() {
        // act
        var result = "ab".Repeat(-1);

        // assert
        Assert.Equal("ab", result);
    }

    [Fact]
    public void Repeat_WithThreeTimes_RepeatsThreeTimes() {
        // act
        var result = "ab".Repeat(3);

        // assert
        Assert.Equal("ababab", result);
    }

    // ─── ToMemoryStream ──────────────────────────────────────────────────────

    [Fact]
    public void ToMemoryStream_ReturnsMemoryStream() {
        // act
        using var stream = "hello".ToMemoryStream();

        // assert
        Assert.IsType<MemoryStream>(stream);
    }

    [Fact]
    public void ToMemoryStream_ContentMatchesUtf8Bytes() {
        // arrange
        const string value = "hello";

        // act
        using var stream = value.ToMemoryStream();
        var bytes = stream.ToArray();

        // assert
        Assert.Equal(Encoding.UTF8.GetBytes(value), bytes);
    }

    // ─── CamelFriendly ───────────────────────────────────────────────────────

    [Fact]
    public void CamelFriendly_PascalCaseString_InsertsSpaces() {
        // act
        var result = "MyClassName".CamelFriendly();

        // assert
        Assert.Equal("My Class Name", result);
    }

    // ─── Ellipsize ───────────────────────────────────────────────────────────

    [Fact]
    public void Ellipsize_ShortString_ReturnsOriginal() {
        // act
        var result = "hi".Ellipsize(5);

        // assert
        Assert.Equal("hi", result);
    }

    [Fact]
    public void Ellipsize_LongString_TruncatesAndAddsEllipsis() {
        // act
        var result = "hello world".Ellipsize(5);

        // assert
        Assert.Contains("&#8230;", result);
        Assert.True(result.Length < "hello world".Length + 20);
    }

    [Fact]
    public void Ellipsize_AtWordBoundaryWithTrailingSpace_BacktracksOverWhitespace() {
        // characterCount=6 lands on ' ' after "hello", triggering the whitespace backtracking loop
        var result = "hello world".Ellipsize(6);

        Assert.StartsWith("hello", result);
    }

    // ─── ToHexByteArray ──────────────────────────────────────────────────────

    [Fact]
    public void ToHexByteArray_WithValidHexString_ReturnsByteArray() {
        // act
        var result = "FF00AB".ToHexByteArray();

        // assert
        Assert.Multiple(() => {
            Assert.Equal(3, result.Length);
            Assert.Equal(0xFF, result[0]);
            Assert.Equal(0x00, result[1]);
            Assert.Equal(0xAB, result[2]);
        });
    }

    // ─── ToTechnicalName ─────────────────────────────────────────────────────

    [Fact]
    public void ToTechnicalName_WithWhitespaceOnly_ReturnsEmpty() {
        var result = "   ".ToTechnicalName();
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ToTechnicalName_WithSymbolsOnly_ReturnsEmpty() {
        var result = "@#$%".ToTechnicalName();
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ToTechnicalName_StartingWithDigits_StripsLeadingDigits() {
        var result = "123abc".ToTechnicalName();
        Assert.Equal("abc", result);
    }

    [Fact]
    public void ToTechnicalName_ExceedingMaxSize_Truncates() {
        var result = "abcdef".ToTechnicalName(maxSize: 3);
        Assert.Equal("abc", result);
    }

    [Fact]
    public void ToTechnicalName_WithNormalName_ReturnsName() {
        var result = "HelloWorld".ToTechnicalName();
        Assert.Equal("HelloWorld", result);
    }

    // ─── Split(regex) ────────────────────────────────────────────────────────

    [Fact]
    public void Split_WithRegexPattern_SplitsOnMatches() {
        // Split(string, RegexOptions) — no built-in string.Split takes RegexOptions,
        // so this resolves to the extension method.
        var result = "a1b2c".Split(@"\d", System.Text.RegularExpressions.RegexOptions.None);

        Assert.Equal(3, result.Length);
    }

    // ─── RemoveHead with whitespace prefix values ─────────────────────────────

    [Fact]
    public void RemoveHead_WithWhitespaceEntryInValues_SkipsAndContinues() {
        // the empty string in the array hits the IsNullOrWhiteSpace(value) continue branch
        var result = "IMyInterface".RemoveHead(["", "I"]);
        Assert.Equal("MyInterface", result);
    }

    // ─── RemoveTail with whitespace suffix values ─────────────────────────────

    [Fact]
    public void RemoveTail_WithWhitespaceEntryInValues_SkipsAndContinues() {
        var result = "MyService".RemoveTail(["", "Service"]);
        Assert.Equal("My", result);
    }

    // ─── Strip(char[]) ───────────────────────────────────────────────────────

    [Fact]
    public void Strip_Chars_RemovesSpecifiedChars() {
        // act
        var result = "hello world".Strip('l', 'o');

        // assert
        Assert.Equal("he wrd", result);
    }

    // ─── Strip(Func<char, bool>) ─────────────────────────────────────────────

    [Fact]
    public void Strip_Predicate_RemovesCharsMatchingPredicate() {
        // act
        var result = "hello123".Strip(char.IsDigit);

        // assert
        Assert.Equal("hello", result);
    }

    // ─── Any ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Any_WhenCharPresent_ReturnsTrue() {
        // act
        var result = "hello".Any('e');

        // assert
        Assert.True(result);
    }

    [Fact]
    public void Any_WhenCharNotPresent_ReturnsFalse() {
        // act
        var result = "hello".Any('z');

        // assert
        Assert.False(result);
    }

    // ─── All ─────────────────────────────────────────────────────────────────

    [Fact]
    public void All_WhenAllCharsPresent_ReturnsTrue() {
        // act
        var result = "hello".All('h', 'e', 'l', 'o');

        // assert
        Assert.True(result);
    }

    [Fact]
    public void All_WhenSomeCharsNotPresent_ReturnsFalse() {
        // act
        var result = "hello".All('h', 'z');

        // assert
        Assert.False(result);
    }

    // ─── ToBase64 / FromBase64 ───────────────────────────────────────────────

    [Fact]
    public void ToBase64_ThenFromBase64_RoundTrips() {
        // arrange
        const string original = "hello world";

        // act
        var encoded = original.ToBase64();
        var decoded = encoded.FromBase64();

        // assert
        Assert.Equal(original, decoded);
    }

    // ─── GetBytes ────────────────────────────────────────────────────────────

    [Fact]
    public void GetBytes_ReturnsUtf8Bytes() {
        // arrange
        const string value = "hello";

        // act
        var bytes = value.GetBytes();

        // assert
        Assert.Equal(Encoding.UTF8.GetBytes(value), bytes);
    }

    // ─── RemoveHtmlTags ──────────────────────────────────────────────────────

    [Fact]
    public void RemoveHtmlTags_StripsTags() {
        // act
        var result = "<p>Hello <b>World</b></p>".RemoveHtmlTags();

        // assert
        Assert.Equal("Hello World", result);
    }

    // ─── Contains(string, StringComparison) ─────────────────────────────────

    [Fact]
    public void Contains_CaseInsensitive_ReturnsTrue() {
        // act
        var result = "Hello World".Contains("world", StringComparison.OrdinalIgnoreCase);

        // assert
        Assert.True(result);
    }

    // ─── IsMatch ─────────────────────────────────────────────────────────────

    [Fact]
    public void IsMatch_WhenPatternMatches_ReturnsTrue() {
        // act
        var result = "hello123".IsMatch(@"\d+");

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsMatch_WhenPatternDoesNotMatch_ReturnsFalse() {
        // act
        var result = "hello".IsMatch(@"^\d+$");

        // assert
        Assert.False(result);
    }

    // ─── SafeSubstring ───────────────────────────────────────────────────────

    [Fact]
    public void SafeSubstring_WithNull_ReturnsEmpty() {
        // arrange
        string? value = null;

        // act
        var result = value.SafeSubstring(0, 5);

        // assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void SafeSubstring_WithValidRange_ReturnsSubstring() {
        // act
        var result = "hello world".SafeSubstring(6, 5);

        // assert
        Assert.Equal("world", result);
    }

    [Fact]
    public void SafeSubstring_WithOverflowLength_ReturnsTrimmedSubstring() {
        // act
        var result = "hello".SafeSubstring(3, 100);

        // assert
        Assert.Equal("lo", result);
    }

    // ─── ToBoolean ───────────────────────────────────────────────────────────

    [Theory]
    [InlineData("true", true)]
    [InlineData("True", true)]
    [InlineData("TRUE", true)]
    [InlineData("Yes", true)]
    [InlineData("yes", true)]
    [InlineData("Y", true)]
    [InlineData("1", true)]
    public void ToBoolean_TruthyValues_ReturnsTrue(string? input, bool expected) {
        // act
        var result = input.ToBoolean();

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("false", false)]
    [InlineData("False", false)]
    [InlineData("No", false)]
    [InlineData("0", false)]
    public void ToBoolean_FalsyValues_ReturnsFalse(string? input, bool expected) {
        // act
        var result = input.ToBoolean();

        // assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToBoolean_WithNull_ReturnsFalse() {
        // arrange
        string? value = null;

        // act
        var result = value.ToBoolean();

        // assert
        Assert.False(result);
    }

    // ─── WithFallback ────────────────────────────────────────────────────────

    [Fact]
    public void WithFallback_WhenNull_ReturnsFallback() {
        // arrange
        string? value = null;

        // act
        var result = value.WithFallback("default");

        // assert
        Assert.Equal("default", result);
    }

    [Fact]
    public void WithFallback_WhenEmpty_ReturnsFallback() {
        // act
        var result = string.Empty.WithFallback("default");

        // assert
        Assert.Equal("default", result);
    }

    [Fact]
    public void WithFallback_WhenWhitespace_ReturnsFallback() {
        // act
        var result = "   ".WithFallback("default");

        // assert
        Assert.Equal("default", result);
    }

    [Fact]
    public void WithFallback_WhenValid_ReturnsOriginal() {
        // act
        var result = "hello".WithFallback("default");

        // assert
        Assert.Equal("hello", result);
    }

    // ─── ToSnakeCase ─────────────────────────────────────────────────────────

    [Fact]
    public void ToSnakeCase_PascalCase_ReturnsSnakeCase() {
        // act
        var result = "MyProperty".ToSnakeCase();

        // assert
        Assert.Equal("my_property", result);
    }

    [Fact]
    public void ToSnakeCase_WithNull_ReturnsEmpty() {
        // arrange
        string? value = null;

        // act
        var result = value.ToSnakeCase();

        // assert
        Assert.Equal(string.Empty, result);
    }

    // ─── RemoveHead ──────────────────────────────────────────────────────────

    [Fact]
    public void RemoveHead_WhenPrefixMatches_RemovesPrefix() {
        // act
        var result = "IMyInterface".RemoveHead(["I"]);

        // assert
        Assert.Equal("MyInterface", result);
    }

    [Fact]
    public void RemoveHead_WhenNoPrefixMatches_ReturnsOriginal() {
        // act
        var result = "MyClass".RemoveHead(["Foo"]);

        // assert
        Assert.Equal("MyClass", result);
    }

    // ─── RemoveTail ──────────────────────────────────────────────────────────

    [Fact]
    public void RemoveTail_WhenSuffixMatches_RemovesSuffix() {
        // act
        var result = "MyService".RemoveTail(["Service"]);

        // assert
        Assert.Equal("My", result);
    }

    [Fact]
    public void RemoveTail_WhenNoSuffixMatches_ReturnsOriginal() {
        // act
        var result = "MyClass".RemoveTail(["Foo"]);

        // assert
        Assert.Equal("MyClass", result);
    }

    // ─── SplitUpperCase ──────────────────────────────────────────────────────

    [Fact]
    public void SplitUpperCase_PascalCaseString_SplitsOnUppercaseBoundaries() {
        // act
        var result = "MyClassName".SplitUpperCase();

        // assert
        Assert.Multiple(() => {
            Assert.Equal(3, result.Length);
            Assert.Equal("My", result[0]);
            Assert.Equal("Class", result[1]);
            Assert.Equal("Name", result[2]);
        });
    }

    // ─── GetMD5 ──────────────────────────────────────────────────────────────

    [Fact]
    public void GetMD5_ReturnsNonEmptyString() {
        // act
        var result = "hello".GetMD5();

        // assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetMD5_SameInputProducesSameResult() {
        // act
        var first = "hello".GetMD5();
        var second = "hello".GetMD5();

        // assert
        Assert.Equal(first, second);
    }

    // ─── ReplaceAll ──────────────────────────────────────────────────────────

    [Fact]
    public void ReplaceAll_ReplacesAllKeysWithValues() {
        // arrange
        var replacements = new Dictionary<string, string> {
            ["foo"] = "bar",
            ["baz"] = "qux"
        };

        // act
        var result = "foo and baz".ReplaceAll(replacements);

        // assert
        Assert.Equal("bar and qux", result);
    }
}
