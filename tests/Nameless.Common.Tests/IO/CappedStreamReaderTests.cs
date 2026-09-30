// ReSharper disable MethodHasAsyncOverload
using System.Text;

namespace Nameless.IO;

public class CappedReadStreamTests {
    private static byte[] Data(int length) {
        var bytes = new byte[length];
        new Random(7).NextBytes(bytes);

        return bytes;
    }

    private static CappedReadStream Cap(byte[] data, long limit, bool leaveOpen = true) {
        return new CappedReadStream(new MemoryStream(data), limit, leaveOpen);
    }

    private static async Task<int> ReadOnceAsync(Stream stream, byte[] buffer, bool useAsync) {
        return useAsync
            ? await stream.ReadAsync(buffer.AsMemory())
            : stream.Read(buffer, 0, buffer.Length);
    }

    /// <summary>Reads until EOF (or until the exception) collecting whatever was returned.</summary>
    private static async Task<(byte[] Bytes, Exception? Error)> DrainAsync(Stream stream, int chunk, bool useAsync) {
        using var collected = new MemoryStream();
        var buffer = new byte[chunk];

        try {
            int read;
            while ((read = await ReadOnceAsync(stream, buffer, useAsync)) > 0) {
                collected.Write(buffer, 0, read);
            }
        }
        catch (Exception ex) {
            return (collected.ToArray(), ex);
        }

        return (collected.ToArray(), null);
    }

    /// <summary>Inner stream that returns at most one byte per call, like a slow network stream.</summary>
    private sealed class TrickleStream(byte[] data) : MemoryStream(data) {
        public override int Read(byte[] buffer, int offset, int count) {
            return base.Read(buffer, offset, Math.Min(count, 1));
        }

        public override int Read(Span<byte> buffer) {
            return base.Read(buffer[..Math.Min(buffer.Length, 1)]);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) {
            return base.ReadAsync(buffer[..Math.Min(buffer.Length, 1)], cancellationToken);
        }
    }

    // ---- limit semantics -------------------------------------------------------------------

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ContentBelowLimit_ReadsEverything(bool useAsync) {
        var data = Data(50);
        await using var stream = Cap(data, 100);

        var (bytes, error) = await DrainAsync(stream, chunk: 16, useAsync);

        Assert.Null(error);
        Assert.Equal(data, bytes);
    }

    [Theory]
    [InlineData(false, 16)]
    [InlineData(false, 100)]
    [InlineData(true, 16)]
    [InlineData(true, 100)]
    public async Task ContentEqualToLimit_ReadsEverything_AndSignalsEndOfStream(bool useAsync, int chunk) {
        var data = Data(100);
        await using var stream = Cap(data, 100);

        var (bytes, error) = await DrainAsync(stream, chunk, useAsync);

        Assert.Null(error);
        Assert.Equal(data, bytes);
        Assert.Equal(0, stream.Remaining);
    }

