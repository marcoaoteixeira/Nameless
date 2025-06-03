namespace Nameless;

public class IntegerExtensionsTests {
    [Fact]
    public void Times_Should_Execute_Action_X_Times() {
        // arrange
        var times = 5;
        var array = new List<int>();
        var expected = new[] { 5, 5, 5, 5, 5 };

        // act
        times.Times(() => array.Add(times));

        // assert
        Assert.That(array, Is.EquivalentTo(expected));
    }

    [Fact]
    public void Times_Should_Execute_Action_With_Index_X_Times() {
        // arrange
        var times = 5;
        var array = new int[times];
        var expected = new[] { 0, 1, 2, 3, 4 };

        // act
        times.Times(idx => array[idx] = idx);

        // assert
        Assert.That(array, Is.EquivalentTo(expected));
    }
}