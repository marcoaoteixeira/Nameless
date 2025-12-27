using System.Text;

namespace Nameless;

/// <summary>
///     <see cref="Stream" /> extension methods.
/// </summary>
public static class StreamExtensions {
    /// <param name="self">The stream</param>
    extension(Stream self) {
        /// <summary>
        ///     Reads a stream to a string value.
        /// </summary>
        /// <param name="encoding">The encoding. Default is <see cref="Encoding.UTF8" /> without BOM</param>
        /// <param name="fromStart">Whether it should read from the start of the stream. Default is <see langword="true"/>.</param>
        /// <param name="bufferSize">The size of the buffer. Default is <see cref="BufferSize.Tiny" /></param>
        /// <returns>The stream as a string.</returns>
        /// <exception cref="InvalidOperationException">
        ///     if the current stream can not be read.
        /// </exception>
        public string GetContentAsString(Encoding? encoding = null, bool fromStart = true,
            BufferSize bufferSize = BufferSize.Tiny) {
            if (!self.CanRead) {
                throw new InvalidOperationException(message: "Can't read the stream.");
            }

            if (fromStart && !self.CanSeek) {
                throw new InvalidOperationException(message: "Can't change stream cursor position.");
            }

            // let's record the current position for this stream
            var previousPosition = self.Position;

            // if we want it from start, it must be able to seek
            if (fromStart && self.CanSeek) {
                self.Position = 0L; // Set the stream cursor at the beginning.
            }

            var result = new StreamReader(
                self,
                encoding ?? Defaults.Encoding,
                detectEncodingFromByteOrderMarks: true,
                (int)bufferSize
            ).ReadToEnd();

            // again, if we can seek let's put the cursor in the
            // original position
            if (self.CanSeek) {
                self.Position = previousPosition; // Set the stream cursor at its original position.
            }

            return result;
        }

        /// <summary>
        ///     Converts a <see cref="Stream" /> into a byte array. Note: It will set the cursor
        ///     at the beginning of the stream.
        /// </summary>
        /// <param name="fromStart">Whether it should read from the start of the stream. Default is <see langword="true"/>.</param>
        /// <param name="bufferSize">The buffer size. Default is <see cref="BufferSize.Tiny" /></param>
        /// <returns>A byte array representing the <see cref="Stream" />.</returns>
        /// <exception cref="InvalidOperationException">
        ///     if the current stream can not be read.
        /// </exception>
        public byte[] GetContentAsByteArray(bool fromStart = true, BufferSize bufferSize = BufferSize.Tiny) {
            // Return, but faster...
            if (self is MemoryStream ms) { return ms.ToArray(); }

            if (!self.CanRead) {
                throw new InvalidOperationException(message: "Can't read the stream.");
            }

            if (fromStart && !self.CanSeek) {
                throw new InvalidOperationException(message: "Can't change stream cursor position.");
            }

            // let's record the current position for this stream
            var previousPosition = self.Position;

            // if we want it from start, it must be able to seek
            if (fromStart && self.CanSeek) {
                self.Position = 0L; // Set the stream cursor at the beginning.
            }

            var allocationSize = (int)bufferSize;
            using var memoryStream = new MemoryStream();
            var buffer = new byte[allocationSize];
            int count;
            while ((count = self.Read(buffer, offset: 0, allocationSize)) > 0) {
                memoryStream.Write(buffer, offset: 0, count);
            }

            // again, if we can seek let's put the cursor in the
            // original position
            if (self.CanSeek) {
                self.Position = previousPosition; // Set the stream cursor at its original position.
            }

            return memoryStream.ToArray();
        }
    }
}