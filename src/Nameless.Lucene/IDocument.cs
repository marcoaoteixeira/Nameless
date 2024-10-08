﻿namespace Nameless.Lucene;

/// <summary>
/// Defines methods for a document index.
/// </summary>
public interface IDocument : IEnumerable<Field> {
    string ID { get; }

    /// <summary>
    /// Adds a new <see cref="string"/> value to the document.
    /// </summary>
    /// <param name="field">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="options">The field options.</param>
    /// <returns>The current instance of <see cref="IDocument"/>.</returns>
    IDocument Set(string field, string value, FieldOptions options);

    /// <summary>
    /// Adds a new <see cref="DateTimeOffset"/> value to the document.
    /// </summary>
    /// <param name="field">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="options">The field options.</param>
    /// <returns>The current instance of <see cref="IDocument"/>.</returns>
    IDocument Set(string field, DateTimeOffset value, FieldOptions options);

    /// <summary>
    /// Adds a new <see cref="int"/> value to the document.
    /// </summary>
    /// <param name="field">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="options">The field options.</param>
    /// <returns>The current instance of <see cref="IDocument"/>.</returns>
    IDocument Set(string field, int value, FieldOptions options);

    /// <summary>
    /// Adds a new <see cref="bool"/> value to the document.
    /// </summary>
    /// <param name="field">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="options">The field options.</param>
    /// <returns>The current instance of <see cref="IDocument"/>.</returns>
    IDocument Set(string field, bool value, FieldOptions options);

    /// <summary>
    /// Adds a new <see cref="double"/> value to the document.
    /// </summary>
    /// <param name="field">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="options">The field options.</param>
    /// <returns>The current instance of <see cref="IDocument"/>.</returns>
    IDocument Set(string field, double value, FieldOptions options);
}