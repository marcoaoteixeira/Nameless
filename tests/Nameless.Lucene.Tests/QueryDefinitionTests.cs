using Lucene.Net.Search;
using Nameless.Lucene.Fixtures;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene;

[UnitTest]
public class QueryDefinitionTests {
    [Fact]
    public void WhenConstructing_WithValidParameters_ThenReturnsNewInstance() {
        // arrange
        var query = new FakeQuery();
        var sort = Sort.RELEVANCE;
        const int Start = 0;
        const int Limit = 1;

        // act
        var actual = new QueryDefinition(query, sort, Start, Limit);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(query, actual.Query);
            Assert.Equal(sort, actual.Sort);
            Assert.Equal(Start, actual.Start);
            Assert.Equal(Limit, actual.Limit);
        });
    }

    [Fact]
    public void WhenConstructing_WhenStartIsLowerThanZero_ThenThrowsArgumentOutOfRangeException() {
        // arrange
        var query = new FakeQuery();
        var sort = Sort.RELEVANCE;
        const int Limit = 1;

        const int Start = -1;

        // act
        var actual = Record.Exception(() => new QueryDefinition(query, sort, Start, Limit));

        // assert
        Assert.IsType<ArgumentOutOfRangeException>(actual);
    }

    [Fact]
    public void WhenConstructing_WhenLimitIsLowerThanZero_ThenThrowsArgumentOutOfRangeException() {
        // arrange
        var query = new FakeQuery();
        var sort = Sort.RELEVANCE;
        const int Start = 0;

        const int Limit = -1;

        // act
        var actual = Record.Exception(() => new QueryDefinition(query, sort, Start, Limit));

        // assert
        Assert.IsType<ArgumentOutOfRangeException>(actual);
    }

    [Fact]
    public void WhenConstructing_WhenLimitIsZero_ThenThrowsArgumentOutOfRangeException() {
        // arrange
        var query = new FakeQuery();
        var sort = Sort.RELEVANCE;
        const int Start = 0;

        const int Limit = 0;

        // act
        var actual = Record.Exception(() => new QueryDefinition(query, sort, Start, Limit));

        // assert
        Assert.IsType<ArgumentOutOfRangeException>(actual);
    }
}
