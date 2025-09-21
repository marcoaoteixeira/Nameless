namespace Nameless.Lucene;

public interface IQueryBuilder {
    /// <summary>
    ///     Includes the specified fields and value to the search query.
    /// </summary>
    /// <param name="fieldNames">
    ///     The fields name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="fuzziness">
    ///     The amount of fuzzy, the value must be between 0.0 and 2.0.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithFields(string[] fieldNames, string value, float fuzziness);

    /// <summary>
    ///     Includes the specified field and value to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithField(string fieldName, bool value);

    /// <summary>
    ///     Includes the specified field and value to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="useWildcard">
    ///     Whether it should Use wild card for search.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithField(string fieldName, string value, bool useWildcard);

    /// <summary>
    ///     Includes the specified field and values to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="values">
    ///     The values.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithField(string fieldName, string[] values);

    /// <summary>
    ///     Includes the specified field and value to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithField(string fieldName, int value);

    /// <summary>
    ///     Includes the specified field and value to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithField(string fieldName, long value);

    /// <summary>
    ///     Includes the specified field and value to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithField(string fieldName, float value);

    /// <summary>
    ///     Includes the specified field and value to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithField(string fieldName, double value);

    /// <summary>
    ///     Includes the specified field and value to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithField(string fieldName, DateTimeOffset value);

    /// <summary>
    ///     Includes the specified field and value to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithField(string fieldName, DateTime value);

    /// <summary>
    ///     Includes the specified field and value to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithField(string fieldName, DateOnly value);

    /// <summary>
    ///     Includes the specified field and value to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithField(string fieldName, TimeOnly value);

    /// <summary>
    ///     Includes the specified field and value to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithField(string fieldName, TimeSpan value);

    /// <summary>
    ///     Includes the specified field and value to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithField(string fieldName, Enum value);

    /// <summary>
    ///     Includes the specified field and value range to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="minimum">
    ///     The minimum value.
    /// </param>
    /// <param name="maximum">
    ///     The maximum value.
    /// </param>
    /// <param name="includeMinimum">
    ///     Whether it should include minimum value.
    /// </param>
    /// <param name="includeMaximum">
    ///     Whether it should include maximum value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithinRange(string fieldName, string? minimum, string? maximum, bool includeMinimum, bool includeMaximum);

    /// <summary>
    ///     Includes the specified field and value range to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="minimum">
    ///     The minimum value.
    /// </param>
    /// <param name="maximum">
    ///     The maximum value.
    /// </param>
    /// <param name="includeMinimum">
    ///     Whether it should include minimum value.
    /// </param>
    /// <param name="includeMaximum">
    ///     Whether it should include maximum value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithinRange(string fieldName, int? minimum, int? maximum, bool includeMinimum, bool includeMaximum);

    /// <summary>
    ///     Includes the specified field and value range to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="minimum">
    ///     The minimum value.
    /// </param>
    /// <param name="maximum">
    ///     The maximum value.
    /// </param>
    /// <param name="includeMinimum">
    ///     Whether it should include minimum value.
    /// </param>
    /// <param name="includeMaximum">
    ///     Whether it should include maximum value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithinRange(string fieldName, long? minimum, long? maximum, bool includeMinimum, bool includeMaximum);

    /// <summary>
    ///     Includes the specified field and value range to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="minimum">
    ///     The minimum value.
    /// </param>
    /// <param name="maximum">
    ///     The maximum value.
    /// </param>
    /// <param name="includeMinimum">
    ///     Whether it should include minimum value.
    /// </param>
    /// <param name="includeMaximum">
    ///     Whether it should include maximum value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithinRange(string fieldName, float? minimum, float? maximum, bool includeMinimum, bool includeMaximum);

    /// <summary>
    ///     Includes the specified field and value range to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="minimum">
    ///     The minimum value.
    /// </param>
    /// <param name="maximum">
    ///     The maximum value.
    /// </param>
    /// <param name="includeMinimum">
    ///     Whether it should include minimum value.
    /// </param>
    /// <param name="includeMaximum">
    ///     Whether it should include maximum value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithinRange(string fieldName, double? minimum, double? maximum, bool includeMinimum, bool includeMaximum);

    /// <summary>
    ///     Includes the specified field and value range to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="minimum">
    ///     The minimum value.
    /// </param>
    /// <param name="maximum">
    ///     The maximum value.
    /// </param>
    /// <param name="includeMinimum">
    ///     Whether it should include minimum value.
    /// </param>
    /// <param name="includeMaximum">
    ///     Whether it should include maximum value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithinRange(string fieldName, DateTimeOffset? minimum, DateTimeOffset? maximum, bool includeMinimum, bool includeMaximum);

    /// <summary>
    ///     Includes the specified field and value range to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="minimum">
    ///     The minimum value.
    /// </param>
    /// <param name="maximum">
    ///     The maximum value.
    /// </param>
    /// <param name="includeMinimum">
    ///     Whether it should include minimum value.
    /// </param>
    /// <param name="includeMaximum">
    ///     Whether it should include maximum value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithinRange(string fieldName, DateTime? minimum, DateTime? maximum, bool includeMinimum, bool includeMaximum);

