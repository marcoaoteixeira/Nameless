using System.Text;
using Moq;

namespace Nameless;

public class StreamExtensionsTests {
    [Fact]
    public void ToText_Should_Read_And_Return_Text_From_Stream() {
        // arrange
        var expected = "This is a test";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(expected));

        // act
        var actual = stream.ToText();

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToText_Should_Throw_InvalidOperationException_If_Can_Not_Read_Stream() {
        // arrange
        var streamMock = new Mock<Stream>();
        streamMock
           .Setup(mock => mock.CanRead)
           .Returns(false);

        // act

        // assert
        Assert.Throws<InvalidOperationException>(
            () => streamMock.Object.ToText()
        );
    }

    [Fact]
    public void ToByteArray_Should_Return_Byte_Array_From_Stream() {
        // arrange
        var expected = Encoding.UTF8.GetBytes("This is a test");
        using var stream = new MemoryStream(expected);

        // act
        var actual = stream.ToByteArray();

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToByteArray_Should_Throw_InvalidOperationException_If_Can_Not_Read_Stream() {
        // arrange
        var streamMock = new Mock<Stream>();
        streamMock
           .Setup(mock => mock.CanRead)
           .Returns(false);

        // act

        // assert
        Assert.Throws<InvalidOperationException>(
            () => streamMock.Object.ToByteArray()
        );
    }

    [Fact]
    public void ToByteArray_Should_Return_Byte_Array_From_FileStream() {
        // arrange
        var assemblyPath = Path.GetDirectoryName(GetType().Assembly.Location)!;
        var filePath = Path.Combine(assemblyPath, "Content", "LoremIpsun.txt");
        var expected = File.ReadAllBytes(filePath);

        using var stream = File.OpenRead(filePath);

        // act
        var actual = stream.ToByteArray();

        // assert
        Assert.Equivalent(expected, actual);
    }
}