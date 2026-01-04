using Lucene.Net.Analysis.Standard;
using Nameless.Lucene.Fixtures;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene;

[UnitTest]
public class AnalyzerProviderTests {
    [Fact]
    public void WhenConstructing_WhenOptionsIsNotNull_ThenReturnsNewInstance() {
        // arrange
        var options = OptionsHelper.Create<LuceneOptions>();

        // act
        var actual = Record.Exception(() => new AnalyzerProvider(options));

        // assert
        Assert.Null(actual);
    }

    [Fact]
    public void WhenGettingAnalyzer_WhenOptionsDoesNotProviderAnalyzerSelectorForIndex_ThenReturnsDefaultAnalyzer() {
        const string IndexName = nameof(IndexName);

        // arrange
        var options = OptionsHelper.Create<LuceneOptions>();
        var provider = new AnalyzerProvider(options);

        // act
        var actual = provider.GetAnalyzer(IndexName);

        // assert
        Assert.IsType<StandardAnalyzer>(actual);
    }

    [Fact]
    public void WhenGettingAnalyzer_WhenOptionsProvidersAnalyzerSelectorForIndex_ThenReturnsSpecificAnalyzer() {
        const string IndexName = nameof(IndexName);

        // arrange
        var analyzerSelector = new FakeAnalyzerSelector(IndexName);
        var options = OptionsHelper.Create<LuceneOptions>(opts => {
            opts.RegisterAnalyzerSelector(analyzerSelector);
        });
        var provider = new AnalyzerProvider(options);

        // act
        var actual = provider.GetAnalyzer(IndexName);

        // assert
        Assert.IsType<FakeAnalyzer>(actual);
    }

    [Fact]
    public void WhenGettingAnalyzer_WhenOptionsProvidersAnalyzerSelector_ButNotForSpecifiedIndex_ThenReturnsDefaultAnalyzer() {
        const string IndexName = nameof(IndexName);
        const string AnotherIndexName = nameof(AnotherIndexName);

        // arrange
        var analyzerSelector = new FakeAnalyzerSelector(AnotherIndexName);
        var options = OptionsHelper.Create<LuceneOptions>(opts => {
            opts.RegisterAnalyzerSelector(analyzerSelector);
        });

        var provider = new AnalyzerProvider(options);

        // act
        var actual = provider.GetAnalyzer(IndexName);

        // assert
        Assert.IsType<StandardAnalyzer>(actual);
    }
}
