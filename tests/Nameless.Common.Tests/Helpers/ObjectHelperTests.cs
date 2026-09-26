namespace Nameless.Helpers;

[UnitTest]
public class ObjectHelperTests {
    private sealed class Sample {
        public string Name { get; set; } = "alpha";
        public int Count { get; set; } = 3;
        public string? Missing { get; set; }
        internal string Hidden { get; set; } = "x";
        public static string Static => "static";
    }

    [Fact]
    public void Transform_WithNull_ReturnsEmptyDictionary() {
        // act & assert
        Assert.Empty(ObjectHelper.Transform(null));
    }

    [Fact]
    public void Transform_WithObject_ReturnsPublicInstanceProperties() {
        // act
        var actual = ObjectHelper.Transform(new Sample());

        // assert
        Assert.Multiple(
            () => Assert.Equal(3, actual.Count),
            () => Assert.Equal("alpha", actual["Name"]),
            () => Assert.Equal(3, actual["Count"]),
            () => Assert.Null(actual["Missing"])
        );
    }

    [Fact]
    public void Transform_WithAnonymousObject_ReturnsItsProperties() {
        // act
        var actual = ObjectHelper.Transform(new { A = 1, B = "two" });

        // assert
        Assert.Multiple(
            () => Assert.Equal(1, actual["A"]),
            () => Assert.Equal("two", actual["B"])
        );
    }
}
