namespace Nameless.Lucene;

/// <summary>
///     Defines a search hit object.
/// </summary>
public interface ISearchHit {
    /// <summary>
    ///     Gets the document ID.
    /// </summary>
    string DocumentID { get; }

    /// <summary>
    ///     Gets the score.
    /// </summary>
    float Score { get; }

    /// <summary>
    ///     Retrieves a <see cref="bool" /> value.
    /// </summary>
    /// <param name="fieldName">
    ///     The name of the field.
    /// </param>
    /// <returns>
    ///     The <see cref="bool" /> value.
    /// </returns>
    bool? GetBoolean(string fieldName);

    /// <summary>
    ///     Retrieves a <see cref="string" /> value.
    /// </summary>
    /// <param name="fieldName">
    ///     The name of the field.
    /// </param>
    /// <returns>
    ///     The <see cref="string" /> value.
    /// </returns>
    string? GetString(string fieldName);

    /// <summary>
    ///     Retrieves a <see cref="byte" /> value.
    /// </summary>
    /// <param name="fieldName">
    ///     The name of the field.
    /// </param>
    /// <returns>
    ///     The <see cref="byte" /> value.
    /// </returns>
    byte? GetByte(string fieldName);

    /// <summary>
    ///     Retrieves a <see cref="short" /> value.
    /// </summary>
    /// <param name="fieldName">
    ///     The name of the field.
    /// </param>
    /// <returns>
    ///     The <see cref="short" /> value.
    /// </returns>
    short? GetShort(string fieldName);

    /// <summary>
    ///     Retrieves the <see cref="int" /> value.
    /// </summary>
    /// <param name="fieldName">
    ///     The name of the field.
    /// </param>
    /// <returns>
    ///     The <see cref="int" /> value.
    /// </returns>
    int? GetInteger(string fieldName);

    /// <summary>
    ///     Retrieves a <see cref="long" /> value.
    /// </summary>
    /// <param name="fieldName">
    ///     The name of the field.
    /// </param>
    /// <returns>
    ///     The <see cref="long" /> value.
    /// </returns>
    long? GetLong(string fieldName);

    /// <summary>
    ///     Retrieves a <see cref="float" /> value.
    /// </summary>
    /// <param name="fieldName">
    ///     The name of the field.
    /// </param>
    /// <returns>
    ///     The <see cref="float" /> value.
    /// </returns>
    float? GetFloat(string fieldName);

    /// <summary>
    ///     Retrieves the <see cref="double" /> value.
    /// </summary>
    /// <param name="fieldName">
    ///     The name of the field.
    /// </param>
    /// <returns>
    ///     The <see cref="double" /> value.
    /// </returns>
    double? GetDouble(string fieldName);

    /// <summary>
    ///     Retrieves the <see cref="DateTime" /> value.
    /// </summary>
    /// <param name="fieldName">
    ///     The name of the field.
    /// </param>
    /// <returns>
    ///     The <see cref="DateTime" /> value.
    /// </returns>
    DateTimeOffset? GetDateTimeOffset(string fieldName);

    /// <summary>
    ///     Retrieves a <see cref="DateTime" /> value.
    /// </summary>
    /// <param name="fieldName">
    ///     The name of the field.
    /// </param>
    /// <returns>
    ///     The <see cref="DateTime" /> value.
    /// </returns>
    DateTime? GetDateTime(string fieldName);

    /// <summary>
    ///     Retrieves a <see cref="DateTime" /> value.
    /// </summary>
    /// <param name="fieldName">
    ///     The name of the field.
    /// </param>
    /// <returns>
    ///     The <see cref="DateTime" /> value.
    /// </returns>
    DateOnly? GetDateOnly(string fieldName);

    /// <summary>
    ///     Retrieves a <see cref="DateTime" /> value.
    /// </summary>
    /// <param name="fieldName">
    ///     The name of the field.
    /// </param>
    /// <returns>
    ///     The <see cref="DateTime" /> value.
    /// </returns>
    TimeOnly? GetTimeOnly(string fieldName);

    /// <summary>
    ///     Retrieves a <see cref="DateTime" /> value.
    /// </summary>
    /// <param name="fieldName">
    ///     The name of the field.
    /// </param>
    /// <returns>
    ///     The <see cref="DateTime" /> value.
    /// </returns>
    TimeSpan? GetTimeSpan(string fieldName);

    /// <summary>
    ///     Retrieves a <typeparamref name="TEnum"/> value.
    /// </summary>
    /// <param name="fieldName">
    ///     The name.
    /// </param>
    /// <returns>
    ///     The <typeparamref name="TEnum"/> value.
    /// </returns>
    TEnum? GetEnum<TEnum>(string fieldName)
        where TEnum : struct, Enum;
}