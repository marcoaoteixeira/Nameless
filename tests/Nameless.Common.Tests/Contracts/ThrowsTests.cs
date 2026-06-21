using System.Collections;
using System.Text.RegularExpressions;

namespace Nameless.Contracts;

public class ThrowsTests {
    // ─── Singleton ─────────────────────────────────────────────────────────

    [Fact]
    public void When_IsSingleton_ReturnsSameInstance() {
        // act & assert
        Assert.Same(Throws.When, Throws.When);
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

    // ─── Default<T> ─────────────────────────────────────────────────────────

    [Fact]
    public void Default_WithNonDefaultValue_ReturnsValue() {
        // arrange
        var value = 42;

        // act
        var result = Throws.When.Default(value);

        // assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void Default_WithNullReferenceType_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.Default<string>(null));
    }

    [Fact]
    public void Default_WithDefaultValueType_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.Default(default(int)));
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
        Assert.Throws<ArgumentNullException>(() => Throws.When.NullOrEmpty((string?)null));
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

    // ─── NullOrEmpty<T>(IEnumerable) ────────────────────────────────────────

    [Fact]
    public void NullOrEmptyEnumerable_WithNonEmptyList_ReturnsValue() {
        // arrange
        var list = new List<int> { 1, 2, 3 };

        // act
        var result = Throws.When.NullOrEmpty(list);

        // assert
        Assert.Same(list, result);
    }

