using System;
using System.Collections.Generic;
using System.IO;
using MS_XmlSerializer = System.Xml.Serialization.XmlSerializer;
using MS_XmlSerializerNamespaces = System.Xml.Serialization.XmlSerializerNamespaces;

namespace Nameless.Serialization.Xml {
    public sealed class XmlSerializer : ISerializer {
        #region Private Static Methods

        private static void Serialize (Stream stream, object graph, IDictionary<string, string> namespaces = null) {
            Prevent.ParameterNull (stream, nameof (stream));
            Prevent.ParameterNull (graph, nameof (graph));

            var xmlSerializer = new MS_XmlSerializer (graph.GetType ());
            var xmlSerializerNamespaces = new MS_XmlSerializerNamespaces ();
            if (namespaces != null && namespaces.Count > 0) {
                foreach (var kvp in namespaces) {
                    xmlSerializerNamespaces.Add (kvp.Key, kvp.Value);
                }
            } else { xmlSerializerNamespaces.Add (string.Empty, string.Empty); }

            xmlSerializer.Serialize (stream, graph, xmlSerializerNamespaces);
        }

        #endregion

        #region ISerializer Members

        public byte[] Serialize (object graph, SerializationOptions options = null) {
            using (var memoryStream = new MemoryStream ()) {
                var namespaces = options is XmlSerializationOptions opts ? opts.Namespaces : null;

                Serialize (memoryStream, graph, namespaces);

                memoryStream.Seek (offset: 0, loc: SeekOrigin.Begin);
                return memoryStream.ToArray ();
            }
        }
        public object Deserialize (Type type, byte[] buffer, SerializationOptions options = null) {
            using (var memoryStream = new MemoryStream (buffer)) {
                return Deserialize (type, memoryStream);
            }
        }

        public void Serialize (Stream stream, object graph, SerializationOptions options = null) {
            var namespaces = options is XmlSerializationOptions opts ? opts.Namespaces : null;

            Serialize (stream, graph, namespaces);
        }

        public object Deserialize (Type type, Stream stream, SerializationOptions options = null) {
            return new MS_XmlSerializer (type).Deserialize (stream);
        }

        #endregion
    }
}