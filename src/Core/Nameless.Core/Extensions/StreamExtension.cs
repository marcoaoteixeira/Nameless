using System.Text;

namespace Nameless;

/// <summary>
/// <see cref="Stream"/> extension methods.
/// </summary>
public static class StreamExtension {
    /// <summary>
    /// Reads a stream to a string value.
    /// </summary>
    /// <param name="self">The stream</param>
    /// <param name="encoding">The encoding. Default is <see cref="Encoding.UTF8" /> without BOM</param>
    /// <param name="bufferSize">The size of the buffer. Default is <see cref="BufferSize.Tiny"/></param>
    /// <returns>The stream as a string.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// if the current stream can not be read.
    /// </exception>
    public static string ToText(this Stream self, Encoding? encoding = null, BufferSize bufferSize = BufferSize.Tiny) {
        Prevent.Argument.Null(self);

        if (!self.CanRead) {
            throw new InvalidOperationException("Stream does not support read operation.");
        }

        // Set stream cursor to the begging of the stream.
        if (self.CanSeek) {
            self.Seek(offset: 0, SeekOrigin.Begin);
        }

        using var reader = new StreamReader(stream: self,
                                            encoding: encoding ?? Defaults.Encoding,
                                            detectEncodingFromByteOrderMarks: true,
                                            bufferSize: (int)bufferSize);

        return reader.ReadToEnd();
    }

    /// <summary>
    /// Converts a <see cref="Stream"/> into a byte array. Note: It will set the cursor
    /// at the beginning of the stream.
    /// </summary>
    /// <param name="self">The source <see cref="Stream"/></param>
    /// <param name="bufferSize">The buffer size. Default is <see cref="BufferSize.Tiny"/></param>
    /// <returns>A byte array representing the <see cref="Stream"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// if the current stream can not be read.
    /// </exception>
    public static byte[] ToByteArray(this Stream self, BufferSize bufferSize = BufferSize.Tiny) {
        Prevent.Argument.Null(self);

        if (!self.CanRead) {
            throw new InvalidOperationException("Can't read stream.");
        }

        if (!self.CanSeek) {
            throw new InvalidOperationException("Can't execute stream seek.");
        }

        self.Seek(offset: 0, origin: SeekOrigin.Begin);

        // Return, but faster...
        if (self is MemoryStream ms) { return ms.ToArray(); }

        var allocationSize = (int)bufferSize;
        using var memoryStream = new MemoryStream();
        var buffer = new byte[allocationSize];
        int count;
        while ((count = self.Read(buffer, offset: 0, count: allocationSize)) > 0) {
            memoryStream.Write(buffer, offset: 0, count: count);
        }

        return memoryStream.ToArray();
    }
}