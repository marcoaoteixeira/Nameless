// ReSharper disable MethodHasAsyncOverloadWithCancellation

using System.Text;
using Nameless.Infrastructure;
using static Nameless.IO.CappedReadStreamTests;

namespace Nameless.Extensions;

[UnitTest]
public class StreamExtensionsTests {
    private const string CONTENT = "Hello, olá mundo! 🌍";

    private static MemoryStream NewStream(string text = CONTENT, Encoding? encoding = null) {
        return new MemoryStream((encoding ?? new UTF8Encoding(false)).GetBytes(text));
    }

    /// <summary>Runs the sync or the async variant so each scenario is covered twice.</summary>
    private static async Task<string> ReadAsync(
        Stream stream,
        bool useAsync,
        Encoding? encoding = null,
        bool fromStart = true,
        BufferSize bufferSize = BufferSize.Tiny
    ) {
        if (useAsync) {
            return await stream.GetContentAsStringAsync(encoding, fromStart, bufferSize, TestContext.Current.CancellationToken);
        }

        return stream.GetContentAsString(encoding, fromStart, bufferSize);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadsWholeContent_WhenStreamIsAtStart(bool useAsync) {
        using var stream = NewStream();

        var result = await ReadAsync(stream, useAsync);

        Assert.Equal(CONTENT, result);
        Assert.Equal(0L, stream.Position);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadsFromStartAndRestoresPosition_WhenStreamIsMidway(bool useAsync) {
        using var stream = NewStream();
        stream.Position = 5;

        var result = await ReadAsync(stream, useAsync, fromStart: true);

        Assert.Equal(CONTENT, result);
        Assert.Equal(5L, stream.Position);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadsFromCurrentPositionAndRestoresIt_WhenFromStartIsFalse(bool useAsync) {
        using var stream = NewStream("abcdef");
        stream.Position = 2;

        var result = await ReadAsync(stream, useAsync, fromStart: false);

        Assert.Equal("cdef", result);
        Assert.Equal(2L, stream.Position);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CanBeCalledRepeatedly_WithSameResult(bool useAsync) {
        using var stream = NewStream();

        var first = await ReadAsync(stream, useAsync);
        var second = await ReadAsync(stream, useAsync);

        Assert.Equal(first, second);
        Assert.Equal(0L, stream.Position);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task LeavesStreamOpen(bool useAsync) {
        await using var stream = new TestStream(NewStream());

        _ = await ReadAsync(stream, useAsync);

        Assert.False(stream.Disposed);
        Assert.True(stream.CanRead);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadsEmptyStream(bool useAsync) {
        using var stream = new MemoryStream();

        var result = await ReadAsync(stream, useAsync);

        Assert.Equal(string.Empty, result);
        Assert.Equal(0L, stream.Position);
    }

    [Theory]
    [InlineData(false, BufferSize.Tiny)]
    [InlineData(false, BufferSize.Large)]
    [InlineData(true, BufferSize.Tiny)]
    [InlineData(true, BufferSize.Large)]
    public async Task ReadsContentLargerThanBuffer(bool useAsync, BufferSize bufferSize) {
        var text = string.Concat(Enumerable.Repeat(CONTENT, 500));
        using var stream = NewStream(text);

        var result = await ReadAsync(stream, useAsync, bufferSize: bufferSize);

        Assert.Equal(text, result);
        Assert.Equal(0L, stream.Position);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task UsesProvidedEncoding(bool useAsync) {
        var latin1 = Encoding.Latin1;
        using var stream = NewStream("olá", latin1);

        var result = await ReadAsync(stream, useAsync, encoding: latin1);

        Assert.Equal("olá", result);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ByteOrderMark_TakesPrecedenceOverProvidedEncoding_WhenFromStart(bool useAsync) {
        var utf16 = new UnicodeEncoding(bigEndian: false, byteOrderMark: true);
        var bytes = utf16.GetPreamble().Concat(utf16.GetBytes("olá")).ToArray();
        using var stream = new MemoryStream(bytes);

        // Caller says UTF-8, but the BOM says UTF-16 LE.
        var result = await ReadAsync(stream, useAsync, encoding: Encoding.UTF8, fromStart: true);

        Assert.Equal("olá", result);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Throws_WhenStreamIsNotReadable(bool useAsync) {
        await using var stream = new TestStream(NewStream(), canRead: false);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => ReadAsync(stream, useAsync));

        Assert.Equal("Can't read the stream.", ex.Message);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Throws_WhenFromStartAndStreamIsNotSeekable(bool useAsync) {
        await using var stream = new TestStream(NewStream(), canSeek: false);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => ReadAsync(stream, useAsync, fromStart: true)
        );

        Assert.Equal("Can't change stream cursor position.", ex.Message);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadsNonSeekableStream_WhenNotFromStart(bool useAsync) {
        // This is the scenario that used to throw NotSupportedException
        // because Position was read before checking CanSeek.
        await using var stream = new TestStream(NewStream(), canSeek: false);

        var result = await ReadAsync(stream, useAsync, fromStart: false);

        Assert.Equal(CONTENT, result);
        Assert.False(stream.Disposed);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Throws_WhenStreamIsNull(bool useAsync) {
        Stream stream = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(() => ReadAsync(stream, useAsync));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RestoresPosition_WhenReadFails(bool useAsync) {
        using var inner = NewStream();
        inner.Position = 3;
        await using var stream = new TestStream(inner, throwOnRead: true);

        await Assert.ThrowsAsync<IOException>(() => ReadAsync(stream, useAsync, fromStart: true));

        Assert.Equal(3L, inner.Position);
    }

    [Fact]
    public async Task Async_RestoresPosition_AndThrows_WhenAlreadyCancelled() {
        using var stream = NewStream();
        stream.Position = 4;
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => stream.GetContentAsStringAsync(cancellationToken: cts.Token)
        );

        Assert.Equal(4L, stream.Position);
    }
}

public class StreamExtensionsBytesTests {
    private static readonly byte[] Content = [.. "Hello, olá mundo! 🌍"u8];

    private static MemoryStream NewStream(byte[]? bytes = null) {
        return new MemoryStream([.. bytes ?? Content]);
    }

    /// <summary>Runs the sync or the async variant so each scenario is covered twice.</summary>
    private static async Task<byte[]> ReadAsync(
        Stream stream,
        bool useAsync,
        bool fromStart = true,
        BufferSize bufferSize = BufferSize.Tiny
    ) {
        if (useAsync) {
            return await stream.GetContentAsByteArrayAsync(fromStart, bufferSize, cancellationToken: TestContext.Current.CancellationToken);
        }

        return stream.GetContentAsByteArray(fromStart, bufferSize);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadsWholeContent_WhenStreamIsAtStart(bool useAsync) {
        using var stream = NewStream();

        var result = await ReadAsync(stream, useAsync);

        Assert.Equal(Content, result);
        Assert.Equal(0L, stream.Position);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadsFromStartAndRestoresPosition_WhenStreamIsMidway(bool useAsync) {
        using var stream = NewStream();
        stream.Position = 5;

        var result = await ReadAsync(stream, useAsync, fromStart: true);

        Assert.Equal(Content, result);
        Assert.Equal(5L, stream.Position);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadsFromCurrentPositionAndRestoresIt_WhenFromStartIsFalse(bool useAsync) {
        using var stream = NewStream("abcdef"u8.ToArray());
        stream.Position = 2;

        var result = await ReadAsync(stream, useAsync, fromStart: false);

        Assert.Equal("cdef"u8.ToArray(), result);
        Assert.Equal(2L, stream.Position);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadsNothing_WhenPositionIsAtEndAndNotFromStart(bool useAsync) {
        using var stream = NewStream();
        stream.Position = stream.Length;

        var result = await ReadAsync(stream, useAsync, fromStart: false);

        Assert.Empty(result);
        Assert.Equal(stream.Length, stream.Position);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CanBeCalledRepeatedly_WithSameResult(bool useAsync) {
        using var stream = NewStream();

        var first = await ReadAsync(stream, useAsync);
        var second = await ReadAsync(stream, useAsync);

        Assert.Equal(first, second);
        Assert.Equal(0L, stream.Position);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReturnsIndependentArray_NotBackedByTheStream(bool useAsync) {
        using var stream = NewStream();

        var result = await ReadAsync(stream, useAsync);
        result[0] = 0xFF;

        Assert.Equal(Content[0], stream.ToArray()[0]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task LeavesStreamOpen(bool useAsync) {
        await using var stream = new TestStream(NewStream());

        _ = await ReadAsync(stream, useAsync);

        Assert.False(stream.Disposed);
        Assert.True(stream.CanRead);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadsEmptyStream(bool useAsync) {
        using var stream = new MemoryStream();

        var result = await ReadAsync(stream, useAsync);

        Assert.Empty(result);
        Assert.Equal(0L, stream.Position);
    }

    [Theory]
    [InlineData(false, BufferSize.Tiny)]
    [InlineData(false, BufferSize.Large)]
    [InlineData(true, BufferSize.Tiny)]
    [InlineData(true, BufferSize.Large)]
    public async Task ReadsContentLargerThanBuffer(bool useAsync, BufferSize bufferSize) {
        var bytes = new byte[100_000];
        new Random(42).NextBytes(bytes);
        using var stream = NewStream(bytes);

        var result = await ReadAsync(stream, useAsync, bufferSize: bufferSize);

        Assert.Equal(bytes, result);
        Assert.Equal(0L, stream.Position);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PreservesBinaryContent_IncludingByteOrderMark(bool useAsync) {
        // Unlike the string version, no decoding happens: BOM and arbitrary bytes are kept as-is.
        var bytes = new byte[] { 0xEF, 0xBB, 0xBF, 0x00, 0xFF, 0xFE, 0x80 };
        using var stream = NewStream(bytes);

        var result = await ReadAsync(stream, useAsync);

        Assert.Equal(bytes, result);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Throws_WhenStreamIsNotReadable(bool useAsync) {
        await using var stream = new TestStream(NewStream(), canRead: false);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => ReadAsync(stream, useAsync));

        Assert.Equal("Can't read the stream.", ex.Message);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Throws_WhenFromStartAndStreamIsNotSeekable(bool useAsync) {
        await using var stream = new TestStream(NewStream(), canSeek: false);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => ReadAsync(stream, useAsync, fromStart: true)
        );

        Assert.Equal("Can't change stream cursor position.", ex.Message);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadsNonSeekableStream_WhenNotFromStart(bool useAsync) {
        await using var stream = new TestStream(NewStream(), canSeek: false);

        var result = await ReadAsync(stream, useAsync, fromStart: false);

        Assert.Equal(Content, result);
        Assert.False(stream.Disposed);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Throws_WhenStreamIsNull(bool useAsync) {
        Stream stream = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(() => ReadAsync(stream, useAsync));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RestoresPosition_WhenReadFails(bool useAsync) {
        using var inner = NewStream();
        inner.Position = 3;
        await using var stream = new TestStream(inner, throwOnRead: true);

        await Assert.ThrowsAsync<IOException>(() => ReadAsync(stream, useAsync, fromStart: true));

        Assert.Equal(3L, inner.Position);
    }

    [Fact]
    public async Task Async_RestoresPosition_AndThrows_WhenAlreadyCancelled() {
        using var stream = NewStream();
        stream.Position = 4;
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => stream.GetContentAsByteArrayAsync(cancellationToken: cts.Token)
        );

        Assert.Equal(4L, stream.Position);
    }
}
