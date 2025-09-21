namespace Nameless.Lucene;

/// <summary>
///     Defines a document.
/// </summary>
public interface IDocument : IEnumerable<Field> {
    /// <summary>
    ///     Gets the document ID.
    /// </summary>
    string ID { get; }

    /// <summary>
    ///     Adds a new <see cref="bool" /> field to the document.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <returns>
    ///     The current <see cref="IDocument" /> so other actions
    ///     can be chained.
    /// </returns>
    IDocument Set(string name, bool value, FieldOptions options);

    /// <summary>
    ///     Adds a new <see cref="string" /> field to the document.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <returns>
    ///     The current <see cref="IDocument" /> so other actions
    ///     can be chained.
    /// </returns>
    IDocument Set(string name, string value, FieldOptions options);

    /// <summary>
    ///     Adds a new <see cref="byte" /> field to the document.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <returns>
    ///     The current <see cref="IDocument" /> so other actions
    ///     can be chained.
    /// </returns>
    IDocument Set(string name, byte value, FieldOptions options);

    /// <summary>
    ///     Adds a new <see cref="short" /> field to the document.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <returns>
    ///     The current <see cref="IDocument" /> so other actions
    ///     can be chained.
    /// </returns>
    IDocument Set(string name, short value, FieldOptions options);

    /// <summary>
    ///     Adds a new <see cref="int" /> field to the document.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <returns>
    ///     The current <see cref="IDocument" /> so other actions
    ///     can be chained.
    /// </returns>
    IDocument Set(string name, int value, FieldOptions options);

    /// <summary>
    ///     Adds a new <see cref="long" /> field to the document.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <returns>
    ///     The current <see cref="IDocument" /> so other actions
    ///     can be chained.
    /// </returns>
    IDocument Set(string name, long value, FieldOptions options);

    /// <summary>
    ///     Adds a new <see cref="float" /> field to the document.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <returns>
    ///     The current <see cref="IDocument" /> so other actions
    ///     can be chained.
    /// </returns>
    IDocument Set(string name, float value, FieldOptions options);

    /// <summary>
    ///     Adds a new <see cref="double" /> field to the document.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <returns>
    ///     The current <see cref="IDocument" /> so other actions
    ///     can be chained.
    /// </returns>
    IDocument Set(string name, double value, FieldOptions options);

    /// <summary>
    ///     Adds a new <see cref="DateTimeOffset" /> field to the document.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <returns>
    ///     The current <see cref="IDocument" /> so other actions
    ///     can be chained.
    /// </returns>
    IDocument Set(string name, DateTimeOffset value, FieldOptions options);

    /// <summary>
    ///     Adds a new <see cref="DateTime" /> field to the document.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <returns>
    ///     The current <see cref="IDocument" /> so other actions
    ///     can be chained.
    /// </returns>
    IDocument Set(string name, DateTime value, FieldOptions options);

    /// <summary>
    ///     Adds a new <see cref="DateOnly" /> field to the document.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <returns>
    ///     The current <see cref="IDocument" /> so other actions
    ///     can be chained.
    /// </returns>
    IDocument Set(string name, DateOnly value, FieldOptions options);

    /// <summary>
    ///     Adds a new <see cref="TimeOnly" /> field to the document.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <returns>
    ///     The current <see cref="IDocument" /> so other actions
    ///     can be chained.
    /// </returns>
    IDocument Set(string name, TimeOnly value, FieldOptions options);

    /// <summary>
    ///     Adds a new <see cref="TimeSpan" /> field to the document.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <returns>
    ///     The current <see cref="IDocument" /> so other actions
    ///     can be chained.
    /// </returns>
    IDocument Set(string name, TimeSpan value, FieldOptions options);

    /// <summary>
    ///     Adds a new <see cref="Enum" /> field to the document.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <returns>
    ///     The current <see cref="IDocument" /> so other actions
    ///     can be chained.
    /// </returns>
    IDocument Set(string name, Enum value, FieldOptions options);
}