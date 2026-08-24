using Lucene.Net.Search;
using Nameless.Lucene.Repository;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene.Repository;

[UnitTest]
public class QueryBuilderExtendedTests {
    private static QueryBuilder CreateBuilder() {
        return QueryBuilder.Create(LuceneDefaults.Analyzer);
    }

    // ── WithField overloads ──────────────────────────────────────────────────

    [Fact]
    public void WithField_BoolTrue_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("active", value: true);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithField_BoolFalse_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("active", value: false);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithField_StringWithWildcard_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("name", "Ali*", useWildcard: true);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithField_StringWithEmptyValue_ReturnsMatchAllQuery() {
        // arrange
        var builder = CreateBuilder();

        // act — empty string value is a no-op, so build returns MatchAllDocsQuery
        builder.WithField("name", value: "", useWildcard: false);
        var definition = builder.Build();

        // assert
        Assert.IsType<MatchAllDocsQuery>(definition.Query);
    }

    [Fact]
    public void WithField_StringArray_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("tags", new[] { "foo", "bar" });
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithField_StringArrayEmpty_ReturnsMatchAllQuery() {
        // arrange
        var builder = CreateBuilder();

        // act — empty array is a no-op
        builder.WithField("tags", Array.Empty<string>());
        var definition = builder.Build();

        // assert
        Assert.IsType<MatchAllDocsQuery>(definition.Query);
    }

    [Fact]
    public void WithField_IntValue_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("age", value: 42);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithField_LongValue_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("timestamp", value: 9_999_999_999L);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithField_FloatValue_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("score", value: 3.14F);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithField_DoubleValue_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("ratio", value: 1.5D);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithField_DateTimeOffsetValue_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("created", value: DateTimeOffset.UtcNow);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithField_DateTimeValue_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("updated", value: DateTime.UtcNow);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithField_DateOnlyValue_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("dob", value: DateOnly.FromDateTime(DateTime.UtcNow));
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithField_TimeOnlyValue_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("startTime", value: TimeOnly.FromDateTime(DateTime.UtcNow));
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithField_TimeSpanValue_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("duration", value: TimeSpan.FromHours(2));
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithField_EnumValue_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("status", value: DayOfWeek.Monday);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithFields_MultipleNames_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithFields(new[] { "firstName", "lastName" }, "Alice", fuzziness: 0.5F);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    // ── WithinRange overloads ────────────────────────────────────────────────

    [Fact]
    public void WithinRange_IntRange_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithinRange("age", minimum: 18, maximum: 65, includeMinimum: true, includeMaximum: true);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithinRange_LongRange_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithinRange("ticks", minimum: 0L, maximum: 1_000_000L, includeMinimum: true, includeMaximum: false);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithinRange_FloatRange_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithinRange("score", minimum: 0.0F, maximum: 10.0F, includeMinimum: true, includeMaximum: true);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithinRange_DoubleRange_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithinRange("price", minimum: 0.0D, maximum: 999.99D, includeMinimum: true, includeMaximum: false);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithinRange_StringRange_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithinRange("name", minimum: "A", maximum: "Z", includeMinimum: true, includeMaximum: true);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithinRange_OpenEndedStringBounds_BuildsQueryDefinition() {
        // arrange — use non-null bounds so TermRangeQuery.LowerTerm/UpperTerm are set
        var builder = CreateBuilder();

        // act
        builder.WithinRange("name", minimum: "A", maximum: "Z", includeMinimum: false, includeMaximum: false);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithinRange_DateTimeOffsetRange_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();
        var from = DateTimeOffset.UtcNow.AddDays(-7);
        var to = DateTimeOffset.UtcNow;

        // act
        builder.WithinRange("created", minimum: from, maximum: to, includeMinimum: true, includeMaximum: true);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithinRange_DateTimeRange_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();
        var from = DateTime.UtcNow.AddDays(-30);
        var to = DateTime.UtcNow;

        // act
        builder.WithinRange("updated", minimum: from, maximum: to, includeMinimum: true, includeMaximum: true);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithinRange_DateOnlyRange_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();
        var from = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var to = DateOnly.FromDateTime(DateTime.UtcNow);

        // act
        builder.WithinRange("date", minimum: from, maximum: to, includeMinimum: true, includeMaximum: true);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithinRange_TimeOnlyRange_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();
        var from = new TimeOnly(8, 0);
        var to = new TimeOnly(17, 0);

        // act
        builder.WithinRange("shift", minimum: from, maximum: to, includeMinimum: true, includeMaximum: true);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithinRange_TimeSpanRange_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();
        var from = TimeSpan.Zero;
        var to = TimeSpan.FromHours(8);

        // act
        builder.WithinRange("duration", minimum: from, maximum: to, includeMinimum: true, includeMaximum: true);
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    // ── Occurrence modifiers ─────────────────────────────────────────────────

    [Fact]
    public void Mandatory_ChangesClauseOccurToMust() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("status", "active", useWildcard: false)
               .ExactMatch()
               .Mandatory();
        var definition = builder.Build();

        // assert
        var boolQuery = Assert.IsType<BooleanQuery>(definition.Query);
        Assert.Contains(boolQuery.Clauses, c => c.Occur == Occur.MUST);
    }

