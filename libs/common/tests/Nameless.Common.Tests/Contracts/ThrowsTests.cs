using System.Text.RegularExpressions;

namespace Nameless.Contracts;

[UnitTest]
public class ThrowsTests {
    // ─── Singleton ─────────────────────────────────────────────────────────

    [Fact]
    public void When_IsSingleton_ReturnsSameInstance() {
        // act & assert
        Assert.Same(Throws.When, Throws.When);
    }

    // ─── Custom message parameter ────────────────────────────────────────────

    [Fact]
    public void Null_WithCustomMessage_UsesProvidedMessage() {
        // act
        var ex = Assert.Throws<ArgumentNullException>(
            () => Throws.When.Null<string>(null, message: "my error")
        );

        // assert
        Assert.Contains("my error", ex.Message);
    }

    // ─── Null<T> ────────────────────────────────────────────────────────────

    [Fact]
    public void Null_WithNonNullValue_ReturnsValue() {
        // arrange
        var value = "hello";

        // act
        var result = Throws.When.Null(value);

        // assert
        Assert.Equal("hello", result);
    }

    [Fact]
    public void Null_WithNullValue_ThrowsArgumentNullException() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => Throws.When.Null<string>(null));
    }

    [Fact]
    public void Null_WithCustomExceptionCreator_UsesCustomException() {
        // arrange
        var custom = new InvalidOperationException("custom");

        // act & assert
        var ex = Assert.Throws<InvalidOperationException>(
            () => Throws.When.Null<string>(null, exceptionCreator: () => custom)
        );
        Assert.Same(custom, ex);
    }

    [Fact]
    public void Null_WithCustomMessage_MessageIsSet() {
        // act
        var ex = Assert.Throws<ArgumentNullException>(
            () => Throws.When.Null<string>(null, message: "custom message")
        );

        // assert
        Assert.Contains("custom message", ex.Message);
    }
    
    // ─── NullOrEmpty(string) ────────────────────────────────────────────────

    [Fact]
    public void NullOrEmpty_WithValidString_ReturnsValue() {
        // arrange
        var value = "hello";

        // act
        var result = Throws.When.NullOrEmpty(value);

        // assert
        Assert.Equal("hello", result);
    }

    [Fact]
    public void NullOrEmpty_WithNull_ThrowsArgumentNullException() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => Throws.When.NullOrEmpty(null));
    }

    [Fact]
    public void NullOrEmpty_WithEmptyString_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.NullOrEmpty(string.Empty));
    }

    // ─── NullOrWhiteSpace(string) ───────────────────────────────────────────

    [Fact]
    public void NullOrWhiteSpace_WithValidString_ReturnsValue() {
        // arrange
        var value = "hello";

        // act
        var result = Throws.When.NullOrWhiteSpace(value);

        // assert
        Assert.Equal("hello", result);
    }

    [Fact]
    public void NullOrWhiteSpace_WithNull_ThrowsArgumentNullException() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => Throws.When.NullOrWhiteSpace(null));
    }

    [Fact]
    public void NullOrWhiteSpace_WithEmptyString_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.NullOrWhiteSpace(string.Empty));
    }

    [Fact]
    public void NullOrWhiteSpace_WithWhitespaceOnlyString_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.NullOrWhiteSpace("   "));
    }

    // ─── NoMatchingPattern(string, string) ──────────────────────────────────

    [Fact]
    public void NoMatchingPattern_WhenPatternMatches_ReturnsValue() {
        // arrange
        var value = "hello123";

        // act
        var result = Throws.When.NoMatchingPattern(value, @"\w+");

        // assert
        Assert.Equal("hello123", result);
    }

    [Fact]
    public void NoMatchingPattern_WhenPatternDoesNotMatch_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(
            () => Throws.When.NoMatchingPattern("hello", @"^\d+$")
        );
    }

    [Fact]
    public void NoMatchingPattern_WithIgnoreCase_MatchesCaseInsensitively() {
        // act
        var result = Throws.When.NoMatchingPattern("HELLO", "hello", ignoreCase: true);

        // assert
        Assert.Equal("HELLO", result);
    }

    [Fact]
    public void NoMatchingPattern_WithRegexObject_WhenMatches_ReturnsValue() {
        // arrange
        var regex = new Regex(@"^\d+$");

        // act
        var result = Throws.When.NoMatchingPattern("123", regex);

        // assert
        Assert.Equal("123", result);
    }

    [Fact]
    public void NoMatchingPattern_WithRegexObject_WhenNoMatch_ThrowsArgumentException() {
        // arrange
        var regex = new Regex(@"^\d+$");

        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.NoMatchingPattern("abc", regex));
    }

    // ─── Empty(ReadOnlySpan<char>) ──────────────────────────────────────────

    [Fact]
    public void Empty_WithNonEmptySpan_ReturnsSpan() {
        // arrange
        ReadOnlySpan<char> span = "hello";

        // act
        var result = Throws.When.Empty(span);

        // assert
        Assert.True(result.SequenceEqual(span));
    }

    [Fact]
    public void Empty_WithEmptySpan_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.Empty(ReadOnlySpan<char>.Empty));
    }

    // ─── WhiteSpace(ReadOnlySpan<char>) ─────────────────────────────────────

    [Fact]
    public void WhiteSpace_WithNonWhitespaceSpan_ReturnsSpan() {
        // arrange
        ReadOnlySpan<char> span = "hello";

        // act
        var result = Throws.When.WhiteSpace(span);

        // assert
        Assert.True(result.SequenceEqual(span));
    }

    [Fact]
    public void WhiteSpace_WithWhitespaceOnlySpan_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.WhiteSpace("   ".AsSpan()));
    }

    // ─── Int32 guards ───────────────────────────────────────────────────────

    [Fact]
    public void LowerOrEqual_WhenValueGreaterThanCompare_ReturnsValue() {
        // act & assert
        Assert.Equal(10, Throws.When.LowerOrEqual(10, 5));
    }

    [Fact]
    public void LowerOrEqual_WhenValueEqualToCompare_ThrowsArgumentOutOfRangeException() {
        // act & assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.LowerOrEqual(5, 5));
    }

    [Fact]
    public void LowerOrEqual_WhenValueLessThanCompare_ThrowsArgumentOutOfRangeException() {
        // act & assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.LowerOrEqual(3, 5));
    }

    [Fact]
    public void GreaterOrEqual_WhenValueLessThanCompare_ReturnsValue() {
        // act & assert
        Assert.Equal(3, Throws.When.GreaterOrEqual(3, 5));
    }

    [Fact]
    public void GreaterOrEqual_WhenValueEqualToCompare_ThrowsArgumentOutOfRangeException() {
        // act & assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.GreaterOrEqual(5, 5));
    }

    [Fact]
    public void GreaterOrEqual_WhenValueGreaterThanCompare_ThrowsArgumentOutOfRangeException() {
        // act & assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.GreaterOrEqual(10, 5));
    }

    [Fact]
    public void LowerThan_WhenValueEqualToCompare_ReturnsValue() {
        // act & assert
        Assert.Equal(5, Throws.When.LowerThan(5, 5));
    }

    [Fact]
    public void LowerThan_WhenValueGreaterThanCompare_ReturnsValue() {
        // act & assert
        Assert.Equal(6, Throws.When.LowerThan(6, 5));
    }

    [Fact]
    public void LowerThan_WhenValueLessThanCompare_ThrowsArgumentOutOfRangeException() {
        // act & assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.LowerThan(3, 5));
    }

    [Fact]
    public void GreaterThan_WhenValueEqualToCompare_ReturnsValue() {
        // act & assert
        Assert.Equal(5, Throws.When.GreaterThan(5, 5));
    }

    [Fact]
    public void GreaterThan_WhenValueLessThanCompare_ReturnsValue() {
        // act & assert
        Assert.Equal(3, Throws.When.GreaterThan(3, 5));
    }

    [Fact]
    public void GreaterThan_WhenValueGreaterThanCompare_ThrowsArgumentOutOfRangeException() {
        // act & assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.GreaterThan(10, 5));
    }

    [Fact]
    public void OutOfRange_WhenValueWithinRange_ReturnsValue() {
        // act & assert
        Assert.Equal(5, Throws.When.OutOfRange(5, 1, 10));
    }

    [Fact]
    public void OutOfRange_WhenValueAtMinimumBoundary_ReturnsValue() {
        // act & assert
        Assert.Equal(1, Throws.When.OutOfRange(1, 1, 10));
    }

    [Fact]
    public void OutOfRange_WhenValueAtMaximumBoundary_ReturnsValue() {
        // act & assert
        Assert.Equal(10, Throws.When.OutOfRange(10, 1, 10));
    }

    [Fact]
    public void OutOfRange_WhenValueBelowRange_ThrowsArgumentOutOfRangeException() {
        // act & assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.OutOfRange(0, 1, 10));
    }

    [Fact]
    public void OutOfRange_WhenValueAboveRange_ThrowsArgumentOutOfRangeException() {
        // act & assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.OutOfRange(11, 1, 10));
    }

    [Fact]
    public void Zero_WithPositiveValue_ReturnsValue() {
        // act & assert
        Assert.Equal(1, Throws.When.Zero(1));
    }

    [Fact]
    public void Zero_WithZeroValue_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.Zero(0));
    }

    [Fact]
    public void Zero_WithNegativeValue_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.Zero(-1));
    }

    [Fact]
    public void Negative_WithZeroValue_ReturnsValue() {
        // act & assert
        Assert.Equal(0, Throws.When.Negative(0));
    }

    [Fact]
    public void Negative_WithPositiveValue_ReturnsValue() {
        // act & assert
        Assert.Equal(5, Throws.When.Negative(5));
    }

    [Fact]
    public void Negative_WithNegativeValue_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.Negative(-1));
    }

    // ─── Type guards ────────────────────────────────────────────────────────

    [Fact]
    public void NonConcreteType_WithConcreteClass_ReturnsType() {
        // act
        var result = Throws.When.NonConcreteType(typeof(List<int>));

        // assert
        Assert.Equal(typeof(List<int>), result);
    }

    [Fact]
    public void NonConcreteType_WithAbstractClass_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.NonConcreteType(typeof(AbstractBase)));
    }

    [Fact]
    public void NonConcreteType_WithInterface_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.NonConcreteType(typeof(IDisposable)));
    }

    [Fact]
    public void NonOpenGenericType_WithOpenGeneric_ReturnsType() {
        // act
        var result = Throws.When.NonOpenGenericType(typeof(List<>));

        // assert
        Assert.Equal(typeof(List<>), result);
    }

    [Fact]
    public void NonOpenGenericType_WithClosedGeneric_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.NonOpenGenericType(typeof(List<int>)));
    }

    [Fact]
    public void OpenGenericType_WithClosedGeneric_ReturnsType() {
        // act
        var result = Throws.When.OpenGenericType(typeof(List<int>));

        // assert
        Assert.Equal(typeof(List<int>), result);
    }

    [Fact]
    public void OpenGenericType_WithOpenGeneric_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.OpenGenericType(typeof(List<>)));
    }

    [Fact]
    public void NonAssignableFrom_WhenAssignable_ReturnsType() {
        // act
        var result = Throws.When.NonAssignableFrom(typeof(List<int>), typeof(IEnumerable<int>));

        // assert
        Assert.Equal(typeof(List<int>), result);
    }

    [Fact]
    public void NonAssignableFrom_WhenNotAssignable_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(
            () => Throws.When.NonAssignableFrom(typeof(string), typeof(IDisposable))
        );
    }

    [Fact]
    public void HasNoParameterlessConstructor_WithParameterlessCtor_ReturnsType() {
        // act
        var result = Throws.When.HasNoParameterlessConstructor(typeof(List<int>));

        // assert
        Assert.Equal(typeof(List<int>), result);
    }

    [Fact]
    public void HasNoParameterlessConstructor_WithoutParameterlessCtor_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(
            () => Throws.When.HasNoParameterlessConstructor(typeof(NoDefaultCtor))
        );
    }

    // ─── NonAssignableFromGeneric ──────────────────────────────────────────────

    [Fact]
    public void NonAssignableFromGeneric_WhenAssignable_ReturnsType() {
        var result = Throws.When.NonAssignableFromGeneric(typeof(List<int>), typeof(IEnumerable<>));
        Assert.Equal(typeof(List<int>), result);
    }

    [Fact]
    public void NonAssignableFromGeneric_WhenNotAssignable_ThrowsArgumentException() {
        Assert.Throws<ArgumentException>(
            () => Throws.When.NonAssignableFromGeneric(typeof(Uri), typeof(IEnumerable<>))
        );
    }

    // ─── TimeSpan guards ─────────────────────────────────────────────────────────

    [Fact]
    public void TimeSpan_LowerOrEqual_WhenValueGreater_ReturnsValue() {
        var t = TimeSpan.FromSeconds(10);
        Assert.Equal(t, Throws.When.LowerOrEqual(t, TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public void TimeSpan_LowerOrEqual_WhenValueEqual_ThrowsArgumentOutOfRangeException() {
        var t = TimeSpan.FromSeconds(5);
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.LowerOrEqual(t, t));
    }

    [Fact]
    public void TimeSpan_GreaterOrEqual_WhenValueLess_ReturnsValue() {
        var t = TimeSpan.FromSeconds(3);
        Assert.Equal(t, Throws.When.GreaterOrEqual(t, TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public void TimeSpan_GreaterOrEqual_WhenValueEqual_ThrowsArgumentOutOfRangeException() {
        var t = TimeSpan.FromSeconds(5);
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.GreaterOrEqual(t, t));
    }

    [Fact]
    public void TimeSpan_LowerThan_WhenValueEqual_ReturnsValue() {
        var t = TimeSpan.FromSeconds(5);
        Assert.Equal(t, Throws.When.LowerThan(t, t));
    }

    [Fact]
    public void TimeSpan_LowerThan_WhenValueLess_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Throws.When.LowerThan(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5))
        );
    }

    [Fact]
    public void TimeSpan_GreaterThan_WhenValueEqual_ReturnsValue() {
        var t = TimeSpan.FromSeconds(5);
        Assert.Equal(t, Throws.When.GreaterThan(t, t));
    }

    [Fact]
    public void TimeSpan_GreaterThan_WhenValueGreater_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Throws.When.GreaterThan(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5))
        );
    }

    [Fact]
    public void TimeSpan_OutOfRange_WhenWithin_ReturnsValue() {
        var t = TimeSpan.FromSeconds(5);
        Assert.Equal(t, Throws.When.OutOfRange(t, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10)));
    }

    [Fact]
    public void TimeSpan_OutOfRange_WhenAbove_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Throws.When.OutOfRange(TimeSpan.FromSeconds(11), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10))
        );
    }

    [Fact]
    public void TimeSpan_Zero_WithPositiveValue_ReturnsValue() {
        var t = TimeSpan.FromSeconds(1);
        Assert.Equal(t, Throws.When.Zero(t));
    }

    [Fact]
    public void TimeSpan_Zero_WithZero_ThrowsArgumentException() {
        Assert.Throws<ArgumentException>(() => Throws.When.Zero(TimeSpan.Zero));
    }

    [Fact]
    public void TimeSpan_Negative_WithZeroValue_ReturnsValue() {
        Assert.Equal(TimeSpan.Zero, Throws.When.Negative(TimeSpan.Zero));
    }

    [Fact]
    public void TimeSpan_Negative_WithNegativeValue_ThrowsArgumentException() {
        Assert.Throws<ArgumentException>(() => Throws.When.Negative(TimeSpan.FromSeconds(-1)));
    }

    // ─── test doubles ────────────────────────────────────────────────────────

    private abstract class AbstractBase { }
    #pragma warning disable CS9113
    private sealed class NoDefaultCtor(int value) { }
    #pragma warning restore CS9113
}
