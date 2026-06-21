namespace Nameless.Xml;

/// <summary>
///     Defines methods for validate XML using XML schema.
/// </summary>
public interface IXmlSchemaValidator {
    /// <summary>
    ///     Validates the XML <see cref="Stream" />.
    /// </summary>
    /// <param name="xml">The XML <see cref="Stream" />.</param>
    /// <param name="schema">The XML schema <see cref="Stream" />.</param>
    /// <returns><see langword="true"/> if is valid; otherwise, <see langword="false"/>.</returns>
    bool Validate(Stream xml, Stream schema);
}