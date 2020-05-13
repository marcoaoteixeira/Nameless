using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Nameless.Helpers {

    /// <summary>
    /// An <see cref="object"/> helper class.
    /// </summary>
    public static class ObjectHelper {
        #region Public Static Methods

        /// <summary>
        /// Converts the <paramref name="obj"/> <see cref="object"/> to a XML <see cref="string"/> representation.
        /// </summary>
        /// <param name="obj">The source <see cref="object" />.</param>
        /// <returns>A XML <see cref="string"/>.</returns>
        public static string ToXml (object obj) {
            Prevent.ParameterNull (obj, nameof (obj));

            return !obj.GetType ().IsAnonymous () ?
                ConvertComplexObjectToXml (obj) :
                ConvertAnonymousObjectToXml (obj).ToString ();
        }

        /// <summary>
        /// Converts an anonymous object into a <see cref="Dictionary{String, Object}"/>.
        /// </summary>
        /// <param name="obj">The source <see cref="object"/>.</param>
        /// <returns>A <see cref="Dictionary{String, Object}"/>.</returns>
        public static Dictionary<string, object> ToAnonymousDictionary (object obj) {
            Prevent.ParameterNull (obj, nameof (obj));

            if (!obj.GetType ().IsAnonymous ()) { return null; }

            var result = new Dictionary<string, object> ();
            foreach (var property in obj.GetType ().GetProperties ()) {
                result.Add (property.Name, property.GetValue (obj, null));
            }

            return result;
        }

        /// <summary>
        /// Converts a struct to a <see cref="Nullable{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the struct</typeparam>
        /// <param name="value">The source value.</param>
        /// <returns>A nullable value.</returns>
        public static T? AsNullable<T> (T value) where T : struct => new T?(value);

        #endregion Public Static Methods

        #region Private Static Methods

        private static string ConvertComplexObjectToXml (object input) {
            if (input == null) { return string.Empty; }

            using var memoryStream = new MemoryStream();
            using var streamReader = new StreamReader(memoryStream);
            var xmlSerializer = new XmlSerializer(input.GetType());
            xmlSerializer.Serialize(memoryStream, input);
            xmlSerializer = null;

            memoryStream.Seek(0, SeekOrigin.Begin);

            return streamReader.ReadToEnd();
        }

        private static XElement ConvertAnonymousObjectToXml (object input) {
            if (input == null) { return null; }

            return ConvertAnonymousObjectToXml (input, null);
        }

        private static XElement ConvertAnonymousObjectToXml (object input, string element) {
            if (input == null) { return null; }
            if (string.IsNullOrEmpty (element)) { element = "root"; }

            element = XmlConvert.EncodeName (element);
            var result = new XElement (element);

            var type = input.GetType ();
            var properties = type.GetProperties ();
            var elements = from property in properties
            let name = XmlConvert.EncodeName (property.Name)
            let val = property.PropertyType.IsArray ? "array" : property.GetValue (input, null)
            let value = property.PropertyType.IsArray ? GetArrayElement (property, (Array) property.GetValue (input, null)) : (property.PropertyType.IsSimple () ? new XElement (name, val) : ConvertAnonymousObjectToXml (val, name))
            where value != null
            select value;

            result.Add (elements);

            return result;
        }

        private static XElement GetArrayElement (PropertyInfo info, Array input) {
            var name = XmlConvert.EncodeName (info.Name);
            var rootElement = new XElement (name);
            var arrayCount = input.GetLength (0);

            for (var idx = 0; idx < arrayCount; idx++) {
                var value = input.GetValue (idx);
                var childElement = value.GetType ().IsSimple () ? new XElement (string.Concat (name, "Child"), value) : ConvertAnonymousObjectToXml (value);
                rootElement.Add (childElement);
            }

            return rootElement;
        }

        #endregion Private Static Methods
    }
}