    [Theory]
    [InlineData(false, 16)]
    [InlineData(false, 1000)]
    [InlineData(true, 16)]
    [InlineData(true, 1000)]
    public async Task ContentAboveLimit_Throws_AndNeverReturnsMoreThanLimit(bool useAsync, int chunk) {
        var data = Data(101);
        await using var stream = Cap(data, 100);

        var (bytes, error) = await DrainAsync(stream, chunk, useAsync);

        var ex = Assert.IsType<MaximumReadLimitExceededException>(error);
        Assert.Equal(100L, ex.Limit);
        Assert.True(bytes.Length <= 100);
        Assert.Equal(data.Take(bytes.Length), bytes);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task OverflowIsDetectedInTheCallThatCrossesTheLimit(bool useAsync) {
        await using var stream = Cap(Data(11), 10);
        var buffer = new byte[100];

        await Assert.ThrowsAsync<MaximumReadLimitExceededException>(() => ReadOnceAsync(stream, buffer, useAsync));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task LargeBufferOnExactContent_ReturnsAllThenEndOfStream(bool useAsync) {
        await using var stream = Cap(Data(10), 10);
        var buffer = new byte[100];

        Assert.Equal(10, await ReadOnceAsync(stream, buffer, useAsync));
        Assert.Equal(0, await ReadOnceAsync(stream, buffer, useAsync));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ZeroLimit_EmptyContent_IsFine(bool useAsync) {
        await using var stream = Cap([], 0);

        var (bytes, error) = await DrainAsync(stream, 8, useAsync);

        Assert.Null(error);
        Assert.Empty(bytes);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ZeroLimit_AnyContent_Throws(bool useAsync) {
        await using var stream = Cap(Data(1), 0);

        var (bytes, error) = await DrainAsync(stream, 8, useAsync);

        Assert.IsType<MaximumReadLimitExceededException>(error);
        Assert.Empty(bytes);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ZeroLengthRead_NeverThrows_EvenAtTheLimit(bool useAsync) {
        await using var stream = Cap(Data(100), 10);
        var buffer = new byte[10];
        Assert.Equal(10, await ReadOnceAsync(stream, buffer, useAsync));

        var read = await ReadOnceAsync(stream, [], useAsync);

        Assert.Equal(0, read);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WorksWithInnerStreamThatReturnsOneByteAtATime(bool useAsync) {
        var data = Data(20);

        await using (var ok = new CappedReadStream(new TrickleStream(data), 20)) {
            var (bytes, error) = await DrainAsync(ok, 8, useAsync);

            Assert.Null(error);
            Assert.Equal(data, bytes);
        }

        await using var tooSmall = new CappedReadStream(new TrickleStream(data), 19);
        var (partial, overflow) = await DrainAsync(tooSmall, 8, useAsync);

        Assert.IsType<MaximumReadLimitExceededException>(overflow);
        Assert.Equal(19, partial.Length);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task AfterTheLimitIsExceeded_EveryFurtherReadThrows(bool useAsync) {
        await using var stream = Cap(Data(50), 10);
        var buffer = new byte[100];
        await Assert.ThrowsAsync<MaximumReadLimitExceededException>(() => ReadOnceAsync(stream, buffer, useAsync));

        await Assert.ThrowsAsync<MaximumReadLimitExceededException>(() => ReadOnceAsync(stream, buffer, useAsync));
    }

    [Fact]
    public async Task BytesReadAndRemaining_TrackProgress() {
        await using var stream = Cap(Data(100), 40);
        var buffer = new byte[15];

        Assert.Equal(0, stream.TotalBytesRead);
        Assert.Equal(40, stream.Remaining);

        _ = await stream.ReadAsync(buffer, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(15, stream.TotalBytesRead);
        Assert.Equal(25, stream.Remaining);
        Assert.Equal(40, stream.Limit);
    }

    [Fact]
    public void LimitCanBeLongMaxValue() {
        var data = Data(10);
        using var stream = Cap(data, long.MaxValue);

        var buffer = new byte[100];

        Assert.Equal(10, stream.Read(buffer, 0, buffer.Length));
        Assert.Equal(0, stream.Read(buffer, 0, buffer.Length));
    }

    [Fact]
    public void ReadByte_ReturnsBytesThenMinusOne_AndThrowsOnOverflow() {
        using var exact = Cap([1, 2], 2);
        Assert.Equal(1, exact.ReadByte());
        Assert.Equal(2, exact.ReadByte());
        Assert.Equal(-1, exact.ReadByte());

        using var tooBig = Cap([1, 2, 3], 2);
        Assert.Equal(1, tooBig.ReadByte());
        Assert.Equal(2, tooBig.ReadByte());
        Assert.Throws<MaximumReadLimitExceededException>(() => tooBig.ReadByte());
    }

    // ---- integration with the framework and the other extensions ---------------------------

    [Fact]
    public void CopyTo_Throws_WhenContentExceedsLimit() {
        using var stream = Cap(Data(1000), 100);
        using var destination = new MemoryStream();

        Assert.Throws<MaximumReadLimitExceededException>(() => stream.CopyTo(destination));
        Assert.True(destination.Length <= 100);
    }

    [Fact]
    public async Task CopyToAsync_Throws_WhenContentExceedsLimit() {
        await using var stream = Cap(Data(1000), 100);
        using var destination = new MemoryStream();

        await Assert.ThrowsAsync<MaximumReadLimitExceededException>(() => stream.CopyToAsync(destination, cancellationToken: TestContext.Current.CancellationToken));
        Assert.True(destination.Length <= 100);
    }

    [Fact]
    public async Task StreamReader_ReadToEnd_ThrowsOnlyWhenContentExceedsLimit() {
        var text = new string('a', 100);

        await using (var ok = new CappedReadStream(new MemoryStream(Encoding.UTF8.GetBytes(text)), 100)) {
            using var reader = new StreamReader(ok);
            Assert.Equal(text, await reader.ReadToEndAsync(cancellationToken: TestContext.Current.CancellationToken));
        }

        await using var tooBig = new CappedReadStream(new MemoryStream(Encoding.UTF8.GetBytes(text + "a")), 100);
        using var reader2 = new StreamReader(tooBig);

        await Assert.ThrowsAsync<MaximumReadLimitExceededException>(reader2.ReadToEndAsync);
    }

    [Fact]
    public void GetContentAsString_WorksWithFromStartFalse_AndThrowsOnOverflow() {
        using var ok = new CappedReadStream(new MemoryStream([.. "hello"u8]), 5);
        Assert.Equal("hello", ok.GetContentAsString(fromStart: false));

        using var tooBig = new CappedReadStream(new MemoryStream([.. "hello!"u8]), 5);
        Assert.Throws<MaximumReadLimitExceededException>(() => tooBig.GetContentAsString(fromStart: false));
    }

    [Fact]
    public void GetContentAsString_FromStart_IsRejected_BecauseTheStreamIsForwardOnly() {
        using var stream = Cap([.. "hello"u8], 5);

        Assert.Throws<InvalidOperationException>(() => stream.GetContentAsString());
    }

    [Fact]
    public async Task GetContentAsBytesAsync_WorksWithFromStartFalse_AndThrowsOnOverflow() {
        var data = Data(64);

        await using var ok = Cap(data, 64);
        Assert.Equal(data, await ok.GetContentAsByteArrayAsync(fromStart: false, cancellationToken: TestContext.Current.CancellationToken));

        await using var tooBig = Cap(Data(65), 64);
        await Assert.ThrowsAsync<MaximumReadLimitExceededException>(() => tooBig.GetContentAsByteArrayAsync(fromStart: false, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public void WithReadLimit_CreatesCappedStream() {
        using var inner = new MemoryStream(Data(10));

        using var capped = inner.WithReadLimit(5, leaveOpen: true);

        Assert.Equal(5, capped.Limit);
        Assert.Throws<MaximumReadLimitExceededException>(() => capped.CopyTo(Stream.Null));
    }

    // ---- disposal --------------------------------------------------------------------------

    [Fact]
    public void Dispose_ClosesInnerStream_ByDefault() {
        var inner = new TestStream(new MemoryStream(Data(4)));

        new CappedReadStream(inner, 10).Dispose();

        Assert.True(inner.Disposed);
    }

    [Fact]
    public void Dispose_LeavesInnerStreamOpen_WhenRequested() {
        var inner = new TestStream(new MemoryStream(Data(4)));

        new CappedReadStream(inner, 10, leaveOpen: true).Dispose();

        Assert.False(inner.Disposed);
    }

    [Fact]
    public async Task DisposeAsync_HonoursLeaveOpen() {
        var closed = new TestStream(new MemoryStream(Data(4)));
        var open = new TestStream(new MemoryStream(Data(4)));

        await new CappedReadStream(closed, 10).DisposeAsync();
        await new CappedReadStream(open, 10, leaveOpen: true).DisposeAsync();

        Assert.True(closed.Disposed);
        Assert.False(open.Disposed);
    }

    [Fact]
    public void Read_AfterDispose_Throws() {
        var stream = Cap(Data(4), 10);
        stream.Dispose();

        Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
        Assert.False(stream.CanRead);
    }

    // ---- capabilities and argument validation ----------------------------------------------

    [Fact]
    public void IsAForwardOnlyReadOnlyStream() {
        using var stream = Cap(Data(4), 10);

        Assert.True(stream.CanRead);
        Assert.False(stream.CanSeek);
        Assert.False(stream.CanWrite);
        Assert.Throws<NotSupportedException>(() => stream.Length);
        Assert.Throws<NotSupportedException>(() => stream.Position);
        Assert.Throws<NotSupportedException>(() => stream.Position = 0);
        Assert.Throws<NotSupportedException>(() => stream.Seek(0, SeekOrigin.Begin));
        Assert.Throws<NotSupportedException>(() => stream.SetLength(0));
        Assert.Throws<NotSupportedException>(() => stream.Write(new byte[1], 0, 1));
    }

    [Fact]
    public void Constructor_ValidatesArguments() {
        Assert.Throws<ArgumentNullException>(() => new CappedReadStream(null!, 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CappedReadStream(new MemoryStream(), -1));
        Assert.Throws<ArgumentException>(() => new CappedReadStream(new TestStream(new MemoryStream(), canRead: false), 10));
    }

    [Fact]
    public void Read_ValidatesBufferArguments() {
        using var stream = Cap(Data(4), 10);

        Assert.Throws<ArgumentNullException>(() => stream.Read(null!, 0, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => stream.Read(new byte[2], 1, 5));
    }

    [Fact]
    public async Task ReadAsync_PropagatesCancellation() {
        await using var stream = Cap(Data(4), 10);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => _ = await stream.ReadAsync(new byte[4].AsMemory(), cts.Token)
        );
    }

    [Fact]
    public void Exception_ExposesLimitAndHasDefaultMessage() {
        var ex = new MaximumReadLimitExceededException(42);

        Assert.Equal(42, ex.Limit);
        Assert.Contains("42", ex.Message);
        Assert.IsAssignableFrom<IOException>(ex);
    }

    /// <summary>
    /// Configurable test double around another stream, used to simulate
    /// non-readable, non-seekable and failing streams.
    /// </summary>
    internal sealed class TestStream(
        Stream inner,
        bool canRead = true,
        bool canSeek = true,
        bool throwOnRead = false
    ) : Stream {
        public bool Disposed { get; private set; }

        public override bool CanRead => canRead && inner.CanRead;
        public override bool CanSeek => canSeek && inner.CanSeek;
        public override bool CanWrite => false;

        public override long Length => CanSeek ? inner.Length : throw new NotSupportedException();

        public override long Position {
            get => CanSeek ? inner.Position : throw new NotSupportedException();
            set {
                if (!CanSeek) {
                    throw new NotSupportedException();
                }

                inner.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count) {
            return throwOnRead ? throw new IOException("Simulated read failure.") : inner.Read(buffer, offset, count);
        }

        public override int Read(Span<byte> buffer) {
            return throwOnRead ? throw new IOException("Simulated read failure.") : inner.Read(buffer);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) {
            return throwOnRead
                ? throw new IOException("Simulated read failure.")
                : inner.ReadAsync(buffer, cancellationToken);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
            return throwOnRead
                ? throw new IOException("Simulated read failure.")
                : inner.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override void Flush() { }

        public override long Seek(long offset, SeekOrigin origin) {
            return CanSeek ? inner.Seek(offset, origin) : throw new NotSupportedException();
        }

        public override void SetLength(long value) {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count) {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing) {
            Disposed = true;
            base.Dispose(disposing);
        }
    }
}
