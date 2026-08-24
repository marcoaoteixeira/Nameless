using Microsoft.Extensions.Options;
using Moq;
using Nameless.IO;
using Nameless.IO.Explorer;
using Nameless.Lucene;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.IO;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Lucene;

[IntegrationTest]
public class IndexProviderTests : IDisposable {
    private readonly string _tempDir;
    private readonly IndexProvider _sut;

    public IndexProviderTests() {
        _tempDir = Path.Combine(Path.GetTempPath(), $"lucene-provider-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);

        _sut = CreateProvider(_tempDir);
    }

    public void Dispose() {
        _sut.Dispose();

        if (Directory.Exists(_tempDir)) {
            Directory.Delete(_tempDir, recursive: true);
        }
    }

    [Fact]
    public void Get_ReturnsSameIndexForSameName() {
        // Arrange
        const string IndexName = "shared-index";

        // Act
        var first = _sut.Get(IndexName);
        var second = _sut.Get(IndexName);

        // Assert
        Assert.Same(first, second);
    }

    [Fact]
    public void Get_ReturnsDifferentIndexForDifferentName() {
        // Arrange
        const string IndexNameA = "index-alpha";
        const string IndexNameB = "index-beta";

        // Act
        var alpha = _sut.Get(IndexNameA);
        var beta = _sut.Get(IndexNameB);

        // Assert
        Assert.NotSame(alpha, beta);
        Assert.Equal(IndexNameA, alpha.Name);
        Assert.Equal(IndexNameB, beta.Name);
    }

    [Fact]
    public void Dispose_DoesNotThrow() {
        // Arrange
        var providerDir = Path.Combine(Path.GetTempPath(), $"lucene-provider-dispose-{Guid.NewGuid():N}");
        Directory.CreateDirectory(providerDir);
        var provider = CreateProvider(providerDir);

        // Act
        var exception = Record.Exception(provider.Dispose);

        // Assert
        Assert.Null(exception);

        if (Directory.Exists(providerDir)) {
            Directory.Delete(providerDir, recursive: true);
        }
    }

    private static IndexProvider CreateProvider(string tempDir) {
        var options = Options.Create(new LuceneOptions { DirectoryName = tempDir });
        var logger = new LoggerMocker<Index>().WithAnyLogLevel().Build();

        var analyzerProvider = new Mock<IAnalyzerProvider>();
        analyzerProvider
            .Setup(ap => ap.GetAnalyzer(It.IsAny<string>()))
            .Returns(LuceneDefaults.Analyzer);

        // The file system provider must return a directory whose Path is a unique
        // sub-directory under tempDir for each call, so each index gets its own
        // directory.  We use a callback to materialise the correct path per call.
        var fileSystemMock = new Mock<IFileExplorer>();
        fileSystemMock
            .Setup(fs => fs.GetDirectory(It.IsAny<string>()))
            .Returns((string relativePath) => {
                var fullPath = Path.Combine(tempDir, relativePath);
                Directory.CreateDirectory(fullPath);

                var dirMock = new Mock<IDirectory>();
                dirMock.Setup(d => d.Path).Returns(fullPath);
                dirMock.Setup(d => d.Create()).Callback(() => Directory.CreateDirectory(fullPath));
                return dirMock.Object;
            });

        return new IndexProvider(
            analyzerProvider.Object,
            fileSystemMock.Object,
            options,
            logger
        );
    }
}
