using System.Linq.Expressions;

namespace Nameless;

public class ExpressionExtensionsTests {
    // ─── GetExpressionPath ──────────────────────────────────────────────────

    [Fact]
    public void GetExpressionPath_WithArrayIndex_ReturnsPathWithBrackets() {
        // arrange
        Expression<Func<int[], int>> expr = a => a[0];

        // act
        var path = expr.GetExpressionPath();

        // assert
        Assert.Multiple(() => {
            Assert.StartsWith("a", path);
            Assert.Contains("[", path);
            Assert.Contains("]", path);
        });
    }

    [Fact]
    public void GetExpressionPath_WithStringIndexer_ReturnsPathWithBrackets() {
        // arrange — string has [DefaultMember("Chars")] so s[0] is a Call/IsSingleArgumentIndexer
        Expression<Func<string, char>> expr = s => s[0];

        // act
        var path = expr.GetExpressionPath();

        // assert
        Assert.Multiple(() => {
            Assert.StartsWith("s", path);
            Assert.Contains("[", path);
            Assert.Contains("]", path);
        });
    }

    [Fact]
    public void GetExpressionPath_WithConstantBody_ReturnsEmpty() {
        // arrange — Constant is the default/unsupported case → segmentCount stays 0 → returns ""
        var expr = Expression.Lambda<Func<int>>(Expression.Constant(42));

        // act
        var path = expr.GetExpressionPath();

        // assert
        Assert.Equal(string.Empty, path);
    }

    [Fact]
    public void GetExpressionPath_WithSimpleProperty_ReturnsParameterAndPropertyName() {
        // arrange
        Expression<Func<Person, string>> expr = p => p.Name;

        // act
        var path = expr.GetExpressionPath();

        // assert
        Assert.Equal("p.Name", path);
    }

    [Fact]
    public void GetExpressionPath_WithNestedProperty_ReturnsDotSeparatedPath() {
        // arrange
        Expression<Func<Person, string>> expr = p => p.Address.City;

        // act
        var path = expr.GetExpressionPath();

        // assert
        Assert.Equal("p.Address.City", path);
    }

    // ─── And ────────────────────────────────────────────────────────────────

    [Fact]
    public void And_WithTwoExpressions_ReturnsTrueOnlyWhenBothMatch() {
        // arrange
        Expression<Func<int, bool>> isPositive = x => x > 0;
        Expression<Func<int, bool>> isLessThanTen = x => x < 10;

        // act
        var combined = isPositive.And(isLessThanTen);
        var compiled = combined.Compile();

        // assert
        Assert.Multiple(() => {
            Assert.True(compiled(5));
            Assert.False(compiled(-1));
            Assert.False(compiled(15));
        });
    }

    // ─── Or ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Or_WithTwoExpressions_ReturnsTrueWhenEitherMatches() {
        // arrange
        Expression<Func<int, bool>> isNegative = x => x < 0;
        Expression<Func<int, bool>> isLargePositive = x => x > 100;

        // act
        var combined = isNegative.Or(isLargePositive);
        var compiled = combined.Compile();

        // assert
        Assert.Multiple(() => {
            Assert.True(compiled(-5));
            Assert.True(compiled(200));
            Assert.False(compiled(50));
        });
    }

    // ─── test doubles ─────────────────────────────────────────────────────────

    private sealed class Person {
        public string Name { get; set; } = string.Empty;
        public Address Address { get; set; } = new();
    }

    private sealed class Address {
        public string City { get; set; } = string.Empty;
    }
}
