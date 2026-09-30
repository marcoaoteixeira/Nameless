namespace Nameless.IO;

/// <summary>
///     A read-only wrapper that lets at most <see cref="Limit"/> bytes
///     through from the wrapped stream. Reading exactly
///     <see cref="Limit"/> bytes is fine; as soon as the wrapped stream
///     turns out to hold more than that,
///     <see cref="MaximumReadLimitExceededException"/> is thrown.
/// </summary>
/// <remarks>
///     <para>
///         Typical use is protecting against oversized input (request
///         bodies, uploads, decompression bombs) without having to trust
///         <c>Length</c> or a <c>Content-Length</c> header: pass the
///         capped stream to a <see cref="StreamReader"/>, <c>CopyTo</c>,
///         a deserializer, etc.
///     </para>
///     <para>
///         The stream never hands out bytes beyond the limit. To detect
///         an overflow inside the same call that crosses the limit, it
///         asks the wrapped stream for one byte more than it may return;
///         that extra byte is consumed from the wrapped stream and discarded.
///         After the exception the stream is considered faulted: every
///         further read throws the same exception.
///     </para>
///     <para>
///         The stream is forward-only (<see cref="CanSeek"/> is
///         <see langword="false"/>), so <c>fromStart: true</c> on the
///         <c>GetContentAs*</c> extensions is not applicable; use
///         <c>fromStart: false</c>. Not thread-safe.
///     </para>
/// </remarks>
public sealed class CappedReadStream : Stream {
    private readonly Stream _inner;
    private readonly bool _leaveOpen;
    private bool _disposed;
    private bool _exceeded;

    /// <summary>
    ///     Gets the maximum number of bytes that may be read.
    /// </summary>
    public long Limit { get; }

    /// <summary>
    ///     Gets how many bytes have been returned to callers so far.
    /// </summary>
    public long TotalBytesRead { get; private set; }

    /// <summary>
    ///     Gets how many more bytes may be read before the limit is exceeded.
    /// </summary>
    public long Remaining => Limit - TotalBytesRead;

    /// <inhertidoc />
    public override bool CanRead => !_disposed && _inner.CanRead;

    /// <inhertidoc />
    public override bool CanSeek => false;

    /// <inhertidoc />
    public override bool CanWrite => false;

    /// <inhertidoc />
    public override long Length => throw new NotSupportedException();

    /// <inhertidoc />
    public override long Position {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="CappedReadStream" />
    ///     class.
    /// </summary>
    /// <param name="inner">
    ///     The stream to read from.
    /// </param>
    /// <param name="limit">
    ///     Maximum number of bytes that may be read. Zero is allowed.
    /// </param>
    /// <param name="leaveOpen">
    ///     <see langword="true"/> to keep <paramref name="inner"/> open
    ///     when this stream is disposed.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="inner"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="limit"/> is negative.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <paramref name="inner"/> is not readable.
    /// </exception>
    public CappedReadStream(Stream inner, long limit, bool leaveOpen = false) {
        Throws.When.Null(inner);
        Throws.When.LowerThan(limit, compare: 0);

        if (!inner.CanRead) {
            throw new ArgumentException("The stream must be readable.", nameof(inner));
        }

        _inner = inner;
        _leaveOpen = leaveOpen;

        Limit = limit;
    }

    /// <inhertidoc />
    public override int Read(byte[] buffer, int offset, int count) {
        ValidateBufferArguments(buffer, offset, count);

        return Read(buffer.AsSpan(offset, count));
    }

    /// <inhertidoc />
    public override int Read(Span<byte> buffer) {
        var allowed = ValidateReadRequest(buffer.Length);
        var read = _inner.Read(buffer[..allowed]);

        return IncrementTotalBytesRead(read);
    }

    /// <inhertidoc />
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
        ValidateBufferArguments(buffer, offset, count);

        var memory = buffer.AsMemory(offset, count);

        return ReadAsync(memory, cancellationToken).AsTask();
    }

    /// <inhertidoc />
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) {
        var allowed = ValidateReadRequest(buffer.Length);

        var read = await _inner.ReadAsync(buffer[..allowed], cancellationToken)
                               .ConfigureAwait(false);

        return IncrementTotalBytesRead(read);
    }

    /// <inhertidoc />
    public override int ReadByte() {
        Span<byte> single = stackalloc byte[1];

        return Read(single) == 0 ? -1 : single[0];
    }

    /// <inhertidoc />
    public override void Flush() { }

    /// <inhertidoc />
    public override long Seek(long offset, SeekOrigin origin) {
        throw new NotSupportedException();
    }

    /// <inhertidoc />
    public override void SetLength(long value) {
        throw new NotSupportedException();
    }

    /// <inhertidoc />
    public override void Write(byte[] buffer, int offset, int count) {
        throw new NotSupportedException();
    }

    /// <inhertidoc />
    protected override void Dispose(bool disposing) {
        if (disposing && !_disposed) {
            _disposed = true;

            if (!_leaveOpen) {
                _inner.Dispose();
            }
        }

        base.Dispose(disposing);
    }

    /// <inhertidoc />
    public override async ValueTask DisposeAsync() {
        if (!_disposed) {
            _disposed = true;

            if (!_leaveOpen) {
                await _inner.DisposeAsync()
                            .ConfigureAwait(false);
            }
        }

        await base.DisposeAsync()
                  .ConfigureAwait(false);
    }

    // Returns how many bytes to request from the inner stream: the caller's
    // count, or, when the limit would be crossed, one byte more than may be
    // returned so an overflow shows up right here.
    private int ValidateReadRequest(int requested) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_exceeded) {
            throw new MaximumReadLimitExceededException(Limit);
        }

        var remaining = Remaining;

        // remaining < requested (an int) guarantees remaining + 1
        // fits in an int.
        return remaining < requested ? (int)remaining + 1 : requested;
    }

    private int IncrementTotalBytesRead(int read) {
        if (read > Remaining) {
            _exceeded = true;

            throw new MaximumReadLimitExceededException(Limit);
        }

        TotalBytesRead += read;

        return read;
    }
}