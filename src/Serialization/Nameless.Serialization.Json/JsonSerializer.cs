using Newtonsoft.Json;

namespace Nameless.Serialization.Json {
    public class JsonSerializer : ISerializer {
        #region ISerializer Members

        public byte[] Serialize(object? graph, SerializationOptions? options = null) {
            if (graph == null) { return Array.Empty<byte>(); }

            var opts = options as JsonSerializationOptions ?? JsonSerializationOptions.Default;
            var json = JsonConvert.SerializeObject(graph, opts.Settings);
            return opts.Encoding.GetBytes(json);
        }

        public object? Deserialize(Type? type, byte[]? buffer, SerializationOptions? options = null) {
            Garda.Prevent.Null(type, nameof(type));

            if (buffer == null) { return null; }

            var opts = options as JsonSerializationOptions ?? JsonSerializationOptions.Default;
            var json = opts.Encoding.GetString(buffer);
            return JsonConvert.DeserializeObject(json, type, opts.Settings);
        }

        public void Serialize(Stream? stream, object? graph, SerializationOptions? options = null) {
            Garda.Prevent.Null(stream, nameof(stream));

            if (!stream.CanWrite) { throw new InvalidOperationException("Cannot write to the stream"); }

            var buffer = Serialize(graph, options);
            stream.Write(buffer, offset: 0, count: buffer.Length);
        }

        public object? Deserialize(Type? type, Stream? stream, SerializationOptions? options = null) {
            Garda.Prevent.Null(type, nameof(type));

            if (stream == null) { return null; }

            if (!stream.CanRead) { throw new InvalidOperationException("Cannot read the stream"); }

            var buffer = stream.ToByteArray();
            return Deserialize(type, buffer, options);
        }

        #endregion
    }
}