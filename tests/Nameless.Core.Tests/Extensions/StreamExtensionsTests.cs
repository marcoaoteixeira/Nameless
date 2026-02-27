using System.Text;
using Nameless.Testing.Tools.Mockers.IO;
using Nameless.Testing.Tools.Resources;

namespace Nameless;

public class StreamExtensionsTests {
    [Fact]
    public void
        WhenGettingContentAsString_WhenStreamCanRead_WhenStreamCanSeek_WhenStreamHasContent_ThenReturnsStreamContentAsString() {
        // arrange
        const string Expected = "This is a test";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(Expected));

        // act
        var actual = stream.GetContentAsString();

        // assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void WhenGettingContentAsString_WhenStreamCantRead_ThenThrowsInvalidOperationException() {
        // arrange
        var stream = new StreamMocker().WithCanRead(returnValue: false).Build();

        // act

        // assert
        Assert.Throws<InvalidOperationException>(() => stream.GetContentAsString());
    }

    [Fact]
    public void WhenGettingContentAsString_WhenReadFromStart_WhenStreamCantSeek_ThenThrowsInvalidOperationException() {
        // arrange
        var stream = new StreamMocker()
                     .WithCanRead()
                     .WithCanSeek(returnValue: false)
                     .Build();

        // act & assert
        Assert.Throws<InvalidOperationException>(() => stream.GetContentAsString(fromStart: true));
    }

    [Fact]
    public void WhenGettingContentAsString_WhenStreamCanSeek_WhenFromStartIsTrue_ThenReadsFromTheStreamBeginning() {
        // arrange
        const string Expected = "This is a test";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(Expected));

        var originalPosition = stream.Seek(offset: 10, SeekOrigin.Begin);

        // act
        var notFromStart = stream.GetContentAsString(fromStart: false);
        var fromStart = stream.GetContentAsString(fromStart: true);

        var finalPosition = stream.Position;

        // assert
        Assert.Multiple(() => {
            Assert.Equal(expected: "test", notFromStart);
            Assert.Equal(Expected, fromStart);
            Assert.Equal(originalPosition, finalPosition);
        });
    }

    [Fact]
    public void WhenGettingContentAsByteArray_WhenStreamIsMemoryStream_ThenReturnsStreamAsByteArray() {
        // arrange
        var expected = "This is a test"u8.ToArray();
        using var stream = new MemoryStream(expected);

        // act
        var actual = stream.GetContentAsByteArray();

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void WhenGettingContentAsByteArray_WhenStreamIsNotMemoryStream_WhenCanRead_ThenReturnsStreamAsByteArray() {
        // arrange
        using var resource = ResourcesHelper.GetResource("ThisIsATest.txt");
        var expected = "This Is A Test"u8.ToArray();

        // act
        var actual = resource.Open().GetContentAsByteArray();

        // assert
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void WhenGettingContentAsByteArray_WhenStreamCantRead_ThenThrowsInvalidOperationException() {
        // arrange
        var stream = new StreamMocker().WithCanRead(returnValue: false).Build();

        // act

        // assert
        Assert.Throws<InvalidOperationException>(() => stream.GetContentAsByteArray());
    }

    [Fact]
    public void
        WhenGettingContentAsByteArray_WhenReadFromStart_WhenStreamCantSeek_ThenThrowsInvalidOperationException() {
        // arrange
        var stream = new StreamMocker()
                     .WithCanRead()
                     .WithCanSeek(returnValue: false)
                     .Build();

        // act & assert
        Assert.Throws<InvalidOperationException>(() => stream.GetContentAsByteArray(fromStart: true));
    }
}