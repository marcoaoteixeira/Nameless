namespace Nameless.Search;

/// <summary>
///     Contract to a search builder.
/// </summary>
public interface ISearchBuilder {
    /// <summary>
    ///     Parses a query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="escape">Use escape.</param>
    /// <param name="fuzziness">The amount of fuzzy.</param>
    /// <param name="fields">An array of fields.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder Parse(string query, bool escape, float fuzziness, string[] fields);

    /// <summary>
    ///     Adds the specified field and value to the search.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <param name="useWildcard">Use wild card search.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder WithField(string field, string value, bool useWildcard);

    /// <summary>
    ///     Adds the specified field and parts to the search.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="parts">The parts.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder WithField(string field, string[] parts);

    /// <summary>
    ///     Adds the specified field and value to the search.
    /// </summary>
    /// <param name="fieldName">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder WithField(string fieldName, int value);

    /// <summary>
    ///     Adds the specified field and value to the search.
    /// </summary>
    /// <param name="fieldName">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder WithField(string fieldName, DateTimeOffset value);

    /// <summary>
    ///     Adds the specified field and value to the search.
    /// </summary>
    /// <param name="fieldName">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder WithField(string fieldName, bool value);

    /// <summary>
    ///     Adds the specified field and value to the search.
    /// </summary>
    /// <param name="fieldName">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder WithField(string fieldName, double value);

    /// <summary>
    ///     Adds the specified field and value range to the search.
    /// </summary>
    /// <param name="fieldName">The field.</param>
    /// <param name="minimum">The minimum value.</param>
    /// <param name="maximum">The maximum value.</param>
    /// <param name="includeMinimum">Should include minimum value.</param>
    /// <param name="includeMaximum">Should include maximum value.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder WithinRange(string fieldName, string? minimum, string? maximum, bool includeMinimum,
                               bool includeMaximum);

    /// <summary>
    ///     Adds the specified field and value range to the search.
    /// </summary>
    /// <param name="fieldName">The field.</param>
    /// <param name="minimum">The minimum value.</param>
    /// <param name="maximum">The maximum value.</param>
    /// <param name="includeMinimum">Should include minimum value.</param>
    /// <param name="includeMaximum">Should include maximum value.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder WithinRange(string fieldName, int? minimum, int? maximum, bool includeMinimum, bool includeMaximum);

    /// <summary>
    ///     Adds the specified field and value range to the search.
    /// </summary>
    /// <param name="fieldName">The field.</param>
    /// <param name="minimum">The minimum value.</param>
    /// <param name="maximum">The maximum value.</param>
    /// <param name="includeMinimum">Should include minimum value.</param>
    /// <param name="includeMaximum">Should include maximum value.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder WithinRange(string fieldName, DateTime? minimum, DateTime? maximum, bool includeMinimum,
                               bool includeMaximum);

    /// <summary>
    ///     Adds the specified field and value range to the search.
    /// </summary>
    /// <param name="fieldName">The field.</param>
    /// <param name="minimum">The minimum value.</param>
    /// <param name="maximum">The maximum value.</param>
    /// <param name="includeMinimum">Should include minimum value.</param>
    /// <param name="includeMaximum">Should include maximum value.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder WithinRange(string fieldName, double? minimum, double? maximum, bool includeMinimum,
                               bool includeMaximum);

    /// <summary>
    ///     Marks the search as mandatory.
    /// </summary>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder Mandatory();

    /// <summary>
    ///     Marks the search as a forbidden.
    /// </summary>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder Forbidden();

    /// <summary>
    ///     Applied on string clauses, the searched value will not be tokenized.
    /// </summary>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder NoTokenize();

    /// <summary>
    ///     Applied on string clauses, it removes the default Prefix mechanism. Like 'broadcast' won't
    ///     return 'broadcasting'.
    /// </summary>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder ExactMatch();

    /// <summary>
    ///     Apply a specific weight.
    /// </summary>
    /// <param name="weight">
    ///     A value greater than zero, by which the score will be multiplied.
    ///     If greater than 1, it will improve the weight of a clause.
    /// </param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder Weighted(float weight);

    /// <summary>
    ///     Defines a clause as a filter, so that it only affect the results of the other clauses.
    ///     For instance, if the other clauses returns nothing, even if this filter has matches the
    ///     end result will be empty. It's like a two-pass query
    /// </summary>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder AsFilter();

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder SortBy(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder SortByInteger(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder SortByBoolean(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder SortByString(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder SortByDouble(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder SortByDateTime(string fieldName);

    /// <summary>
    ///     Order ascending.
    /// </summary>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder Ascending();

    /// <summary>
    ///     Slice the search.
    /// </summary>
    /// <param name="skip">Records to skip.</param>
    /// <param name="count">TotalDocumentsAffected of records to retrieve.</param>
    /// <returns>A <see cref="ISearchBuilder" /> object.</returns>
    ISearchBuilder Slice(int skip, int count);

    /// <summary>
    ///     Executes the search.
    /// </summary>
    /// <returns>A collection of search hits.</returns>
    IEnumerable<ISearchHit> Search();

    /// <summary>
    ///     Retrieves the document by its ID.
    /// </summary>
    /// <param name="documentID">The document ID.</param>
    /// <returns>The document.</returns>
    ISearchHit GetDocument(Guid documentID);

    /// <summary>
    ///     Retrieves the search bits.
    /// </summary>
    /// <returns>The search bits.</returns>
    ISearchBit GetBits();

    /// <summary>
    ///     Returns the count of records for the search.
    /// </summary>
    /// <returns>
    ///     An integer representing the number of records
    ///     that the search will retrieve.
    /// </returns>
    long Count();
}