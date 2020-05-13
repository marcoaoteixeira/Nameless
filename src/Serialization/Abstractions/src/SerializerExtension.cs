using System.IO;

namespace Nameless.Serialization {
    public static class SerializerExtension {
        #region Public Static Methods

        public static T Deserialize<T> (this ISerializer self, byte[] buffer, SerializationOptions options = null) {
            if (self == null) { return default; }

            Prevent.ParameterNull (buffer, nameof (buffer));

            return (T) self.Deserialize (typeof (T), buffer, options);
        }

        public static T Deserialize<T> (this ISerializer self, Stream stream, SerializationOptions options = null) {
            if (self == null) { return default; }

            Prevent.ParameterNull (stream, nameof (stream));

            return (T) self.Deserialize (typeof (T), stream, options);
        }

        #endregion
    }
}