namespace Nameless;

public class ArrayExtensionTests {

    [Test]
    public void Get_An_Item_By_Index_Without_Fuss() {
        // arrange
        var array = new[] { 1, 2, 3 };

        // act
        ArrayExtension.TryGetElementAt(array, 1, out var item);

        // assert
        Assert.That(item, Is.EqualTo(2));
    }

    [Test]
    public void If_The_Item_Exists_Returns_True_And_Output() {
        // arrange
        var array = new[] { 1, 2, 3 };

        // act
        var exists = ArrayExtension.TryGetElementAt(array, 1, out var item);

        // assert
        Assert.Multiple(() => {
            Assert.That(item, Is.EqualTo(2));
            Assert.That(exists, Is.True);
        });
    }

    [Test]
    public void If_The_Item_Does_Not_Exists_Returns_False_And_Output_Default() {
        // arrange
        var array = new[] { 1, 2, 3 };

        // act
        var exists = ArrayExtension.TryGetElementAt(array, 5, out var item);

        // assert
        Assert.Multiple(() => {
            Assert.That(item, Is.EqualTo(default(int)));
            Assert.That(exists, Is.False);
        });
    }

    [Test]
    public void When_Array_Has_Elements_Then_IsInRange_Returns_True_If_Asked_For_Element_Inside_Range() {
        // arrange
        var array = new[] { 1, 2, 3, 4, 5 };

        // act
        var isInRange = ArrayExtension.IsInRange(array, index: 2); // number 3

        // assert
        Assert.That(isInRange, Is.True);
    }

    [Test]
    public void When_Array_Has_Elements_Then_IsInRange_Returns_False_If_Asked_For_Element_Outside_Range() {
        // arrange
        var array = new[] { 1, 2, 3, 4, 5 };

        // act
        var isInRange = ArrayExtension.IsInRange(array, index: 5); // out of range

        // assert
        Assert.That(isInRange, Is.False);
    }

    [Test]
    public void When_Array_Has_Elements_Then_IsInRange_Returns_False_If_Asked_Negative_Position() {
        // arrange
        var array = new[] { 1, 2, 3, 4, 5 };

        // act
        var isInRange = ArrayExtension.IsInRange(array, index: -1); // out of range

        // assert
        Assert.That(isInRange, Is.False);
    }
}