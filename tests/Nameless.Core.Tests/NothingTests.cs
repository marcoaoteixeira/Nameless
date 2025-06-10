namespace Nameless;

public class NothingTests {
    [Fact]
    public void WhenGettingValue_ThenReturnsInstance() {
        // arrange, act & assert
        Assert.Equal(Nothing.Value, Nothing.Value);
    }

    [Fact]
    public void WhenComparing_AlwaysReturnsZeroValue() {
        // arrange
        const int Expected = 0;

        // act
        var valueX = new Nothing();
        var valueY = new Nothing();
        var valueZ = new Nothing();

        // assert
        Assert.Equal(Expected, valueX.CompareTo(valueX));
        Assert.Equal(Expected, valueX.CompareTo(valueY));
        Assert.Equal(Expected, valueX.CompareTo(valueZ));

        Assert.Equal(Expected, valueY.CompareTo(valueX));
        Assert.Equal(Expected, valueY.CompareTo(valueY));
        Assert.Equal(Expected, valueY.CompareTo(valueZ));

        Assert.Equal(Expected, valueZ.CompareTo(valueX));
        Assert.Equal(Expected, valueZ.CompareTo(valueY));
        Assert.Equal(Expected, valueZ.CompareTo(valueZ));
    }
}
