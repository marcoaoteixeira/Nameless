using System.Collections;

namespace Nameless.Lucene;

/// <summary>
///     Represents a document to be indexed in Lucene.
/// </summary>
public sealed class Document : IEnumerable<Field> {
    /// <summary>
    ///     Gets the reserved name for the document ID field.
    /// </summary>
    public const string RESERVED_ID_NAME = "__document_id__";

    private readonly Dictionary<string, Field> _fields = new(StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    ///     Initializes a new instance of <see cref="Document" />
    /// </summary>
    /// <param name="id">The document ID.</param>
    public Document(string id) {
        ID = Guard.Against.NullOrWhiteSpace(id);

        _fields[RESERVED_ID_NAME] = new Field(
            RESERVED_ID_NAME,
            ID,
            IndexableType.String,
            FieldOptions.Store
        );
    }

    /// <summary>
    ///     Gets the document ID.
    /// </summary>
    public string ID { get; }

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
    ///     The current <see cref="Document" /> so other actions
    ///     can be chained.
    /// </returns>
    public Document Set(string name, bool value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Boolean);
    }

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
    ///     The current <see cref="Document" /> so other actions
    ///     can be chained.
    /// </returns>
    public Document Set(string name, string value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.String);
    }

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
    ///     The current <see cref="Document" /> so other actions
    ///     can be chained.
    /// </returns>
    public Document Set(string name, byte value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Byte);
    }

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
    ///     The current <see cref="Document" /> so other actions
    ///     can be chained.
    /// </returns>
    public Document Set(string name, short value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Short);
    }

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
    ///     The current <see cref="Document" /> so other actions
    ///     can be chained.
    /// </returns>
    public Document Set(string name, int value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Integer);
    }

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
    ///     The current <see cref="Document" /> so other actions
    ///     can be chained.
    /// </returns>
    public Document Set(string name, long value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Long);
    }

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
    ///     The current <see cref="Document" /> so other actions
    ///     can be chained.
    /// </returns>
    public Document Set(string name, float value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Float);
    }

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
    ///     The current <see cref="Document" /> so other actions
    ///     can be chained.
    /// </returns>
    public Document Set(string name, double value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Double);
    }

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
    ///     The current <see cref="Document" /> so other actions
    ///     can be chained.
    /// </returns>
    public Document Set(string name, DateTimeOffset value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.DateTimeOffset);
    }

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
    ///     The current <see cref="Document" /> so other actions
    ///     can be chained.
    /// </returns>
    public Document Set(string name, DateTime value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.DateTime);
    }

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
    ///     The current <see cref="Document" /> so other actions
    ///     can be chained.
    /// </returns>
    public Document Set(string name, DateOnly value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.DateOnly);
    }

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
    ///     The current <see cref="Document" /> so other actions
    ///     can be chained.
    /// </returns>
    public Document Set(string name, TimeOnly value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.TimeOnly);
    }

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
    ///     The current <see cref="Document" /> so other actions
    ///     can be chained.
    /// </returns>
    public Document Set(string name, TimeSpan value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.TimeSpan);
    }

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
    ///     The current <see cref="Document" /> so other actions
    ///     can be chained.
    /// </returns>
    public Document Set(string name, Enum value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Enum);
    }

    /// <inheritdoc />
    public IEnumerator<Field> GetEnumerator() {
        return _fields.Values.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    private Document Set(string name, object? value, FieldOptions options, IndexableType type) {
        if (value is null) { return this; }

        Guard.Against.NullOrWhiteSpace(name);

        if (string.Equals(name, RESERVED_ID_NAME, StringComparison.InvariantCultureIgnoreCase)) {
            throw new InvalidOperationException($"Field name '{RESERVED_ID_NAME}' is reserved.");
        }

        _fields[name] = new Field(name, value, type, options);

        return this;
    }
}