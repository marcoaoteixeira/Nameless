using Lucene.Net.Search;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene.Repository;

[UnitTest]
public class QueryBuilderTests {
    private static QueryBuilder CreateBuilder() {
        return QueryBuilder.Create(LuceneDefaults.Analyzer);
    }

    [Fact]
    public void Create_ReturnsBuilderInstance() {
        // Arrange & Act
        var builder = CreateBuilder();

        // Assert
        Assert.NotNull(builder);
        Assert.IsType<QueryBuilder>(builder);
    }

    [Fact]
    public void WithField_StringValue_BuildsQueryDefinition() {
        // Arrange
        var builder = CreateBuilder();

        // Act
        builder.WithField("name", "Alice", useWildcard: false);
        var definition = builder.Build();

        // Assert
        Assert.NotNull(definition);
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void WithinRange_IntRange_BuildsQueryDefinition() {
        // Arrange
        var builder = CreateBuilder();

        // Act
        builder.WithinRange("age", minimum: 18, maximum: 65, includeMinimum: true, includeMaximum: true);
        var definition = builder.Build();

        // Assert
        Assert.NotNull(definition.Query);
    }

    [Fact]
    public void Mandatory_ChangesOccur() {
        // Arrange
        var builder = CreateBuilder();

        // Act — Mandatory() should not throw; the resulting query is a BooleanQuery
        // with a MUST clause.
        builder.WithField("status", "active", useWildcard: false).Mandatory();
        var definition = builder.Build();

        // Assert
        var boolQuery = Assert.IsType<BooleanQuery>(definition.Query);
        Assert.Single(boolQuery.Clauses);
        Assert.Equal(Occur.MUST, boolQuery.Clauses[0].Occur);
    }

    [Fact]
    public void SortByString_SetsSort() {
        // Arrange
        var builder = CreateBuilder();

        // Act
        builder.WithField("name", "Alice", useWildcard: false)
               .SortByString("name");
        var definition = builder.Build();

        // Assert — sort should no longer be the default RELEVANCE sort
        Assert.NotSame(Sort.RELEVANCE, definition.Sort);
    }

    [Fact]
    public void Slice_SetsMaxDocCount() {
        // Arrange
        const int Limit = 10;
        var builder = CreateBuilder();

        // Act
        builder.WithField("name", "Alice", useWildcard: false)
               .Slice(Limit);
        var definition = builder.Build();

        // Assert
        Assert.Equal(Limit, definition.Limit);
    }

    [Fact]
    public void Build_ReturnsQueryDefinition() {
        // Arrange
        var builder = CreateBuilder();

        // Act
        var definition = builder.Build();

        // Assert
        Assert.NotNull(definition);
        Assert.IsType<MatchAllDocsQuery>(definition.Query);
        Assert.Equal(LuceneConstants.MaximumQueryResults, definition.Limit);
    }
}
