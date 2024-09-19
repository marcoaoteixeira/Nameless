using System.Text;
using Moq;

namespace Nameless;

public class StreamExtensionTests {
    [Test]
    public void ToText_Should_Read_And_Return_Text_From_Stream() {
        // arrange
        var expected = "This is a test";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(expected));

        // act
        var actual = StreamExtension.ToText(stream);

        // assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ToText_Should_Throw_InvalidOperationException_If_Can_Not_Read_Stream() {
        // arrange
        var streamMock = new Mock<Stream>();
        streamMock
            .Setup(mock => mock.CanRead)
            .Returns(false);

        // act

        // assert
        Assert.Throws<InvalidOperationException>(
            code: () => StreamExtension.ToText(streamMock.Object)
        );
    }

    [Test]
    public void ToByteArray_Should_Return_Byte_Array_From_Stream() {
        // arrange
        var expected = Encoding.UTF8.GetBytes("This is a test");
        using var stream = new MemoryStream(expected);

        // act
        var actual = StreamExtension.ToByteArray(stream);

        // assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ToByteArray_Should_Throw_InvalidOperationException_If_Can_Not_Read_Stream() {
        // arrange
        var streamMock = new Mock<Stream>();
        streamMock
            .Setup(mock => mock.CanRead)
            .Returns(false);

        // act

        // assert
        Assert.Throws<InvalidOperationException>(
            code: () => StreamExtension.ToByteArray(streamMock.Object)
        );
    }

    [Test]
    public void ToByteArray_Should_Return_Byte_Array_From_FileStream() {
        // arrange
        var assemblyPath = Path.GetDirectoryName(GetType().Assembly.Location)!;
        var filePath = Path.Combine(assemblyPath, "Content", "LoremIpsun.txt");
        var expected = File.ReadAllBytes(filePath);

        using var stream = File.OpenRead(filePath);

        // act
        var actual = StreamExtension.ToByteArray(stream);

        // assert
        Assert.That(actual, Is.EquivalentTo(expected));
    }
}