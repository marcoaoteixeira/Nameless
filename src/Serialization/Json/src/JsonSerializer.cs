using System;
using System.IO;
using Newtonsoft.Json;

namespace Nameless.Serialization.Json {
    public class JsonSerializer : ISerializer {

        #region ISerializer Members

        public byte[] Serialize (object graph, SerializationOptions options = null) {
            Prevent.ParameterNull (graph, nameof (graph));

            var opts = options as JsonSerializationOptions ?? JsonSerializationOptions.Default;
            var json = JsonConvert.SerializeObject (graph, opts.Settings);
            return opts.Encoding.GetBytes (json);
        }

        public object Deserialize (Type type, byte[] buffer, SerializationOptions options = null) {
            Prevent.ParameterNull (type, nameof (type));
            Prevent.ParameterNull (buffer, nameof (buffer));

            var opts = options as JsonSerializationOptions ?? JsonSerializationOptions.Default;
            var json = opts.Encoding.GetString (buffer);
            return JsonConvert.DeserializeObject (json, type, opts.Settings);
        }

        public void Serialize (Stream stream, object graph, SerializationOptions options = null) {
            Prevent.ParameterNull (stream, nameof (stream));
            Prevent.ParameterNull (graph, nameof (graph));

            if (!stream.CanWrite) { throw new InvalidOperationException ("Cannot write to the stream"); }

            var buffer = Serialize (graph, options);
            stream.Write (buffer, offset : 0, count : buffer.Length);
        }

        public object Deserialize (Type type, Stream stream, SerializationOptions options = null) {
            Prevent.ParameterNull (type, nameof (type));
            Prevent.ParameterNull (stream, nameof (stream));

            if (!stream.CanRead) { throw new InvalidOperationException ("Cannot read the stream"); }

            var buffer = stream.ToByteArray ();
            return Deserialize (type, buffer, options);
        }

        #endregion
    }
}