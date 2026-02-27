using Lucene.Net.Analysis.Standard;
using Nameless.Lucene.Fixtures;
using Nameless.Lucene.Infrastructure.Implementations;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene;

[UnitTest]
public class AnalyzerProviderTests {
    [Fact]
    public void WhenGettingAnalyzer_WhenNoAnalyzerSelectorFound_ThenReturnsDefaultAnalyzer() {
        const string IndexName = nameof(IndexName);

        // arrange
        var provider = new AnalyzerProvider([]);

        // act
        var actual = provider.GetAnalyzer(IndexName);

        // assert
        Assert.IsType<StandardAnalyzer>(actual);
    }

    [Fact]
    public void WhenGettingAnalyzer_WhenAnalyzerSelectorFoundForIndex_ThenReturnsSpecificAnalyzer() {
        const string IndexName = nameof(IndexName);

        // arrange
        var analyzerSelector = new FakeAnalyzerSelector(IndexName);
        var provider = new AnalyzerProvider([analyzerSelector]);

        // act
        var actual = provider.GetAnalyzer(IndexName);

        // assert
        Assert.IsType<FakeAnalyzer>(actual);
    }

    [Fact]
    public void WhenGettingAnalyzer_WhenHasAnalyzerSelector_ButNotForSpecifiedIndex_ThenReturnsDefaultAnalyzer() {
        const string IndexName = nameof(IndexName);
        const string AnotherIndexName = nameof(AnotherIndexName);

        // arrange
        var analyzerSelector = new FakeAnalyzerSelector(AnotherIndexName);
        var provider = new AnalyzerProvider([analyzerSelector]);

        // act
        var actual = provider.GetAnalyzer(IndexName);

        // assert
        Assert.IsType<StandardAnalyzer>(actual);
    }
}
