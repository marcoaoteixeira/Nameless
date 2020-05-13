using System.IO;
using System.Text;

namespace Nameless {
    public static class StreamExtension {
        #region Public Static Methods

        /// <summary>
        /// Tries to read a stream to a string value.
        /// </summary>
        /// <param name="self">The stream</param>
        /// <param name="encoding">The encoding. Default is <see cref="Encoding.ASCII" /></param>
        /// <returns>The stream as a string.</returns>
        public static string ToText (this Stream self, Encoding encoding = null) {
            if (self == null) { return null; };

            var buffer = ToByteArray (self);

            return (encoding ?? Encoding.ASCII).GetString (buffer);
        }

        public static byte[] ToByteArray (this Stream self, int bufferSize = 1024 * 4 /* 4Kb */ ) {
            if (self == null) { return null; }
            if (bufferSize <= 0) { bufferSize = 1024 * 4; }
            if (!self.CanRead) { return null; }

            // Return faster...
            if (self is MemoryStream ms) { return ms.ToArray (); }

            var buffer = new byte[bufferSize];
            using var memoryStream = new MemoryStream();
            var count = 0;
            while ((count = self.Read(buffer, offset: 0, count: bufferSize)) > 0) {
                memoryStream.Write(buffer, offset: 0, count: count);
            }
            return memoryStream.ToArray();
        }

        #endregion
    }
}