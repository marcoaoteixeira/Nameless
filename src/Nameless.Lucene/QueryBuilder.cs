using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace Nameless.Lucene;

/// <summary>
///     Default implementation of <see cref="IQueryBuilder"/>.
/// </summary>
public sealed class QueryBuilder : IQueryBuilder {
    public const float MINIMUM_FUZZINESS = 0F;
    public const float MAXIMUM_FUZZINESS = 2F;

    private const double EPSILON = 0.001D;

    private readonly Analyzer _analyzer;
    private readonly List<BooleanClause> _clauses = [];
    private readonly List<BooleanClause> _filters = [];

    private SortFieldType _comparer;
    private string _sort;
    private bool _sortDescending;
    private int _start;
    private int _limit = Defaults.QUERY_MAXIMUM_RESULTS;

    // pending clause attributes
    private Query? _query;
    private float _boost;
    private bool _noTokenize;
    private bool _exactMatch;
    private bool _asFilter;
    private Occur _occur;

    /// <summary>
    ///     Initializes a new instance of the <see cref="QueryBuilder"/> class.
    /// </summary>
    /// <param name="analyzer">
    ///     The analyzer to be used for parsing and analyzing text.
    /// </param>
    public QueryBuilder(Analyzer analyzer) {
        _analyzer = Guard.Against.Null(analyzer);

        _sort = string.Empty;
        _comparer = 0;
        _sortDescending = true;

        InitializePendingClause();
    }

    /// <inheritdoc />
    public IQueryBuilder WithFields(string[] fieldNames, string value, float fuzziness) {
        Guard.Against.Null(fieldNames);
        Guard.Against.NullOrWhiteSpace(value);

        value = QueryParserBase.Escape(value);

        foreach (var fieldName in fieldNames) {
            CreatePendingClause();

            var parser = new QueryParser(
                matchVersion: Constants.CURRENT_VERSION,
                f: fieldName,
                a: _analyzer
            ) { FuzzyMinSim = EnsureFuzzinessRange(fuzziness) };

            _query = parser.Parse(value);
        }

        return this;

        static float EnsureFuzzinessRange(float value) {
            return value is >= MINIMUM_FUZZINESS or <= MAXIMUM_FUZZINESS
                ? value
                : MINIMUM_FUZZINESS;
        }
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string fieldName, bool value) {
        return WithField(fieldName, value ? 1 : 0);
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string fieldName, string value, bool useWildcard) {
        Guard.Against.NullOrWhiteSpace(fieldName);

        if (string.IsNullOrWhiteSpace(value)) {
            return this;
        }

        CreatePendingClause();

        value = QueryParserBase.Escape(value);

        if (useWildcard) { _query = new WildcardQuery(new Term(fieldName, value)); }
        else { _query = new TermQuery(new Term(fieldName, value)); }

        return this;
    }

