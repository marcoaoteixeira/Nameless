using System.Text;
using Nameless.Infrastructure;
using Nameless.IO;

namespace Nameless;

/// <summary>
///     <see cref="Stream" /> extension methods.
/// </summary>
public static class StreamExtensions {
    /// <param name="self">
    ///     The current <see cref="Stream"/> instance.
    /// </param>
    extension(Stream self) {
        /// <summary>
        ///     Reads the stream content as a string and, when the stream is
        ///     seekable, restores the cursor to where it was before the call.
        ///     The stream is left open.
        /// </summary>
        /// <param name="encoding">
        ///     Encoding used to decode the content. When
        ///     <paramref name="fromStart"/> is <see langword="true"/>, a byte
        ///     order mark found at the beginning of the stream takes
        ///     precedence over this value.
        /// </param>
        /// <param name="fromStart">
        ///     <see langword="true"/> to read from position 0 (requires a
        ///     seekable stream); <see langword="false"/> to read from the
        ///     current position.
        /// </param>
        /// <param name="bufferSize">
        ///     Buffer size used by the internal reader.
        /// </param>
        /// <returns>
        ///     The decoded content.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="self"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     The stream is not readable, or <paramref name="fromStart"/> is
        ///     set on a non-seekable stream.
        /// </exception>
        /// <remarks>
        ///     For a non-seekable stream (only allowed with
        ///     <paramref name="fromStart"/> = <see langword="false"/>) the
        ///     content is consumed and the position cannot be restored. The
        ///     method is not thread-safe: concurrent use of the same stream
        ///     corrupts its position.
        /// </remarks>
        public string GetContentAsString(Encoding? encoding = null, bool fromStart = true, BufferSize bufferSize = BufferSize.Tiny) {
            return InnerGetContentAs(
                self,
                fromStart,
                stream => {
                    using var reader = CreateStreamReader(
                        stream,
                        encoding,
                        fromStart,
                        bufferSize
                    );

                    return reader.ReadToEnd();
                }
            );
        }

        /// <summary>
        ///     Reads the stream content as a byte array and, when the stream
        ///     is seekable, restores the cursor to where it was before the
        ///     call. The stream is left open.
        /// </summary>
        /// <param name="fromStart">
        ///     <see langword="true"/> to read from position 0 (requires a
        ///     seekable stream); <see langword="false"/> to read from the
        ///     current position.
        /// </param>
        /// <param name="bufferSize">
        ///     Buffer size used by the internal reader.
        /// </param>
        /// <returns>
        ///     A new array holding the bytes read. Never
        ///     <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="self"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     The stream is not readable, or <paramref name="fromStart"/> is
        ///     set on a non-seekable stream.
        /// </exception>
        /// <remarks>
        ///     The whole content is buffered in memory, so avoid this on very
        ///     large streams. For a non-seekable stream (only allowed with
        ///     <paramref name="fromStart"/> = <see langword="false"/>) the
        ///     content is consumed and the position cannot be restored. The
        ///     method is not thread-safe: concurrent use of the same stream
        ///     corrupts its position.
        /// </remarks>
        public byte[] GetContentAsByteArray(bool fromStart = true, BufferSize bufferSize = BufferSize.Tiny) {
            return InnerGetContentAs(
                self,
                fromStart,
                stream => {
                    using var buffer = CreateMemoryStream(stream);

                    stream.CopyTo(buffer, (int)bufferSize);

                    return buffer.ToArray();
                }
            );
        }

        /// <summary>
        ///     Reads the stream content as a string and, when the stream is
        ///     seekable, restores the cursor to where it was before the call.
        ///     The stream is left open.
        /// </summary>
        /// <param name="encoding">
        ///     Encoding used to decode the content. When
        ///     <paramref name="fromStart"/> is <see langword="true"/>, a byte
        ///     order mark found at the beginning of the stream takes
        ///     precedence over this value.
        /// </param>
        /// <param name="fromStart">
        ///     <see langword="true"/> to read from position 0 (requires a
        ///     seekable stream); <see langword="false"/> to read from the
        ///     current position.
        /// </param>
        /// <param name="bufferSize">
        ///     Buffer size used by the internal reader.
        /// </param>
        /// <param name="cancellationToken">
        ///     Token used to cancel the read.
        /// </param>
        /// <returns>
        ///     A <see cref="Task{TResult}"/> where the result is the decoded content.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="self"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     The stream is not readable, or <paramref name="fromStart"/> is
        ///     set on a non-seekable stream.
        /// </exception>
        /// <remarks>
        ///     For a non-seekable stream (only allowed with
        ///     <paramref name="fromStart"/> = <see langword="false"/>) the
        ///     content is consumed and the position cannot be restored. The
        ///     method is not thread-safe: concurrent use of the same stream
        ///     corrupts its position.
        /// </remarks>
        public Task<string> GetContentAsStringAsync(Encoding? encoding = null, bool fromStart = true, BufferSize bufferSize = BufferSize.Tiny, CancellationToken cancellationToken = default) {
            return InnerGetContentAsAsync<string>(
                self,
                fromStart,
                async stream => {
                    using var reader = CreateStreamReader(
                        stream,
                        encoding,
                        fromStart,
                        bufferSize
                    );

                    return await reader.ReadToEndAsync(cancellationToken)
                                       .SkipContextSync();
                }
            );
        }

