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
    public IQueryBuilder WithFields(string[] names, string value, float fuzziness) {
        Guard.Against.Null(names);
        Guard.Against.NullOrWhiteSpace(value);

        value = QueryParserBase.Escape(value);

        foreach (var name in names) {
            CreatePendingClause();

            var parser = new QueryParser(
                Constants.CURRENT_VERSION,
                name,
                _analyzer
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
    public IQueryBuilder WithField(string name, bool value) {
        return WithField(name, value ? 1 : 0);
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string name, string value, bool useWildcard) {
        Guard.Against.NullOrWhiteSpace(name);

        if (string.IsNullOrWhiteSpace(value)) {
            return this;
        }

        CreatePendingClause();

        if (useWildcard) {
            _query = new WildcardQuery(new Term(name, value));

            return this;
        }

        value = QueryParserBase.Escape(value);

        _query = new TermQuery(new Term(name, value));

        return this;
    }

    /// <inheritdoc />
    /// <remarks>
    ///     It executes a phrase query, meaning that all values must be present
    ///     in the field.
    /// </remarks>
    public IQueryBuilder WithField(string name, string[] values) {
        Guard.Against.NullOrWhiteSpace(name);
        Guard.Against.Null(values);

        if (values.Length == 0 || values.All(string.IsNullOrWhiteSpace)) {
            return this;
        }

        CreatePendingClause();

        var phraseQuery = new PhraseQuery();
        var terms = values.Select(value => new Term(
            name,
            QueryParserBase.Escape(value)
        ));

        foreach (var term in terms) {
            phraseQuery.Add(term);
        }

        _query = phraseQuery;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string name, int value) {
        return WithinRange(
            name,
            value,
            value,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string name, long value) {
        return WithinRange(
            name,
            value,
            value,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string name, float value) {
        return WithinRange(
            name,
            value,
            value,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string name, double value) {
        Guard.Against.NullOrWhiteSpace(name);

        CreatePendingClause();

        _query = NumericRangeQuery.NewDoubleRange(name,
            value,
            value,
            minInclusive: true,
            maxInclusive: true
        );

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string name, DateTimeOffset value) {
        var number = value.ToUniversalTime().ToUnixTimeMilliseconds();

        return WithinRange(
            name,
            number,
            number,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string name, DateTime value) {
        var number = value.ToUnixTimeMilliseconds();

        return WithinRange(
            name,
            number,
            number,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string name, DateOnly value) {
        var number = value.ToUnixTimeMilliseconds();

        return WithinRange(
            name,
            number,
            number,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string name, TimeOnly value) {
        var number = value.Ticks;

        return WithinRange(
            name,
            number,
            number,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string name, TimeSpan value) {
        var number = value.Ticks;

        return WithinRange(
            name,
            number,
            number,
            includeMinimum: true,
            includeMaximum: true
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithField(string name, Enum value) {
        return WithField(name, value.ToString(), useWildcard: false);
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string name, string? minimum, string? maximum, bool includeMinimum,
        bool includeMaximum) {
        Guard.Against.NullOrWhiteSpace(name);

        CreatePendingClause();

        var minimumBytesRef = minimum is not null
            ? new BytesRef(QueryParserBase.Escape(minimum))
            : null;

        var maximumBytesRef = maximum is not null
            ? new BytesRef(QueryParserBase.Escape(maximum))
            : null;

        _query = new TermRangeQuery(
            name,
            minimumBytesRef,
            maximumBytesRef,
            includeMinimum,
            includeMaximum
        );

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string name, int? minimum, int? maximum, bool includeMinimum,
        bool includeMaximum) {
        Guard.Against.NullOrWhiteSpace(name);

        CreatePendingClause();

        _query = NumericRangeQuery.NewInt32Range(
            name,
            minimum,
            maximum,
            includeMinimum,
            includeMaximum
        );

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string name, long? minimum, long? maximum, bool includeMinimum,
        bool includeMaximum) {
        Guard.Against.NullOrWhiteSpace(name);

        CreatePendingClause();

        _query = NumericRangeQuery.NewInt64Range(
            name,
            minimum,
            maximum,
            includeMinimum,
            includeMaximum
        );

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string name, float? minimum, float? maximum, bool includeMinimum,
        bool includeMaximum) {
        Guard.Against.NullOrWhiteSpace(name);

        CreatePendingClause();

        _query = NumericRangeQuery.NewSingleRange(
            name,
            minimum,
            maximum,
            includeMinimum,
            includeMaximum
        );

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string name, double? minimum, double? maximum, bool includeMinimum,
        bool includeMaximum) {
        Guard.Against.NullOrWhiteSpace(name);

        CreatePendingClause();

        _query = NumericRangeQuery.NewDoubleRange(
            name,
            minimum,
            maximum,
            includeMinimum,
            includeMaximum
        );

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string name, DateTimeOffset? minimum, DateTimeOffset? maximum, bool includeMinimum,
        bool includeMaximum) {
        return WithinRange(
            name,
            minimum?.ToUniversalTime().Ticks,
            maximum?.ToUniversalTime().Ticks,
            includeMinimum,
            includeMaximum
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string name, DateTime? minimum, DateTime? maximum, bool includeMinimum,
        bool includeMaximum) {
        return WithinRange(
            name,
            minimum?.ToUniversalTime().Ticks,
            maximum?.ToUniversalTime().Ticks,
            includeMinimum,
            includeMaximum
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string name, DateOnly? minimum, DateOnly? maximum, bool includeMinimum,
        bool includeMaximum) {
        return WithinRange(
            name,
            minimum?.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).Ticks,
            maximum?.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).Ticks,
            includeMinimum,
            includeMaximum
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string name, TimeOnly? minimum, TimeOnly? maximum, bool includeMinimum,
        bool includeMaximum) {
        return WithinRange(
            name,
            minimum?.Ticks,
            maximum?.Ticks,
            includeMinimum,
            includeMaximum
        );
    }

    /// <inheritdoc />
    public IQueryBuilder WithinRange(string name, TimeSpan? minimum, TimeSpan? maximum, bool includeMinimum,
        bool includeMaximum) {
        return WithinRange(
            name,
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
    public IQueryBuilder SortBy(string name) {
        Guard.Against.NullOrWhiteSpace(name);

        _sort = name;
        _comparer = 0;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder SortByBoolean(string name) {
        return SortByInteger(name);
    }

    /// <inheritdoc />
    public IQueryBuilder SortByString(string name) {
        Guard.Against.NullOrWhiteSpace(name);

        _sort = name;
        _comparer = SortFieldType.STRING;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder SortByInteger(string name) {
        Guard.Against.NullOrWhiteSpace(name);

        _sort = name;
        _comparer = SortFieldType.INT32;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder SortByLong(string name) {
        Guard.Against.NullOrWhiteSpace(name);

        _sort = name;
        _comparer = SortFieldType.INT64;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder SortByFloat(string name) {
        Guard.Against.NullOrWhiteSpace(name);

        _sort = name;
        _comparer = SortFieldType.SINGLE;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder SortByDouble(string name) {
        Guard.Against.NullOrWhiteSpace(name);

        _sort = name;
        _comparer = SortFieldType.DOUBLE;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder SortByDateTimeOffset(string name) {
        return SortByLong(name);
    }

    /// <inheritdoc />
    public IQueryBuilder SortByDateTime(string name) {
        return SortByLong(name);
    }

    /// <inheritdoc />
    public IQueryBuilder SortByDateOnly(string name) {
        return SortByLong(name);
    }

    /// <inheritdoc />
    public IQueryBuilder SortByTimeOnly(string name) {
        return SortByLong(name);
    }

    /// <inheritdoc />
    public IQueryBuilder SortByTimeSpan(string name) {
        return SortByLong(name);
    }

    /// <inheritdoc />
    public IQueryBuilder SortByEnum(string name) {
        return SortByString(name);
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
                    _analyzer,
                    term.Field,
                    term.Text
                ).FirstOrDefault();

                _query = new TermQuery(new Term(term.Field, analyzedText));
            }

            if (_query is TermRangeQuery termRangeQuery) {
                var lowerTerm = AnalyzeText(
                    _analyzer,
                    termRangeQuery.Field,
                    termRangeQuery.LowerTerm.Utf8ToString()
                ).FirstOrDefault();

                var upperTerm = AnalyzeText(
                    _analyzer,
                    termRangeQuery.Field,
                    termRangeQuery.UpperTerm.Utf8ToString()
                ).FirstOrDefault();

                _query = new TermRangeQuery(
                    termRangeQuery.Field,
                    new BytesRef(lowerTerm),
                    new BytesRef(upperTerm),
                    termRangeQuery.IncludesLower,
                    termRangeQuery.IncludesUpper
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
            catch {
                /* swallow */
            }
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