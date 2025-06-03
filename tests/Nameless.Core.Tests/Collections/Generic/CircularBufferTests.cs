namespace Nameless.Collections.Generic;

public class CircularBufferTests {
    [Theory]
    [InlineData(1, new object[] { 1 })]
    [InlineData(10, new object[] { })]
    [InlineData(10, new object[] { 1 })]
    public void When_Creating_A_New_Instance_Then_Capacity_Should_Be_Greater_Or_Equal_To_Initial_Items_Length(int capacity, object[] items) {
        var exception = Record.Exception(() => new CircularBuffer<object>(capacity, items));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData(1, new object[] { 1, 2, 3 })]
    [InlineData(0, new object[] { 1 })]
    public void When_Creating_A_New_Instance_Then_Throws_ArgumentOutOfRangeException_If_Capacity_Is_Less_Than_Initial_Items_Length(int capacity, object[] items) {
        // act & assert
        Assert.Throws<ArgumentOutOfRangeException>(() => {
            _ = new CircularBuffer<object>(capacity, items);
        });
    }

    [Fact]
    public void When_Adding_A_New_Item_Into_CircularBuffer_Then_Count_Should_Increment() {
        // arrange
        var circularBuffer = new CircularBuffer<int>(10);

        // act
        var currentCount = circularBuffer.Count;
        circularBuffer.Add(1);
        var newCount = circularBuffer.Count;

        // assert
        Assert.Multiple(() => {
            Assert.Equal(expected: 0, currentCount);
            Assert.Equal(expected: 1, newCount);
        });
    }

    [Fact]
    public void When_Adding_More_Items_Than_Capacity_Into_CircularBuffer_Then_Count_Should_Be_Equal_To_Max_Capacity() {
        // arrange
        const int capacity = 10;
        const int total = 15;
        var circularBuffer = new CircularBuffer<int>(capacity);

        // act
        for (var count = 0; count < total; count++) {
            circularBuffer.Add(count + 1);
        }

        // assert
        Assert.Equal(expected: capacity, circularBuffer.Count);
    }

    [Fact]
    public void When_Clear_CircularBuffer_Then_Count_Should_Be_Zero() {
        // arrange
        const int capacity = 10;
        const int total = 10;
        var circularBuffer = new CircularBuffer<int>(capacity);

        // act
        for (var count = 0; count < total; count++) {
            circularBuffer.Add(count + 1);
        }

        var countBeforeClear = circularBuffer.Count;
        circularBuffer.Clear();
        var countAfterClear = circularBuffer.Count;

        // assert
        Assert.Multiple(() => {
            Assert.Equal(expected: total, countBeforeClear);
            Assert.Equal(expected: 0, countAfterClear);
        });
    }

    [Fact]
    public void
        When_Returning_IndexOf_Element_From_CircularBuffer_Then_Index_Should_Be_Greater_Than_Or_Equal_To_Zero_If_It_Exists() {
        // arrange
        var buffer = new[] { 1, 2, 3, 4, 5 };
        var circularBuffer = new CircularBuffer<int>(buffer.Length, buffer);
        const int expected = 2;

        // act
        var actual = circularBuffer.IndexOf(3);

        // assert
        Assert.Equal(expected: expected, actual);
    }

    [Fact]
    public void
        When_Returning_IndexOf_Element_From_CircularBuffer_With_Multiple_Identical_Elements_Then_Returns_Index_Of_First_Occurrence() {
        // arrange
        var buffer = new[] { 3, 3, 3, 4, 5 };
        var circularBuffer = new CircularBuffer<int>(buffer.Length, buffer);
        const int expected = 0;

        // act
        var actual = circularBuffer.IndexOf(3);

        // assert
        Assert.Equal(expected: expected, actual);
    }

    [Fact]
    public void
        When_Returning_IndexOf_Element_From_CircularBuffer_For_A_Non_Existent_Element_Then_Index_Should_Be_Negative() {
        // arrange
        var buffer = new[] { 1, 2, 3, 4, 5 };
        var circularBuffer = new CircularBuffer<int>(buffer.Length, buffer);
        const int expected = -1;

        // act
        var actual = circularBuffer.IndexOf(6);

        // assert
        Assert.Equal(expected: expected, actual);
    }

    [Fact]
    public void When_CircularBuffer_Contains_Element_Then_Returns_True() {
        // arrange
        var buffer = new[] { 1, 2, 3, 4, 5 };
        var circularBuffer = new CircularBuffer<int>(buffer.Length, buffer);

        // act
        var actual = circularBuffer.Contains(5);

        // assert
        Assert.True(actual);
    }

    [Fact]
    public void When_CircularBuffer_Does_Not_Contains_Element_Then_Returns_False() {
        // arrange
        var buffer = new[] { 1, 2, 3, 4, 5 };
        var circularBuffer = new CircularBuffer<int>(buffer.Length, buffer);

        // act
        var actual = circularBuffer.Contains(6);

        // assert
        Assert.False(actual);
    }