        /// <summary>
        ///     Reads the stream content as a byte array and, when the stream
        ///     is seekable, restores the cursor to where it was before the
        ///     call. The stream is left open.
        /// </summary>
        /// <param name="fromStart">
        ///     <see langword="true"/> to read from position 0 (requires a
        ///     seekable stream); <see langword="false"/> to read from the
        ///     current position.
        /// </param>
        /// <param name="bufferSize">
        ///     Buffer size used by the internal reader.
        /// </param>
        /// <param name="cancellationToken">
        ///     Token used to cancel the read.
        /// </param>
        /// <returns>
        ///     A new array holding the bytes read. Never
        ///     <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="self"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     The stream is not readable, or <paramref name="fromStart"/> is
        ///     set on a non-seekable stream.
        /// </exception>
        /// <remarks>
        ///     The whole content is buffered in memory, so avoid this on very
        ///     large streams. For a non-seekable stream (only allowed with
        ///     <paramref name="fromStart"/> = <see langword="false"/>) the
        ///     content is consumed and the position cannot be restored. The
        ///     method is not thread-safe: concurrent use of the same stream
        ///     corrupts its position.
        /// </remarks>
        public Task<byte[]> GetContentAsByteArrayAsync(bool fromStart = true, BufferSize bufferSize = BufferSize.Tiny, CancellationToken cancellationToken = default) {
            return InnerGetContentAsAsync(
                self,
                fromStart,
                async stream => {
                    await using var buffer = CreateMemoryStream(stream);

                    await stream.CopyToAsync(buffer, (int)bufferSize, cancellationToken)
                                .SkipContextSync();

                    return buffer.ToArray();
                }
            );
        }

        /// <summary>
        ///     Wraps the stream so that reading more than
        ///     <paramref name="limit"/> bytes throws
        ///     <see cref="MaximumReadLimitExceededException"/>.
        ///     See <see cref="CappedReadStream"/>.
        /// </summary>
        /// <param name="limit">
        ///     Maximum number of bytes that may be read.
        /// </param>
        /// <param name="leaveOpen">
        ///     <see langword="true"/> to keep <paramref name="self"/> open
        ///     when the returned stream is disposed.
        /// </param>
        /// <returns>
        ///     The current <see cref="Stream"/> wrapped in a
        ///     <see cref="CappedReadStream"/>.
        /// </returns>
        public CappedReadStream WithReadLimit(long limit, bool leaveOpen = false) {
            return new CappedReadStream(self, limit, leaveOpen);
        }
    }

    // Everything that is not "how the content is read" lives here: argument validation,
    // remembering the cursor, moving it to the start, and putting it back no matter what
    // happens. The public methods only supply the read delegate.

    private static T InnerGetContentAs<T>(Stream stream, bool fromStart, Func<Stream, T> action) {
        Validate(stream, fromStart);

        // Only seekable streams can be restored afterward.
        long? previousPosition = stream.CanSeek
            ? stream.Position
            : null;

        try {
            if (fromStart) {
                stream.Position = 0L;
            }

            return action(stream);
        }
        finally { Restore(stream, previousPosition); }
    }

    private static async Task<T> InnerGetContentAsAsync<T>(Stream stream, bool fromStart, Func<Stream, Task<T>> action) {
        Validate(stream, fromStart);

        // Only seekable streams can be restored afterward.
        long? previousPosition = stream.CanSeek
            ? stream.Position
            : null;

        try {
            if (fromStart) {
                stream.Position = 0L;
            }

            return await action(stream).SkipContextSync();
        }
        finally { Restore(stream, previousPosition); }
    }

    private static void Validate(Stream stream, bool fromStart) {
        Throws.When.Null(stream);

        if (!stream.CanRead) {
            throw new InvalidOperationException(
                "Can't read the stream."
            );
        }

        if (fromStart && !stream.CanSeek) {
            throw new InvalidOperationException(
                "Can't change stream cursor position."
            );
        }
    }

    private static StreamReader CreateStreamReader(Stream stream, Encoding? encoding, bool fromStart, BufferSize bufferSize) {
        return new StreamReader(
            stream,
            encoding ?? Defaults.Encoding,

            // BOM detection only makes sense at the beginning of the stream.
            detectEncodingFromByteOrderMarks: fromStart,

            bufferSize: (int)bufferSize,
            leaveOpen: true
        );
    }

    private static void Restore(Stream stream, long? previousPosition) {
        if (previousPosition is { } position) {
            stream.Position = position;
        }
    }

    // Pre-sizes the buffer when the remaining length is known, avoiding repeated growth.
    // Must be called after the cursor has been positioned.
    private static MemoryStream CreateMemoryStream(Stream stream) {
        if (!stream.CanSeek) {
            return new MemoryStream();
        }

        var remaining = stream.Length - stream.Position;

        if (remaining > 0 && remaining <= Array.MaxLength) {
            return new MemoryStream(capacity: (int)remaining);
        }

        return new MemoryStream();
    }
}