    /// <summary>
    ///     Includes the specified field and value range to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="minimum">
    ///     The minimum value.
    /// </param>
    /// <param name="maximum">
    ///     The maximum value.
    /// </param>
    /// <param name="includeMinimum">
    ///     Whether it should include minimum value.
    /// </param>
    /// <param name="includeMaximum">
    ///     Whether it should include maximum value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithinRange(string fieldName, DateOnly? minimum, DateOnly? maximum, bool includeMinimum, bool includeMaximum);

    /// <summary>
    ///     Includes the specified field and value range to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="minimum">
    ///     The minimum value.
    /// </param>
    /// <param name="maximum">
    ///     The maximum value.
    /// </param>
    /// <param name="includeMinimum">
    ///     Whether it should include minimum value.
    /// </param>
    /// <param name="includeMaximum">
    ///     Whether it should include maximum value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithinRange(string fieldName, TimeOnly? minimum, TimeOnly? maximum, bool includeMinimum, bool includeMaximum);

    /// <summary>
    ///     Includes the specified field and value range to the search query.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <param name="minimum">
    ///     The minimum value.
    /// </param>
    /// <param name="maximum">
    ///     The maximum value.
    /// </param>
    /// <param name="includeMinimum">
    ///     Whether it should include minimum value.
    /// </param>
    /// <param name="includeMaximum">
    ///     Whether it should include maximum value.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder WithinRange(string fieldName, TimeSpan? minimum, TimeSpan? maximum, bool includeMinimum, bool includeMaximum);

    /// <summary>
    ///     Marks the search as mandatory.
    /// </summary>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder Mandatory();

    /// <summary>
    ///     Marks the search as a forbidden.
    /// </summary>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder Forbidden();

    /// <summary>
    ///     Applied on string clauses, the searched value will not be tokenized.
    /// </summary>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder NoTokenize();

    /// <summary>
    ///     Applied on string clauses, it removes the default Prefix mechanism.
    ///     Like 'broadcast' won't return 'broadcasting'.
    /// </summary>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder ExactMatch();

    /// <summary>
    ///     Apply a specific weight.
    /// </summary>
    /// <param name="weight">
    ///     A value greater than zero, by which the score will be multiplied.
    ///     If greater than 1, it will improve the weight of a clause.
    /// </param>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder Weighted(float weight);

    /// <summary>
    ///     Defines a clause as a filter, so that it only affect the results
    ///     of the other clauses. For instance, if the other clauses returns
    ///     nothing, even if this filter has matches the end result will be
    ///     empty. It's like a two-pass query.
    /// </summary>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder AsFilter();

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <returns>
    /// 	The current <see cref="IQueryBuilder" /> so other actions
    /// 	can be chained.
    /// </returns>
    IQueryBuilder SortBy(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <returns>
    /// 	The current <see cref="IQueryBuilder" /> so other actions
    /// 	can be chained.
    /// </returns>
    IQueryBuilder SortByBoolean(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <returns>
    /// 	The current <see cref="IQueryBuilder" /> so other actions
    /// 	can be chained.
    /// </returns>
    IQueryBuilder SortByString(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <returns>
    /// 	The current <see cref="IQueryBuilder" /> so other actions
    /// 	can be chained.
    /// </returns>
    IQueryBuilder SortByInteger(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <returns>
    /// 	The current <see cref="IQueryBuilder" /> so other actions
    /// 	can be chained.
    /// </returns>
    IQueryBuilder SortByLong(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <returns>
    /// 	The current <see cref="IQueryBuilder" /> so other actions
    /// 	can be chained.
    /// </returns>
    IQueryBuilder SortByFloat(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <returns>
    /// 	The current <see cref="IQueryBuilder" /> so other actions
    /// 	can be chained.
    /// </returns>
    IQueryBuilder SortByDouble(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <returns>
    /// 	The current <see cref="IQueryBuilder" /> so other actions
    /// 	can be chained.
    /// </returns>
    IQueryBuilder SortByDateTimeOffset(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <returns>
    /// 	The current <see cref="IQueryBuilder" /> so other actions
    /// 	can be chained.
    /// </returns>
    IQueryBuilder SortByDateTime(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <returns>
    /// 	The current <see cref="IQueryBuilder" /> so other actions
    /// 	can be chained.
    /// </returns>
    IQueryBuilder SortByDateOnly(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <returns>
    /// 	The current <see cref="IQueryBuilder" /> so other actions
    /// 	can be chained.
    /// </returns>
    IQueryBuilder SortByTimeOnly(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <returns>
    /// 	The current <see cref="IQueryBuilder" /> so other actions
    /// 	can be chained.
    /// </returns>
    IQueryBuilder SortByTimeSpan(string fieldName);

    /// <summary>
    ///     Sort by field.
    /// </summary>
    /// <param name="fieldName">
    ///     The field name.
    /// </param>
    /// <returns>
    /// 	The current <see cref="IQueryBuilder" /> so other actions
    /// 	can be chained.
    /// </returns>
    IQueryBuilder SortByEnum(string fieldName);

    /// <summary>
    ///     Orders ascending.
    /// </summary>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder Ascending();

    /// <summary>
    ///     Slices the query results.
    /// </summary>
    /// <returns>
    ///     The current <see cref="IQueryBuilder" /> so other actions
    ///     can be chained.
    /// </returns>
    IQueryBuilder Slice(int start, int limit);

    /// <summary>
    ///     Builds the search query definition.
    /// </summary>
    /// <returns>
    ///     A <see cref="QueryDefinition" /> object.
    /// </returns>
    QueryDefinition Build();
}