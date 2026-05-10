using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Moq;
using Nameless.Lucene;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene;

[UnitTest]
public class AnalyzerProviderTests {
    [Fact]
    public void GetAnalyzer_WithNoSelectors_ReturnsStandardAnalyzer() {
        // Arrange
        var sut = new AnalyzerProvider([]);

        // Act
        var analyzer = sut.GetAnalyzer("test-index");

        // Assert
        Assert.IsType<StandardAnalyzer>(analyzer);
    }

    [Fact]
    public void GetAnalyzer_WithOneSelector_ReturnsSelectedAnalyzer() {
        // Arrange
        var expected = new StandardAnalyzer(LuceneDefaults.Version);
        var selector = new Mock<IAnalyzerSelector>();
        selector
            .Setup(s => s.GetAnalyzer(It.IsAny<string>()))
            .Returns(new AnalyzerSelectorResult(expected, Priority: 10));

        var sut = new AnalyzerProvider([selector.Object]);

        // Act
        var actual = sut.GetAnalyzer("test-index");

        // Assert
        Assert.Same(expected, actual);
    }

    [Fact]
    public void GetAnalyzer_WithMultipleSelectors_UsesHighestPriority() {
        // Arrange
        var lowPriorityAnalyzer = new StandardAnalyzer(LuceneDefaults.Version);
        var highPriorityAnalyzer = new StandardAnalyzer(LuceneDefaults.Version);

        var lowPriority = new Mock<IAnalyzerSelector>();
        lowPriority
            .Setup(s => s.GetAnalyzer(It.IsAny<string>()))
            .Returns(new AnalyzerSelectorResult(lowPriorityAnalyzer, Priority: 1));

        var highPriority = new Mock<IAnalyzerSelector>();
        highPriority
            .Setup(s => s.GetAnalyzer(It.IsAny<string>()))
            .Returns(new AnalyzerSelectorResult(highPriorityAnalyzer, Priority: 99));

        var sut = new AnalyzerProvider([lowPriority.Object, highPriority.Object]);

        // Act
        var actual = sut.GetAnalyzer("test-index");

        // Assert
        Assert.Same(highPriorityAnalyzer, actual);
    }
}
