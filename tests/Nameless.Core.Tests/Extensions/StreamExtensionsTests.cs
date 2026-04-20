using System.Text;

namespace Nameless;

public class StreamExtensionsTests {
    // ─── GetContentAsString ─────────────────────────────────────────────────

    [Fact]
    public void GetContentAsString_WithUtf8Content_ReturnsString() {
        // arrange
        var content = "Hello, World!";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // act
        var result = stream.GetContentAsString();

        // assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void GetContentAsString_FromMiddleOfStream_FromStartTrue_ReadsFromBeginning() {
        // arrange
        var content = "Hello";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        stream.Position = 3; // advance position

        // act
        var result = stream.GetContentAsString(fromStart: true);

        // assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void GetContentAsString_WithNonReadableStream_ThrowsInvalidOperationException() {
        // arrange
        using var stream = new NonReadableStream();

        // act & assert
        Assert.Throws<InvalidOperationException>(() => stream.GetContentAsString());
    }

    // ─── GetContentAsByteArray ──────────────────────────────────────────────

    [Fact]
    public void GetContentAsByteArray_WithMemoryStream_ReturnsBytes() {
        // arrange
        var bytes = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(bytes);

        // act
        var result = stream.GetContentAsByteArray();

        // assert
        Assert.Equal(bytes, result);
    }

    [Fact]
    public void GetContentAsByteArray_FromMiddleOfStream_FromStartTrue_ReadsFromBeginning() {
        // arrange
        var bytes = new byte[] { 1, 2, 3 };
        using var stream = new MemoryStream(bytes);
        stream.Position = 2;

        // act
        var result = stream.GetContentAsByteArray(fromStart: true);

        // assert
        Assert.Equal(bytes, result);
    }

    [Fact]
    public void GetContentAsByteArray_WithNonReadableStream_ThrowsInvalidOperationException() {
        // arrange
        using var stream = new NonReadableStream();

        // act & assert
        Assert.Throws<InvalidOperationException>(() => stream.GetContentAsByteArray());
    }

    [Fact]
    public void GetContentAsString_WithNonSeekableReadableStream_AndFromStartTrue_ThrowsInvalidOperationException() {
        // arrange
        using var stream = new ReadableNonSeekableStream(new byte[] { 65, 66, 67 });

        // act & assert
        Assert.Throws<InvalidOperationException>(() => stream.GetContentAsString(fromStart: true));
    }

    [Fact]
    public void GetContentAsString_WithNonSeekableReadableStream_AndFromStartFalse_ReturnsContent() {
        // arrange
        var bytes = Encoding.UTF8.GetBytes("ABC");
        using var stream = new ReadableNonSeekableStream(bytes);

        // act
        var result = stream.GetContentAsString(fromStart: false);

        // assert
        Assert.Equal("ABC", result);
    }

    [Fact]
    public void GetContentAsByteArray_WithNonSeekableReadableStream_AndFromStartTrue_ThrowsInvalidOperationException() {
        // arrange
        using var stream = new ReadableNonSeekableStream(new byte[] { 1, 2, 3 });

        // act & assert
        Assert.Throws<InvalidOperationException>(() => stream.GetContentAsByteArray(fromStart: true));
    }

    [Fact]
    public void GetContentAsByteArray_WithNonSeekableReadableStream_AndFromStartFalse_ReturnsBytes() {
        // arrange
        var bytes = new byte[] { 10, 20, 30 };
        using var stream = new ReadableNonSeekableStream(bytes);

        // act
        var result = stream.GetContentAsByteArray(fromStart: false);

        // assert
        Assert.Equal(bytes, result);
    }

    [Fact]
    public void GetContentAsByteArray_WithSeekableNonMemoryStream_FromStartTrue_ReadsFromBeginning() {
        // arrange — must NOT be a MemoryStream to bypass the fast-path branch
        var bytes = new byte[] { 7, 8, 9 };
        using var stream = new ReadableSeekableStream(bytes);
        stream.Position = 2;

        // act
        var result = stream.GetContentAsByteArray(fromStart: true);

        // assert
        Assert.Equal(bytes, result);
    }

    [Fact]
    public void GetContentAsByteArray_WithSeekableNonMemoryStream_RestoresPosition() {
        // arrange — covers the self.Position = previousPosition restore path (line 96)
        var bytes = new byte[] { 1, 2, 3 };
        using var stream = new ReadableSeekableStream(bytes);
        stream.Position = 1;

        // act
        stream.GetContentAsByteArray(fromStart: false);

        // assert — position should be restored to 1 after reading
        Assert.Equal(1, stream.Position);
    }

    // ─── test doubles ─────────────────────────────────────────────────────────

    private sealed class NonReadableStream : Stream {
        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => 0;
        public override long Position { get => 0; set { } }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => 0;
        public override long Seek(long offset, SeekOrigin origin) => 0;
        public override void SetLength(long value) { }
        public override void Write(byte[] buffer, int offset, int count) { }
    }

    private sealed class ReadableNonSeekableStream(byte[] data) : Stream {
        private int _position;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => data.Length;
        public override long Position { get => _position; set => throw new NotSupportedException(); }

        public override int Read(byte[] buffer, int offset, int count) {
            var available = data.Length - _position;
            var toRead = Math.Min(count, available);
            Array.Copy(data, _position, buffer, offset, toRead);
            _position += toRead;
            return toRead;
        }

        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    private sealed class ReadableSeekableStream(byte[] data) : Stream {
        private long _position;

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => data.Length;
        public override long Position { get => _position; set => _position = value; }

        public override int Read(byte[] buffer, int offset, int count) {
            var available = (int)(data.Length - _position);
            var toRead = Math.Min(count, available);
            Array.Copy(data, (int)_position, buffer, offset, toRead);
            _position += toRead;
            return toRead;
        }

        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) {
            _position = origin switch {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => _position + offset,
                SeekOrigin.End => data.Length + offset,
                _ => throw new NotSupportedException()
            };
            return _position;
        }
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
