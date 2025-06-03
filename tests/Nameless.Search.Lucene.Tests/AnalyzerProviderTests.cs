using Lucene.Net.Analysis.Standard;
using Nameless.Search.Lucene.Fixtures;
using Nameless.Search.Lucene.Mockers;

namespace Nameless.Search.Lucene;

public class AnalyzerProviderTests {
    [Fact]
    public void WhenGetAnalyzer_ThenReturnsAnalyzerForIndex() {
        // arrange
        const string indexName = "48574ce1-1c24-4727-8d74-9dc8f01bf98c";
        var selectors = new[] { new AnalyzerSelectorMocker().WithAnalyzerFor(indexName).Build() };
        var sut = new AnalyzerProvider(selectors);

        // act
        var actual = sut.GetAnalyzer(indexName);

        // assert
        Assert.That(actual, Is.Not.Null);
    }

    [Fact]
    public void WhenGetAnalyzer_IfNoAnalyzerFound_ThenReturnsDefaultStandardAnalyzer() {
        // arrange
        const string indexName = "48574ce1-1c24-4727-8d74-9dc8f01bf98c";
        var selectors = Array.Empty<IAnalyzerSelector>();
        var sut = new AnalyzerProvider(selectors);

        // act
        var actual = sut.GetAnalyzer(indexName);

        // assert
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual, Is.InstanceOf<StandardAnalyzer>());
    }

    [Fact]
    public void WhenGetAnalyzer_WithMoreThanOneAnalyzer_ThenReturnsPrioritizeAnalyzer() {
        // arrange
        const string indexName = "48574ce1-1c24-4727-8d74-9dc8f01bf98c";
        const int lowestPriority = 1;
        const int mediumPriority = 5;
        const int highestPriority = 10;
        var selectors = new[] {
            new AnalyzerSelectorMocker().WithAnalyzerFor(indexName, new FakeAnalyzer(mediumPriority), mediumPriority)
                                        .Build(),
            new AnalyzerSelectorMocker().WithAnalyzerFor(indexName, new FakeAnalyzer(lowestPriority), lowestPriority)
                                        .Build(),
            new AnalyzerSelectorMocker().WithAnalyzerFor(indexName, new FakeAnalyzer(highestPriority), highestPriority)
                                        .Build()
        };
        var sut = new AnalyzerProvider(selectors);

        // act
        var actual = sut.GetAnalyzer(indexName);

        // assert
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual, Is.InstanceOf<FakeAnalyzer>());
        Assert.That(((FakeAnalyzer)actual).Priority, Is.EqualTo(highestPriority));
    }
}