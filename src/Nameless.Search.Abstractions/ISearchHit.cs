namespace Nameless.Search;

/// <summary>
///     Defines methods for search hit.
/// </summary>
public interface ISearchHit {
    /// <summary>
    ///     Gets the document item ID.
    /// </summary>
    string DocumentID { get; }

    /// <summary>
    ///     Gets the score.
    /// </summary>
    float Score { get; }

    /// <summary>
    ///     Retrieves a <see cref="bool" /> value.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>The <see cref="bool" /> value.</returns>
    bool? GetBoolean(string fieldName);

    /// <summary>
    ///     Retrieves a <see cref="string" /> value.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>The <see cref="string" /> value.</returns>
    string? GetString(string fieldName);

    /// <summary>
    ///     Retrieves a <see cref="byte" /> value.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>The <see cref="byte" /> value.</returns>
    byte? GetByte(string fieldName);

    /// <summary>
    ///     Retrieves a <see cref="short" /> value.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>The <see cref="short" /> value.</returns>
    short? GetShort(string fieldName);

    /// <summary>
    ///     Retrieves the <see cref="int" /> value.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>The <see cref="int" /> value.</returns>
    int? GetInteger(string fieldName);

    /// <summary>
    ///     Retrieves a <see cref="long" /> value.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>The <see cref="long" /> value.</returns>
    long? GetLong(string fieldName);

    /// <summary>
    ///     Retrieves a <see cref="float" /> value.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>The <see cref="float" /> value.</returns>
    float? GetFloat(string fieldName);

    /// <summary>
    ///     Retrieves the <see cref="double" /> value.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>The <see cref="double" /> value.</returns>
    double? GetDouble(string fieldName);

    /// <summary>
    ///     Retrieves the <see cref="DateTime" /> value.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>The <see cref="DateTime" /> value.</returns>
    DateTimeOffset? GetDateTimeOffset(string fieldName);

    /// <summary>
    ///     Retrieves a <see cref="DateTime" /> value.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>The <see cref="DateTime" /> value.</returns>
    DateTime? GetDateTime(string fieldName);
}