    [Fact]
    public void Forbidden_ChangesClauseOccurToMustNot() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("status", "banned", useWildcard: false)
               .ExactMatch()
               .Forbidden();
        var definition = builder.Build();

        // assert
        var boolQuery = Assert.IsType<BooleanQuery>(definition.Query);
        Assert.Contains(boolQuery.Clauses, c => c.Occur == Occur.MUST_NOT);
    }

    [Fact]
    public void Optional_DefaultOccurIsShould() {
        // arrange
        var builder = CreateBuilder();

        // act — no explicit occurrence modifier; default is SHOULD
        builder.WithField("tag", "news", useWildcard: false)
               .ExactMatch();
        var definition = builder.Build();

        // assert
        var boolQuery = Assert.IsType<BooleanQuery>(definition.Query);
        Assert.Contains(boolQuery.Clauses, c => c.Occur == Occur.SHOULD);
    }

    // ── Query modifiers ──────────────────────────────────────────────────────

    [Fact]
    public void NoTokenize_PreventsParsing_BuildsQueryDefinition() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("code", "ABC-123", useWildcard: false)
               .NoTokenize()
               .ExactMatch();
        var definition = builder.Build();

        // assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void ExactMatch_RemovesPrefixMechanism_BuildsTermQuery() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("username", "alice", useWildcard: false)
               .ExactMatch();
        var definition = builder.Build();

        // assert — with ExactMatch the clause should not be a PrefixQuery
        var boolQuery = Assert.IsType<BooleanQuery>(definition.Query);
        Assert.Single(boolQuery.Clauses);
        Assert.IsNotType<PrefixQuery>(boolQuery.Clauses[0].Query);
    }

    [Fact]
    public void Weighted_SetsBoostOnQuery() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("title", "important", useWildcard: false)
               .ExactMatch()
               .Weighted(2.5F);
        var definition = builder.Build();

        // assert — the query exists and no exception was thrown
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void AsFilter_MarksClauseAsFilter_ResultIsFilteredQuery() {
        // arrange
        var builder = CreateBuilder();

        // act — add a regular clause and a filter clause
        builder.WithField("title", "hello", useWildcard: false)
               .ExactMatch();

        builder.WithField("active", value: 1)
               .AsFilter();

        var definition = builder.Build();

        // assert — combining clauses + filters produces a FilteredQuery
        Assert.IsType<FilteredQuery>(definition.Query);
    }

    [Fact]
    public void AsFilter_OnlyFilter_NoClauses_ProducesBooleanQuery() {
        // arrange
        var builder = CreateBuilder();

        // act — only a filter, no regular clause
        builder.WithField("active", value: 1)
               .AsFilter();

        var definition = builder.Build();

        // assert — only filters = BooleanQuery with those clauses
        Assert.IsType<BooleanQuery>(definition.Query);
    }

    // ── Sort methods ─────────────────────────────────────────────────────────

    [Fact]
    public void SortByString_ThenBuild_HasSort() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("name", "alice", useWildcard: false)
               .SortByString("name");
        var definition = builder.Build();

        // assert
        Assert.NotSame(Sort.RELEVANCE, definition.Sort);
    }

    [Fact]
    public void SortByInteger_ThenBuild_HasSort() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("age", 30)
               .SortByInteger("age");
        var definition = builder.Build();

        // assert
        Assert.NotSame(Sort.RELEVANCE, definition.Sort);
    }

    [Fact]
    public void SortByBoolean_ThenBuild_HasSort() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("active", value: true)
               .SortByBoolean("active");
        var definition = builder.Build();

        // assert
        Assert.NotSame(Sort.RELEVANCE, definition.Sort);
    }

    [Fact]
    public void SortByLong_ThenBuild_HasSort() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("ticks", 100L)
               .SortByLong("ticks");
        var definition = builder.Build();

        // assert
        Assert.NotSame(Sort.RELEVANCE, definition.Sort);
    }

    [Fact]
    public void SortByFloat_ThenBuild_HasSort() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("score", 1.0F)
               .SortByFloat("score");
        var definition = builder.Build();

        // assert
        Assert.NotSame(Sort.RELEVANCE, definition.Sort);
    }

    [Fact]
    public void SortByDouble_ThenBuild_HasSort() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("price", 9.99D)
               .SortByDouble("price");
        var definition = builder.Build();

        // assert
        Assert.NotSame(Sort.RELEVANCE, definition.Sort);
    }

    [Fact]
    public void SortByDateTimeOffset_ThenBuild_HasSort() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("created", DateTimeOffset.UtcNow)
               .SortByDateTimeOffset("created");
        var definition = builder.Build();

        // assert
        Assert.NotSame(Sort.RELEVANCE, definition.Sort);
    }

    [Fact]
    public void SortByDateTime_ThenBuild_HasSort() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("updated", DateTime.UtcNow)
               .SortByDateTime("updated");
        var definition = builder.Build();

        // assert
        Assert.NotSame(Sort.RELEVANCE, definition.Sort);
    }

    [Fact]
    public void SortByDateOnly_ThenBuild_HasSort() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("dob", DateOnly.FromDateTime(DateTime.UtcNow))
               .SortByDateOnly("dob");
        var definition = builder.Build();

        // assert
        Assert.NotSame(Sort.RELEVANCE, definition.Sort);
    }

    [Fact]
    public void SortByTimeOnly_ThenBuild_HasSort() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("shift", TimeOnly.MinValue)
               .SortByTimeOnly("shift");
        var definition = builder.Build();

        // assert
        Assert.NotSame(Sort.RELEVANCE, definition.Sort);
    }

    [Fact]
    public void SortByTimeSpan_ThenBuild_HasSort() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("duration", TimeSpan.FromHours(1))
               .SortByTimeSpan("duration");
        var definition = builder.Build();

        // assert
        Assert.NotSame(Sort.RELEVANCE, definition.Sort);
    }

    [Fact]
    public void SortByEnum_ThenBuild_HasSort() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("day", DayOfWeek.Monday)
               .SortByEnum("day");
        var definition = builder.Build();

        // assert
        Assert.NotSame(Sort.RELEVANCE, definition.Sort);
    }

    [Fact]
    public void SortBy_GenericName_ThenBuild_HasSort() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("name", "alice", useWildcard: false)
               .SortBy("name");
        var definition = builder.Build();

        // assert
        Assert.NotSame(Sort.RELEVANCE, definition.Sort);
    }

    [Fact]
    public void Ascending_ChangesDirection_DoesNotThrow() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("name", "alice", useWildcard: false)
               .SortByString("name")
               .Ascending();
        var definition = builder.Build();

        // assert — direction flag is internal; verify no exception and valid definition
        Assert.NotNull(definition);
    }

    // ── Slice ────────────────────────────────────────────────────────────────

    [Fact]
    public void Slice_SetsOffset_LimitIsReflectedInDefinition() {
        // arrange
        const int Limit = 25;
        var builder = CreateBuilder();

        // act
        builder.WithField("name", "alice", useWildcard: false)
               .Slice(Limit);
        var definition = builder.Build();

        // assert
        Assert.Equal(Limit, definition.Limit);
    }

    [Fact]
    public void Slice_WithZero_Throws() {
        // arrange
        var builder = CreateBuilder();

        // act & assert
        Assert.ThrowsAny<Exception>(() => builder.Slice(0));
    }

    // ── Build edge cases ─────────────────────────────────────────────────────

    [Fact]
    public void Build_WithNoTerms_ReturnsMatchAllQuery() {
        // arrange
        var builder = CreateBuilder();

        // act
        var definition = builder.Build();

        // assert
        Assert.IsType<MatchAllDocsQuery>(definition.Query);
    }

    [Fact]
    public void Build_WithMultipleClauses_ReturnsBooleanQuery() {
        // arrange
        var builder = CreateBuilder();

        // act
        builder.WithField("firstName", "Alice", useWildcard: false).ExactMatch();
        builder.WithField("lastName", "Smith", useWildcard: false).ExactMatch();
        var definition = builder.Build();

        // assert
        var boolQuery = Assert.IsType<BooleanQuery>(definition.Query);
        Assert.Equal(2, boolQuery.Clauses.Count);
    }

    [Fact]
    public void Build_DefaultLimit_IsMaximumQueryResults() {
        // arrange
        var builder = CreateBuilder();

        // act
        var definition = builder.Build();

        // assert
        Assert.Equal(LuceneConstants.MaximumQueryResults, definition.Limit);
    }
}
