using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Nameless.Helpers;

/// <summary>
/// An <see cref="object"/> helper class.
/// </summary>
public static class ObjectHelper {
    /// <summary>
    /// Converts the <paramref name="obj"/> to XML <see cref="string"/> representation.
    /// </summary>
    /// <param name="obj">The source <see cref="object" />.</param>
    /// <returns>A XML <see cref="string"/>.</returns>
    public static string ToXml(object obj) {
        Prevent.Argument.Null(obj);

        return !obj.IsAnonymous()
            ? ConvertComplexObjectToXml(obj)
            : ConvertAnonymousObjectToXml(obj).ToString();
    }

    /// <summary>
    /// Converts a struct to a <see cref="Nullable{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the struct</typeparam>
    /// <param name="value">The source value.</param>
    /// <returns>A nullable value.</returns>
    public static T? AsNullable<T>(T value)
        where T : struct
        => value;

    private static string ConvertComplexObjectToXml(object input) {
        using var memoryStream = new MemoryStream();
        using var streamReader = new StreamReader(memoryStream);
        var xmlSerializer = new XmlSerializer(input.GetType());
        xmlSerializer.Serialize(memoryStream, input);

        memoryStream.Seek(0, SeekOrigin.Begin);

        return streamReader.ReadToEnd();
    }

    private static XElement ConvertAnonymousObjectToXml(object input)
        => ConvertAnonymousObjectToXml(input, null);

    private static XElement ConvertAnonymousObjectToXml(object input, string? element) {
        if (string.IsNullOrEmpty(element)) { element = "root"; }

        element = XmlConvert.EncodeName(element);
        var result = new XElement(element);

        var type = input.GetType();
        var properties = type.GetProperties();
        var elements = from property in properties
                       let name = XmlConvert.EncodeName(property.Name)
                       let val = !property.PropertyType.IsArray
                           ? property.GetValue(input, null)
                           : "array"
                       let value = property.PropertyType.IsArray
                           ? GetArrayElement(property, property.GetValue(input, null) as Array)
                           : (property.PropertyType.IsSimple()
                               ? new XElement(name, val)
                               : ConvertAnonymousObjectToXml(val, name))
                       where value is not null
                       select value;

        result.Add(elements);

        return result;
    }

    private static XElement? GetArrayElement(MemberInfo info, Array? input) {
        if (input is null) { return null; }

        var name = XmlConvert.EncodeName(info.Name);
        var rootElement = new XElement(name);
        var arrayCount = input.GetLength(0);

        for (var idx = 0; idx < arrayCount; idx++) {
            var value = input.GetValue(idx);
            if (value is null) { continue; }

            var childElement = value.GetType()
                                    .IsSimple()
                ? new XElement(string.Concat(name, "Child"), value)
                : ConvertAnonymousObjectToXml(value);

            rootElement.Add(childElement);
        }

        return rootElement;
    }
}