    [Fact]
    public void NullOrEmptyEnumerable_WithNull_ThrowsArgumentNullException() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => Throws.When.NullOrEmpty<List<int>>(null));
    }

    [Fact]
    public void NullOrEmptyEnumerable_WithEmptyArray_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.NullOrEmpty(Array.Empty<int>()));
    }

    [Fact]
    public void NullOrEmptyEnumerable_WithEmptyICollection_ThrowsArgumentException() {
        // arrange
        ICollection<int> empty = new List<int>();

        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.NullOrEmpty(empty));
    }

    [Fact]
    public void NullOrEmptyEnumerable_WithEmptyPlainEnumerable_ThrowsArgumentException() {
        // arrange
        static IEnumerable YieldNothing() { yield break; }
        var empty = YieldNothing();

        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.NullOrEmpty(empty));
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
    public void IsNonConcreteType_WithConcreteClass_ReturnsType() {
        // act
        var result = Throws.When.IsNonConcreteType(typeof(List<int>));

        // assert
        Assert.Equal(typeof(List<int>), result);
    }

    [Fact]
    public void IsNonConcreteType_WithAbstractClass_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.IsNonConcreteType(typeof(AbstractBase)));
    }

    [Fact]
    public void IsNonConcreteType_WithInterface_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.IsNonConcreteType(typeof(IDisposable)));
    }

    [Fact]
    public void IsNonOpenGenericType_WithOpenGeneric_ReturnsType() {
        // act
        var result = Throws.When.IsNonOpenGenericType(typeof(List<>));

        // assert
        Assert.Equal(typeof(List<>), result);
    }

    [Fact]
    public void IsNonOpenGenericType_WithClosedGeneric_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.IsNonOpenGenericType(typeof(List<int>)));
    }

    [Fact]
    public void IsOpenGenericType_WithClosedGeneric_ReturnsType() {
        // act
        var result = Throws.When.IsOpenGenericType(typeof(List<int>));

        // assert
        Assert.Equal(typeof(List<int>), result);
    }

    [Fact]
    public void IsOpenGenericType_WithOpenGeneric_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.IsOpenGenericType(typeof(List<>)));
    }

    [Fact]
    public void IsNotAssignableFrom_WhenAssignable_ReturnsType() {
        // act
        var result = Throws.When.IsNotAssignableFrom(typeof(List<int>), typeof(IEnumerable<int>));

        // assert
        Assert.Equal(typeof(List<int>), result);
    }

    [Fact]
    public void IsNotAssignableFrom_WhenNotAssignable_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(
            () => Throws.When.IsNotAssignableFrom(typeof(string), typeof(IDisposable))
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

    // ─── DateTime guards ────────────────────────────────────────────────────────

    [Fact]
    public void DateTime_LowerOrEqual_WhenValueGreaterThanCompare_ReturnsValue() {
        var d = new DateTime(2024, 6, 1);
        Assert.Equal(d, Throws.When.LowerOrEqual(d, new DateTime(2024, 1, 1)));
    }

    [Fact]
    public void DateTime_LowerOrEqual_WhenValueEqualToCompare_ThrowsArgumentOutOfRangeException() {
        var d = new DateTime(2024, 1, 1);
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.LowerOrEqual(d, d));
    }

    [Fact]
    public void DateTime_LowerOrEqual_WhenValueLessThanCompare_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Throws.When.LowerOrEqual(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1))
        );
    }

    [Fact]
    public void DateTime_GreaterOrEqual_WhenValueLessThanCompare_ReturnsValue() {
        var d = new DateTime(2024, 1, 1);
        Assert.Equal(d, Throws.When.GreaterOrEqual(d, new DateTime(2024, 6, 1)));
    }

    [Fact]
    public void DateTime_GreaterOrEqual_WhenValueEqualToCompare_ThrowsArgumentOutOfRangeException() {
        var d = new DateTime(2024, 1, 1);
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.GreaterOrEqual(d, d));
    }

    [Fact]
    public void DateTime_GreaterOrEqual_WhenValueGreaterThanCompare_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Throws.When.GreaterOrEqual(new DateTime(2024, 6, 1), new DateTime(2024, 1, 1))
        );
    }

    [Fact]
    public void DateTime_LowerThan_WhenValueEqualToCompare_ReturnsValue() {
        var d = new DateTime(2024, 1, 1);
        Assert.Equal(d, Throws.When.LowerThan(d, d));
    }

    [Fact]
    public void DateTime_LowerThan_WhenValueLessThanCompare_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Throws.When.LowerThan(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1))
        );
    }

    [Fact]
    public void DateTime_GreaterThan_WhenValueEqualToCompare_ReturnsValue() {
        var d = new DateTime(2024, 1, 1);
        Assert.Equal(d, Throws.When.GreaterThan(d, d));
    }

    [Fact]
    public void DateTime_GreaterThan_WhenValueGreaterThanCompare_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Throws.When.GreaterThan(new DateTime(2024, 6, 1), new DateTime(2024, 1, 1))
        );
    }

    [Fact]
    public void DateTime_OutOfRange_WhenValueWithinRange_ReturnsValue() {
        var d = new DateTime(2024, 3, 1);
        Assert.Equal(d, Throws.When.OutOfRange(d, new DateTime(2024, 1, 1), new DateTime(2024, 6, 1)));
    }

    [Fact]
    public void DateTime_OutOfRange_WhenValueBelowRange_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Throws.When.OutOfRange(new DateTime(2023, 1, 1), new DateTime(2024, 1, 1), new DateTime(2024, 6, 1))
        );
    }

    // ─── DateTimeOffset guards ──────────────────────────────────────────────────

    [Fact]
    public void DateTimeOffset_LowerOrEqual_WhenValueGreater_ReturnsValue() {
        var d = new DateTimeOffset(2024, 6, 1, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(d, Throws.When.LowerOrEqual(d, new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void DateTimeOffset_LowerOrEqual_WhenValueEqual_ThrowsArgumentOutOfRangeException() {
        var d = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.LowerOrEqual(d, d));
    }

    [Fact]
    public void DateTimeOffset_GreaterOrEqual_WhenValueLess_ReturnsValue() {
        var d = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(d, Throws.When.GreaterOrEqual(d, new DateTimeOffset(2024, 6, 1, 0, 0, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void DateTimeOffset_GreaterOrEqual_WhenValueEqual_ThrowsArgumentOutOfRangeException() {
        var d = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.GreaterOrEqual(d, d));
    }

    [Fact]
    public void DateTimeOffset_LowerThan_WhenValueEqual_ReturnsValue() {
        var d = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(d, Throws.When.LowerThan(d, d));
    }

    [Fact]
    public void DateTimeOffset_LowerThan_WhenValueLess_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Throws.When.LowerThan(
                new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2024, 6, 1, 0, 0, 0, TimeSpan.Zero))
        );
    }

    [Fact]
    public void DateTimeOffset_GreaterThan_WhenValueEqual_ReturnsValue() {
        var d = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(d, Throws.When.GreaterThan(d, d));
    }

    [Fact]
    public void DateTimeOffset_GreaterThan_WhenValueGreater_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Throws.When.GreaterThan(
                new DateTimeOffset(2024, 6, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero))
        );
    }

    [Fact]
    public void DateTimeOffset_OutOfRange_WhenWithin_ReturnsValue() {
        var d = new DateTimeOffset(2024, 3, 1, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(d, Throws.When.OutOfRange(
            d,
            new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 6, 1, 0, 0, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void DateTimeOffset_OutOfRange_WhenAbove_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.OutOfRange(
            new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 6, 1, 0, 0, 0, TimeSpan.Zero)));
    }

    // ─── Decimal guards ──────────────────────────────────────────────────────────

    [Fact]
    public void Decimal_LowerOrEqual_WhenValueGreater_ReturnsValue() {
        Assert.Equal(10m, Throws.When.LowerOrEqual(10m, 5m));
    }

    [Fact]
    public void Decimal_LowerOrEqual_WhenValueEqual_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.LowerOrEqual(5m, 5m));
    }

    [Fact]
    public void Decimal_GreaterOrEqual_WhenValueLess_ReturnsValue() {
        Assert.Equal(3m, Throws.When.GreaterOrEqual(3m, 5m));
    }

    [Fact]
    public void Decimal_GreaterOrEqual_WhenValueEqual_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.GreaterOrEqual(5m, 5m));
    }

    [Fact]
    public void Decimal_LowerThan_WhenValueEqual_ReturnsValue() {
        Assert.Equal(5m, Throws.When.LowerThan(5m, 5m));
    }

    [Fact]
    public void Decimal_LowerThan_WhenValueLess_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.LowerThan(3m, 5m));
    }

    [Fact]
    public void Decimal_GreaterThan_WhenValueEqual_ReturnsValue() {
        Assert.Equal(5m, Throws.When.GreaterThan(5m, 5m));
    }

    [Fact]
    public void Decimal_GreaterThan_WhenValueGreater_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.GreaterThan(10m, 5m));
    }

    [Fact]
    public void Decimal_OutOfRange_WhenWithin_ReturnsValue() {
        Assert.Equal(5m, Throws.When.OutOfRange(5m, 1m, 10m));
    }

    [Fact]
    public void Decimal_OutOfRange_WhenBelow_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.OutOfRange(0m, 1m, 10m));
    }

    [Fact]
    public void Decimal_Zero_WithPositiveValue_ReturnsValue() {
        Assert.Equal(1m, Throws.When.Zero(1m));
    }

    [Fact]
    public void Decimal_Zero_WithZero_ThrowsArgumentException() {
        Assert.Throws<ArgumentException>(() => Throws.When.Zero(0m));
    }

    [Fact]
    public void Decimal_Negative_WithZeroValue_ReturnsValue() {
        Assert.Equal(0m, Throws.When.Negative(0m));
    }

    [Fact]
    public void Decimal_Negative_WithNegativeValue_ThrowsArgumentException() {
        Assert.Throws<ArgumentException>(() => Throws.When.Negative(-1m));
    }

    // ─── Double guards ───────────────────────────────────────────────────────────

    [Fact]
    public void Double_LowerOrEqual_WhenValueGreater_ReturnsValue() {
        Assert.Equal(10d, Throws.When.LowerOrEqual(10d, 5d));
    }

    [Fact]
    public void Double_LowerOrEqual_WhenValueEqual_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.LowerOrEqual(5d, 5d));
    }

    [Fact]
    public void Double_GreaterOrEqual_WhenValueLess_ReturnsValue() {
        Assert.Equal(3d, Throws.When.GreaterOrEqual(3d, 5d));
    }

    [Fact]
    public void Double_GreaterOrEqual_WhenValueEqual_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.GreaterOrEqual(5d, 5d));
    }

    [Fact]
    public void Double_LowerThan_WhenValueEqual_ReturnsValue() {
        Assert.Equal(5d, Throws.When.LowerThan(5d, 5d));
    }

    [Fact]
    public void Double_LowerThan_WhenValueLess_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.LowerThan(3d, 5d));
    }

    [Fact]
    public void Double_GreaterThan_WhenValueEqual_ReturnsValue() {
        Assert.Equal(5d, Throws.When.GreaterThan(5d, 5d));
    }

    [Fact]
    public void Double_GreaterThan_WhenValueGreater_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.GreaterThan(10d, 5d));
    }

    [Fact]
    public void Double_OutOfRange_WhenWithin_ReturnsValue() {
        Assert.Equal(5d, Throws.When.OutOfRange(5d, 1d, 10d));
    }

    [Fact]
    public void Double_OutOfRange_WhenAbove_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.OutOfRange(11d, 1d, 10d));
    }

    [Fact]
    public void Double_Zero_WithPositiveValue_ReturnsValue() {
        Assert.Equal(1d, Throws.When.Zero(1d));
    }

    [Fact]
    public void Double_Zero_WithZero_ThrowsArgumentException() {
        Assert.Throws<ArgumentException>(() => Throws.When.Zero(0d));
    }

    [Fact]
    public void Double_Negative_WithZeroValue_ReturnsValue() {
        Assert.Equal(0d, Throws.When.Negative(0d));
    }

    [Fact]
    public void Double_Negative_WithNegativeValue_ThrowsArgumentException() {
        Assert.Throws<ArgumentException>(() => Throws.When.Negative(-1d));
    }

    // ─── Int64 guards ────────────────────────────────────────────────────────────

    [Fact]
    public void Int64_LowerOrEqual_WhenValueGreater_ReturnsValue() {
        Assert.Equal(10L, Throws.When.LowerOrEqual(10L, 5L));
    }

    [Fact]
    public void Int64_LowerOrEqual_WhenValueEqual_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.LowerOrEqual(5L, 5L));
    }

    [Fact]
    public void Int64_GreaterOrEqual_WhenValueLess_ReturnsValue() {
        Assert.Equal(3L, Throws.When.GreaterOrEqual(3L, 5L));
    }

    [Fact]
    public void Int64_GreaterOrEqual_WhenValueEqual_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.GreaterOrEqual(5L, 5L));
    }

    [Fact]
    public void Int64_LowerThan_WhenValueEqual_ReturnsValue() {
        Assert.Equal(5L, Throws.When.LowerThan(5L, 5L));
    }

    [Fact]
    public void Int64_LowerThan_WhenValueLess_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.LowerThan(3L, 5L));
    }

    [Fact]
    public void Int64_GreaterThan_WhenValueEqual_ReturnsValue() {
        Assert.Equal(5L, Throws.When.GreaterThan(5L, 5L));
    }

    [Fact]
    public void Int64_GreaterThan_WhenValueGreater_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.GreaterThan(10L, 5L));
    }

    [Fact]
    public void Int64_OutOfRange_WhenWithin_ReturnsValue() {
        Assert.Equal(5L, Throws.When.OutOfRange(5L, 1L, 10L));
    }

    [Fact]
    public void Int64_OutOfRange_WhenBelow_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => Throws.When.OutOfRange(0L, 1L, 10L));
    }

    [Fact]
    public void Int64_Zero_WithPositiveValue_ReturnsValue() {
        Assert.Equal(1L, Throws.When.Zero(1L));
    }

    [Fact]
    public void Int64_Zero_WithZero_ThrowsArgumentException() {
        Assert.Throws<ArgumentException>(() => Throws.When.Zero(0L));
    }

    [Fact]
    public void Int64_Negative_WithZeroValue_ReturnsValue() {
        Assert.Equal(0L, Throws.When.Negative(0L));
    }

    [Fact]
    public void Int64_Negative_WithNegativeValue_ThrowsArgumentException() {
        Assert.Throws<ArgumentException>(() => Throws.When.Negative(-1L));
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

    // ─── Guid guards ─────────────────────────────────────────────────────────────

    [Fact]
    public void Guid_NullOrEmpty_WithValidGuid_ReturnsValue() {
        var id = Guid.NewGuid();
        Assert.Equal(id, Throws.When.NullOrEmpty((Guid?)id));
    }

    [Fact]
    public void Guid_NullOrEmpty_WithNull_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() => Throws.When.NullOrEmpty((Guid?)null));
    }

    [Fact]
    public void Guid_NullOrEmpty_WithEmptyGuid_ThrowsArgumentException() {
        Assert.Throws<ArgumentException>(() => Throws.When.NullOrEmpty((Guid?)Guid.Empty));
    }

    // ─── IsNotAssignableFromGeneric ──────────────────────────────────────────────

    [Fact]
    public void IsNotAssignableFromGeneric_WhenAssignable_ReturnsType() {
        var result = Throws.When.IsNotAssignableFromGeneric(typeof(List<int>), typeof(IEnumerable<>));
        Assert.Equal(typeof(List<int>), result);
    }

    [Fact]
    public void IsNotAssignableFromGeneric_WhenNotAssignable_ThrowsArgumentException() {
        Assert.Throws<ArgumentException>(
            () => Throws.When.IsNotAssignableFromGeneric(typeof(Uri), typeof(IEnumerable<>))
        );
    }

    // ─── test doubles ────────────────────────────────────────────────────────

    private abstract class AbstractBase { }
#pragma warning disable CS9113
    private sealed class NoDefaultCtor(int value) { }
#pragma warning restore CS9113
}
