namespace Nameless;

public class ArrayExtensionsTests {
    [Fact]
    public void Get_An_Item_By_Index_Without_Fuss() {
        // arrange
        var array = new[] { 1, 2, 3 };

        // act
        array.TryGetElementAt(index: 1, out var item);

        // assert
        Assert.Equal(expected: 2, item);
    }

    [Fact]
    public void If_The_Item_Exists_Returns_True_And_Output() {
        // arrange
        var array = new[] { 1, 2, 3 };

        // act
        var exists = array.TryGetElementAt(index: 1, out var item);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(expected: 2, item);
            Assert.True(exists);
        });
    }

    [Fact]
    public void If_The_Item_Does_Not_Exists_Returns_False_And_Output_Default() {
        // arrange
        var array = new[] { 1, 2, 3 };

        // act
        var exists = array.TryGetElementAt(index: 5, out var item);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(expected: 0, item);
            Assert.False(exists);
        });
    }

    [Fact]
    public void When_Array_Has_Elements_Then_IsInRange_Returns_True_If_Asked_For_Element_Inside_Range() {
        // arrange
        var array = new[] { 1, 2, 3, 4, 5 };

        // act
        var isInRange = array.IsInRange(index: 2); // number 3

        // assert
        Assert.True(isInRange);
    }

    [Fact]
    public void When_Array_Has_Elements_Then_IsInRange_Returns_False_If_Asked_For_Element_Outside_Range() {
        // arrange
        var array = new[] { 1, 2, 3, 4, 5 };

        // act
        var isInRange = array.IsInRange(index: 5); // out of range

        // assert
        Assert.False(isInRange);
    }

    [Fact]
    public void When_Array_Has_Elements_Then_IsInRange_Returns_False_If_Asked_Negative_Position() {
        // arrange
        var array = new[] { 1, 2, 3, 4, 5 };

        // act
        var isInRange = array.IsInRange(index: -1); // out of range

        // assert
        Assert.False(isInRange);
    }
}