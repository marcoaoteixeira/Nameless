using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Lucene.Fixtures;
using Nameless.Lucene.Mockers;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Helpers;
using Nameless.Testing.Tools.Mockers.IO;

namespace Nameless.Lucene;

[UnitTest]
public class IndexProviderTests {
    [Fact]
    public void WhenGettingIndex_WhenIndexDoesNotExists_ThenCreatesAndReturns() {
        // arrange
        var analyzerProviderMocker = new AnalyzerProviderMocker().WithGetAnalyzer(new FakeAnalyzer());
        var fileSystem = new FileSystemMocker().Build();
        var sut = new IndexProvider(
            analyzerProviderMocker.Build(),
            fileSystem,
            OptionsHelper.Create<LuceneOptions>(),
            NullLogger<Index>.Instance
        );

        // act
        var actual = sut.Get("test");

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);

            analyzerProviderMocker.VerifyGetAnalyzer(1);
        });
    }

    [Fact]
    public void WhenGettingIndex_WhenCallMultipleTimes_ThenReturnsSameIndex() {
        // arrange
        var analyzerProviderMocker = new AnalyzerProviderMocker().WithGetAnalyzer(new FakeAnalyzer());
        var fileSystem = new FileSystemMocker().Build();
        ILogger<Index> logger = NullLogger<Index>.Instance;
        var sut = new IndexProvider(
            analyzerProviderMocker.Build(),
            fileSystem,
            OptionsHelper.Create<LuceneOptions>(),
            logger
        );

        // act
        var indexes = new[] {
            sut.Get("test"),
            sut.Get("test"),
            sut.Get("test")
        };

        // assert
        Assert.Multiple(() => {
            Assert.All(indexes, Assert.NotNull);

            analyzerProviderMocker.VerifyGetAnalyzer(1, exactly: true);
        });
    }
}
