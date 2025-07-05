using Lucene.Net.Analysis.Standard;
using Microsoft.Extensions.Options;
using Nameless.Search.Lucene.Fixtures;
using Nameless.Search.Lucene.Mockers;

namespace Nameless.Search.Lucene;

public class AnalyzerProviderTests {
    [Fact]
    public void WhenGetAnalyzer_ThenReturnsAnalyzerForIndex() {
        // arrange
        const string IndexName = "48574ce1-1c24-4727-8d74-9dc8f01bf98c";
        var searchOptions = new SearchOptions();
        searchOptions.RegisterAnalyzerSelector(new AnalyzerSelectorMocker().WithAnalyzerFor(IndexName).Build());
        var options = Options.Create(searchOptions);
        var sut = new AnalyzerProvider(options);

        // act
        var actual = sut.GetAnalyzer(IndexName);

        // assert
        Assert.NotNull(actual);
    }

    [Fact]
    public void WhenGetAnalyzer_IfNoAnalyzerFound_ThenReturnsDefaultStandardAnalyzer() {
        // arrange
        const string IndexName = "48574ce1-1c24-4727-8d74-9dc8f01bf98c";
        var sut = new AnalyzerProvider(Options.Create(new SearchOptions()));

        // act
        var actual = sut.GetAnalyzer(IndexName);

        // assert
        Assert.NotNull(actual);
        Assert.IsType<StandardAnalyzer>(actual);
    }

    [Fact]
    public void WhenGetAnalyzer_WithMoreThanOneAnalyzer_ThenReturnsPrioritizeAnalyzer() {
        // arrange
        const string IndexName = "48574ce1-1c24-4727-8d74-9dc8f01bf98c";
        const int LowestPriority = 1;
        const int MediumPriority = 5;
        const int HighestPriority = 10;
        var selectors = new[] {
            new AnalyzerSelectorMocker().WithAnalyzerFor(IndexName, new FakeAnalyzer(MediumPriority), MediumPriority)
                                        .Build(),
            new AnalyzerSelectorMocker().WithAnalyzerFor(IndexName, new FakeAnalyzer(LowestPriority), LowestPriority)
                                        .Build(),
            new AnalyzerSelectorMocker().WithAnalyzerFor(IndexName, new FakeAnalyzer(HighestPriority), HighestPriority)
                                        .Build()
        };
        var searchOptions = new SearchOptions();
        searchOptions.RegisterAnalyzerSelector(selectors[0]);
        searchOptions.RegisterAnalyzerSelector(selectors[1]);
        searchOptions.RegisterAnalyzerSelector(selectors[2]);

        var sut = new AnalyzerProvider(Options.Create(searchOptions));

        // act
        var actual = sut.GetAnalyzer(IndexName);

        // assert
        Assert.NotNull(actual);
        Assert.IsType<FakeAnalyzer>(actual);
        Assert.Equal(HighestPriority, ((FakeAnalyzer)actual).Priority);
    }
}