using Nameless.Fixtures;

namespace Nameless;

public class ObjectExtensionsTests {
    [Fact]
    public void IsAnonymous_Should_Returns_True_If_Anonymous_Object_Instance() {
        // arrange
        var obj = new { Id = 1, Name = "John Wick" };

        // act
        var actual = obj.IsAnonymous();

        // assert
        Assert.True(actual);
    }

    [Fact]
    public void IsAnonymous_Should_Returns_False_If_Class_Instance() {
        // arrange
        var obj = new Student { Age = 50, Name = "John Wick" };

        // act
        var actual = obj.IsAnonymous();

        // assert
        Assert.False(actual);
    }
}