    [Fact]
    public void When_Copying_CircularBuffer_Then_Fills_Array_With_CircularBuffer_Items() {
        // arrange
        var buffer = new[] { 1, 2, 3, 4, 5 };
        var circularBuffer = new CircularBuffer<int>(buffer.Length, buffer);
        var array = new int[5];

        // act
        circularBuffer.CopyTo(array, 0);

        // assert
        Assert.Equivalent(buffer, array);
    }

    [Fact]
    public void When_Copying_CircularBuffer_Then_Fills_Array_With_CircularBuffer_Items_For_Specified_Start_Index() {
        // arrange
        var buffer = new[] { 1, 2, 3, 4, 5 };
        var circularBuffer = new CircularBuffer<int>(buffer.Length, buffer);
        var array = new int[10];
        var expected = new[] { 0, 0, 0, 1, 2, 3, 4, 5, 0, 0 };

        // act
        circularBuffer.CopyTo(array, 3);

        // assert
        Assert.Equal(expected: expected, array);
    }

    [Fact]
    public void
        When_Copying_CircularBuffer_Then_Throws_InvalidOperationException_If_Specified_Array_Does_Not_Have_Enough_Size() {
        // arrange
        var buffer = new[] { 1, 2, 3, 4, 5 };
        var circularBuffer = new CircularBuffer<int>(buffer.Length, buffer);
        var array = new int[4];

        // act & assert
        Assert.Throws<InvalidOperationException>(() => circularBuffer.CopyTo(array, 0));
    }

    [Fact]
    public void When_Getting_Array_From_CircularBuffer_Then_Returns_Array_With_CircularBuffer_Items() {
        // arrange
        var buffer = new[] { 1, 2, 3, 4, 5 };
        var circularBuffer = new CircularBuffer<int>(buffer.Length, buffer);

        // act
        var array = circularBuffer.ToArray();

        // assert
        Assert.Equal(expected: buffer, array);
    }

    [Fact]
    public void When_Getting_Array_From_CircularBuffer_Then_Returns_Different_Array_If_New_Items_Added_To_It() {
        // arrange
        var buffer = new[] { 1, 2, 3, 4, 5 };
        var circularBuffer = new CircularBuffer<int>(buffer.Length, buffer);
        var expected = new[] { 6, 7, 8, 4, 5 };

        // act
        circularBuffer.Add(6);
        circularBuffer.Add(7);
        circularBuffer.Add(8);

        var array = circularBuffer.ToArray();

        // assert
        Assert.Equal(expected: expected, array);
    }

    [Fact]
    public void When_Getting_Array_From_CircularBuffer_After_Clear_Then_Returns_Empty_Array() {
        // arrange
        var buffer = new[] { 1, 2, 3, 4, 5 };
        var circularBuffer = new CircularBuffer<int>(buffer.Length, buffer);

        // act
        circularBuffer.Clear();
        var actual = circularBuffer.ToArray();

        // assert
        Assert.Empty(actual);
    }

    [Fact]
    public void When_Enumerating_CircularBuffer_Then_Iterate_Through_Its_Items() {
        // arrange
        var buffer = new[] { 1, 2, 3, 4, 5 };
        var circularBuffer = new CircularBuffer<int>(buffer.Length, buffer);
        var actual = new List<int>();

        // act
        foreach (var element in circularBuffer) {
            actual.Add(element);
        }

        // assert
        Assert.Equivalent(buffer, actual);
    }

    [Fact]
    public void When_Retrieving_Item_From_CircularBuffer_Then_Return_Item_With_Specified_Index() {
        // arrange
        var buffer = new[] { 1, 2, 3, 4, 5 };
        var circularBuffer = new CircularBuffer<int>(buffer.Length, buffer);
        const int expected = 5;

        // act
        var actual = circularBuffer[4];

        // assert
        Assert.Equal(expected: expected, actual);
    }

    [Fact]
    public void When_Retrieving_Item_From_Empty_CircularBuffer_Then_Throws_IndexOutOfRangeException() {
        // arrange
        var circularBuffer = new CircularBuffer<int>(5);

        // act & assert
        Assert.Throws<IndexOutOfRangeException>(() => _ = circularBuffer[2]);
    }

    [Fact]
    public void
        When_Retrieving_Item_From_CircularBuffer_Then_Throws_IndexOutOfRangeException_If_Index_Is_Out_Of_Bounds() {
        // arrange
        var buffer = new[] { 1, 2, 3, 4, 5 };
        var circularBuffer = new CircularBuffer<int>(buffer.Length, buffer);

        // act & assert
        Assert.Throws<IndexOutOfRangeException>(() => _ = circularBuffer[10]);
    }
}