    /// <inheritdoc />
    /// <remarks>
    ///     It executes a phrase query, meaning that all values must be present
    ///     in the field.
    /// </remarks>
    public IQueryBuilder WithField(string fieldName, string[] values) {
        Guard.Against.NullOrWhiteSpace(fieldName);
        Guard.Against.Null(values);

        if (values.Length == 0 || values.All(string.IsNullOrWhiteSpace)) {
            return this;
        }

        CreatePendingClause();

        var phraseQuery = new PhraseQuery();
        var terms = values.Select(value => new Term(
            fld: fieldName,
            text: QueryParserBase.Escape(value)
        ));

        foreach (var term in terms) {
            phraseQuery.Add(term);
        }

        _query = phraseQuery;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string fieldName, int value) {
        return WithinRange(
            fieldName,
            minimum: value,
            maximum: value,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string fieldName, long value) {
        return WithinRange(
            fieldName,
            minimum: value,
            maximum: value,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string fieldName, float value) {
        return WithinRange(
            fieldName: fieldName,
            minimum: value,
            maximum: value,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string fieldName, double value) {
        Guard.Against.NullOrWhiteSpace(fieldName);

        CreatePendingClause();

        _query = NumericRangeQuery.NewDoubleRange(field: fieldName,
            min: value,
            max: value,
            minInclusive: true,
            maxInclusive: true
        );

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string fieldName, DateTimeOffset value) {
        var number = value.ToUniversalTime().ToUnixTimeMilliseconds();

        return WithinRange(
            fieldName,
            minimum: number,
            maximum: number,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string fieldName, DateTime value) {
        var number = value.ToUnixTimeMilliseconds();

        return WithinRange(
            fieldName,
            minimum: number,
            maximum: number,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string fieldName, DateOnly value) {
        var number = value.ToUnixTimeMilliseconds();

        return WithinRange(
            fieldName,
            minimum: number,
            maximum: number,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string fieldName, TimeOnly value) {
        var number = value.Ticks;

        return WithinRange(
            fieldName,
            minimum: number,
            maximum: number,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string fieldName, TimeSpan value) {
        var number = value.Ticks;

        return WithinRange(
            fieldName,
            minimum: number,
            maximum: number,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string fieldName, Enum value) {
        return WithField(fieldName, value.ToString(), useWildcard: false);
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string fieldName, string? minimum, string? maximum, bool includeMinimum, bool includeMaximum) {
        Guard.Against.NullOrWhiteSpace(fieldName);

        CreatePendingClause();

        var minimumBytesRef = minimum is not null
            ? new BytesRef(QueryParserBase.Escape(minimum))
            : null;

        var maximumBytesRef = maximum is not null
            ? new BytesRef(QueryParserBase.Escape(maximum))
            : null;

        _query = new TermRangeQuery(
            fieldName,
            minimumBytesRef,
            maximumBytesRef,
            includeMinimum,
            includeMaximum
        );

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string fieldName, int? minimum, int? maximum, bool includeMinimum, bool includeMaximum) {
        Guard.Against.NullOrWhiteSpace(fieldName);

        CreatePendingClause();

        _query = NumericRangeQuery.NewInt32Range(
            fieldName,
            minimum,
            maximum,
            includeMinimum,
            includeMaximum
        );

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string fieldName, long? minimum, long? maximum, bool includeMinimum, bool includeMaximum) {
        Guard.Against.NullOrWhiteSpace(fieldName);

        CreatePendingClause();

        _query = NumericRangeQuery.NewInt64Range(
            fieldName,
            minimum,
            maximum,
            includeMinimum,
            includeMaximum
        );

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string fieldName, float? minimum, float? maximum, bool includeMinimum, bool includeMaximum) {
        Guard.Against.NullOrWhiteSpace(fieldName);

        CreatePendingClause();

        _query = NumericRangeQuery.NewSingleRange(
            fieldName,
            minimum,
            maximum,
            includeMinimum,
            includeMaximum
        );

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string fieldName, double? minimum, double? maximum, bool includeMinimum, bool includeMaximum) {
        Guard.Against.NullOrWhiteSpace(fieldName);

        CreatePendingClause();

        _query = NumericRangeQuery.NewDoubleRange(
            fieldName,
            minimum,
            maximum,
            includeMinimum,
            includeMaximum
        );

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string fieldName, DateTimeOffset? minimum, DateTimeOffset? maximum, bool includeMinimum, bool includeMaximum) {
        return WithinRange(
            fieldName,
            minimum?.ToUniversalTime().ToUnixTimeMilliseconds(),
            maximum?.ToUniversalTime().ToUnixTimeMilliseconds(),
            includeMinimum,
            includeMaximum
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string fieldName, DateTime? minimum, DateTime? maximum, bool includeMinimum, bool includeMaximum) {
        return WithinRange(
            fieldName,
            minimum?.ToUnixTimeMilliseconds(),
            maximum?.ToUnixTimeMilliseconds(),
            includeMinimum,
            includeMaximum
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string fieldName, DateOnly? minimum, DateOnly? maximum, bool includeMinimum, bool includeMaximum) {
        return WithinRange(
            fieldName,
            minimum?.ToUnixTimeMilliseconds(),
            maximum?.ToUnixTimeMilliseconds(),
            includeMinimum,
            includeMaximum
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string fieldName, TimeOnly? minimum, TimeOnly? maximum, bool includeMinimum, bool includeMaximum) {
        return WithinRange(
            fieldName,
            minimum?.Ticks,
            maximum?.Ticks,
            includeMinimum,
            includeMaximum
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string fieldName, TimeSpan? minimum, TimeSpan? maximum, bool includeMinimum, bool includeMaximum) {
        return WithinRange(
            fieldName,
            minimum?.Ticks,
            maximum?.Ticks,
            includeMinimum,
            includeMaximum
        );
    }

    /// <inheritdoc />
    public IQueryBuilder Mandatory() {
        _occur = Occur.MUST;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder Forbidden() {
        _occur = Occur.MUST_NOT;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder NoTokenize() {
        _noTokenize = true;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder ExactMatch() {
        _exactMatch = true;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder Weighted(float weight) {
        _boost = weight;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder AsFilter() {
        _asFilter = true;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder SortBy(string fieldName) {
        Guard.Against.NullOrWhiteSpace(fieldName);

        _sort = fieldName;
        _comparer = 0;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder SortByBoolean(string fieldName) {
        return SortByInteger(fieldName);
    }

    /// <inheritdoc />
    public IQueryBuilder SortByString(string fieldName) {
        Guard.Against.NullOrWhiteSpace(fieldName);

        _sort = fieldName;
        _comparer = SortFieldType.STRING;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder SortByInteger(string fieldName) {
        Guard.Against.NullOrWhiteSpace(fieldName);

        _sort = fieldName;
        _comparer = SortFieldType.INT32;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder SortByLong(string fieldName) {
        Guard.Against.NullOrWhiteSpace(fieldName);

        _sort = fieldName;
        _comparer = SortFieldType.INT64;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder SortByFloat(string fieldName) {
        Guard.Against.NullOrWhiteSpace(fieldName);

        _sort = fieldName;
        _comparer = SortFieldType.SINGLE;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder SortByDouble(string fieldName) {
        Guard.Against.NullOrWhiteSpace(fieldName);

        _sort = fieldName;
        _comparer = SortFieldType.DOUBLE;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder SortByDateTimeOffset(string fieldName) {
        return SortByLong(fieldName);
    }

    /// <inheritdoc />
    public IQueryBuilder SortByDateTime(string fieldName) {
        return SortByLong(fieldName);
    }

    /// <inheritdoc />
    public IQueryBuilder SortByDateOnly(string fieldName) {
        return SortByLong(fieldName);
    }

    /// <inheritdoc />
    public IQueryBuilder SortByTimeOnly(string fieldName) {
        return SortByLong(fieldName);
    }

    /// <inheritdoc />
    public IQueryBuilder SortByTimeSpan(string fieldName) {
        return SortByLong(fieldName);
    }

    /// <inheritdoc />
    public IQueryBuilder SortByEnum(string fieldName) {
        return SortByString(fieldName);
    }

    /// <inheritdoc />
    public IQueryBuilder Ascending() {
        _sortDescending = false;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder Slice(int start, int limit) {
        _start = Guard.Against.LowerThan(start, compare: 0);
        _limit = Guard.Against.LowerOrEqual(limit, compare: 0);

        return this;
    }

    /// <inheritdoc />
    public QueryDefinition Build() {
        var query = CreateQuery();
        var sort = !string.IsNullOrEmpty(_sort)
            ? new Sort(new SortField(_sort, _comparer, _sortDescending))
            : Sort.RELEVANCE;

        return new QueryDefinition(query, sort, _start, _limit);
    }

    private void InitializePendingClause() {
        _occur = Occur.SHOULD;
        _exactMatch = false;
        _noTokenize = false;
        _query = null;
        _boost = 0;
        _asFilter = false;
    }

    private void CreatePendingClause() {
        if (_query is null) { return; }

        // comparing floating-point numbers using an epsilon value
        if (Math.Abs(_boost - 0) > EPSILON) { _query.Boost = _boost; }

        if (!_noTokenize) {
            if (_query is TermQuery termQuery) {
                var term = termQuery.Term;
                var analyzedText = AnalyzeText(
                    analyzer: _analyzer,
                    field: term.Field,
                    text: term.Text
                ).FirstOrDefault();

                _query = new TermQuery(new Term(term.Field, analyzedText));
            }

            if (_query is TermRangeQuery termRangeQuery) {
                var lowerTerm = AnalyzeText(
                    analyzer: _analyzer,
                    field: termRangeQuery.Field,
                    text: termRangeQuery.LowerTerm.Utf8ToString()
                ).FirstOrDefault();

                var upperTerm = AnalyzeText(
                    analyzer: _analyzer,
                    field: termRangeQuery.Field,
                    text: termRangeQuery.UpperTerm.Utf8ToString()
                ).FirstOrDefault();

                _query = new TermRangeQuery(
                    field: termRangeQuery.Field,
                    lowerTerm: new BytesRef(lowerTerm),
                    upperTerm: new BytesRef(upperTerm),
                    includeLower: termRangeQuery.IncludesLower,
                    includeUpper: termRangeQuery.IncludesUpper
                );
            }
        }

        if (!_exactMatch) {
            if (_query is TermQuery termQuery) {
                _query = new PrefixQuery(
                    new Term(
                        termQuery.Term.Field,
                        termQuery.Term.Text
                    )
                );
            }
        }

        if (_asFilter) { _filters.Add(new BooleanClause(_query, _occur)); }
        else { _clauses.Add(new BooleanClause(_query, _occur)); }

        InitializePendingClause();
    }

    private static List<string> AnalyzeText(Analyzer analyzer, string field, string text) {
        if (string.IsNullOrEmpty(text)) { return []; }

        var result = new List<string>();

        using var reader = new StringReader(text);
        using var token = analyzer.GetTokenStream(field, reader);

        token.Reset();

        while (token.IncrementToken()) {
            try {
                var attr = token.GetAttribute<ICharTermAttribute>();
                if (attr is not null) {
                    result.Add(attr.ToString());
                }
            }
            catch { /* swallow */ }
        }

        return result;
    }

    private Query CreateQuery() {
        CreatePendingClause();

        var booleanQuery = new BooleanQuery();
        Query resultQuery = booleanQuery;

        if (_clauses.Count == 0) {
            if (_filters.Count > 0) {
                // only filters applied => transform to a boolean query
                foreach (var clause in _filters) {
                    booleanQuery.Add(clause);
                }

                resultQuery = booleanQuery;
            }
            else {
                // search all documents, without filter or clause
                resultQuery = new MatchAllDocsQuery();
            }
        }
        else {
            foreach (var clause in _clauses) {
                booleanQuery.Add(clause);
            }

            if (_filters.Count > 0) {
                var filter = new BooleanQuery();
                foreach (var clause in _filters) {
                    filter.Add(clause);
                }

                resultQuery = new FilteredQuery(booleanQuery,
                    new QueryWrapperFilter(filter)
                );
            }
        }

        return resultQuery;
    }
}