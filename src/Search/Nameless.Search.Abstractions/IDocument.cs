namespace Nameless.Search;

/// <summary>
/// Contract to a document.
/// </summary>
public interface IDocument : IEnumerable<Field> {
    /// <summary>
    /// Gets the document ID.
    /// </summary>
    string ID { get; }

    /// <summary>
    /// Adds a new <see cref="bool"/> value to the document.
    /// </summary>
    /// <param name="field">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="options">The field options.</param>
    /// <returns>An <see cref="IDocument"/> object.</returns>
    IDocument Set(string field, bool value, FieldOptions options);

    /// <summary>
    /// Adds a new <see cref="string"/> value to the document.
    /// </summary>
    /// <param name="field">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="options">The field options.</param>
    /// <returns>An <see cref="IDocument"/> object.</returns>
    IDocument Set(string field, string value, FieldOptions options);

    /// <summary>
    /// Adds a new <see cref="byte"/> value to the document.
    /// </summary>
    /// <param name="field">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="options">The field options.</param>
    /// <returns>An <see cref="IDocument"/> object.</returns>
    IDocument Set(string field, byte value, FieldOptions options);

    /// <summary>
    /// Adds a new <see cref="short"/> value to the document.
    /// </summary>
    /// <param name="field">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="options">The field options.</param>
    /// <returns>An <see cref="IDocument"/> object.</returns>
    IDocument Set(string field, short value, FieldOptions options);

    /// <summary>
    /// Adds a new <see cref="int"/> value to the document.
    /// </summary>
    /// <param name="field">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="options">The field options.</param>
    /// <returns>An <see cref="IDocument"/> object.</returns>
    IDocument Set(string field, int value, FieldOptions options);

    /// <summary>
    /// Adds a new <see cref="long"/> value to the document.
    /// </summary>
    /// <param name="field">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="options">The field options.</param>
    /// <returns>An <see cref="IDocument"/> object.</returns>
    IDocument Set(string field, long value, FieldOptions options);

    /// <summary>
    /// Adds a new <see cref="float"/> value to the document.
    /// </summary>
    /// <param name="field">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="options">The field options.</param>
    /// <returns>An <see cref="IDocument"/> object.</returns>
    IDocument Set(string field, float value, FieldOptions options);

    /// <summary>
    /// Adds a new <see cref="double"/> value to the document.
    /// </summary>
    /// <param name="field">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="options">The field options.</param>
    /// <returns>An <see cref="IDocument"/> object.</returns>
    IDocument Set(string field, double value, FieldOptions options);

    /// <summary>
    /// Adds a new <see cref="DateTimeOffset"/> value to the document.
    /// </summary>
    /// <param name="field">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="options">The field options.</param>
    /// <returns>An <see cref="IDocument"/> object.</returns>
    IDocument Set(string field, DateTimeOffset value, FieldOptions options);

    /// <summary>
    /// Adds a new <see cref="DateTime"/> value to the document.
    /// </summary>
    /// <param name="field">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="options">The field options.</param>
    /// <returns>An <see cref="IDocument"/> object.</returns>
    IDocument Set(string field, DateTime value, FieldOptions options);
}