using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Nameless.Serialization.Binary {
    /// <summary>
    /// Binary serializer. Objects that need to be binary serialized needs the attribute <see cref="SerializableAttribute" />
    /// </summary>
    public class BinarySerializer : ISerializer {
        #region ISerializer Members

        public byte[] Serialize (object graph, SerializationOptions options = null) {
            Prevent.ParameterNull (graph, nameof (graph));

            using (var memoryStream = new MemoryStream ()) {
                var formatter = new BinaryFormatter ();
                formatter.Serialize (memoryStream, graph);
                return memoryStream.ToArray ();
            }
        }

        public object Deserialize (Type type, byte[] buffer, SerializationOptions options = null) {
            Prevent.ParameterNull (buffer, nameof (buffer));

            var stream = new MemoryStream (buffer);
            var formatter = new BinaryFormatter ();
            return formatter.Deserialize (stream);
        }

        public void Serialize (Stream stream, object graph, SerializationOptions options = null) {
            Prevent.ParameterNull (stream, nameof (stream));
            Prevent.ParameterNull (graph, nameof (graph));

            var formatter = new BinaryFormatter ();
            formatter.Serialize (stream, graph);
        }

        public object Deserialize (Type type, Stream stream, SerializationOptions options = null) {
            Prevent.ParameterNull (stream, nameof (stream));

            var formatter = new BinaryFormatter ();
            return formatter.Deserialize (stream);
        }

        #endregion
    }
}