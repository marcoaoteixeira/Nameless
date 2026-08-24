using Microsoft.Extensions.Options;
using Nameless.IO.Explorer;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.IO;

public class FileSystemProviderTests : IDisposable {
    private readonly string _root;

    public FileSystemProviderTests() {
        _root = Path.Combine(Path.GetTempPath(), $"nameless-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_root);
    }

    public void Dispose() {
        if (Directory.Exists(_root)) {
            Directory.Delete(_root, recursive: true);
        }
    }

    private FileExplorer CreateSut(bool allowOutsideRoot = false) {
        var options = Options.Create(new FileExplorerOptions {
            Root = _root,
            AllowOperationOutsideRoot = allowOutsideRoot
        });

        return new FileExplorer(options);
    }

    [Fact]
    [IntegrationTest]
    public void GetDirectory_WithRelativePath_ReturnsDirectoryWithCorrectPath() {
        // arrange
        var sut = CreateSut();
        const string RelativePath = "subdir";
        var expected = Path.Combine(_root, RelativePath);

        // act
        var directory = sut.GetDirectory(RelativePath);

        // assert
        Assert.Equal(expected, directory.Path);
    }

    [Fact]
    [IntegrationTest]
    public void GetFile_WithRelativePath_ReturnsFileWithCorrectPath() {
        // arrange
        var sut = CreateSut();
        const string RelativePath = "test.txt";
        var expected = Path.Combine(_root, RelativePath);

        // act
        var file = sut.GetFile(RelativePath);

        // assert
        Assert.Equal(expected, file.Path);
    }

    [Fact]
    [IntegrationTest]
    public void GetFullPath_WithRelativePath_ReturnsAbsolutePath() {
        // arrange
        var sut = CreateSut();
        const string RelativePath = "some/nested/file.txt";
        var expected = Path.GetFullPath(RelativePath, _root);

        // act
        var actual = sut.GetFullPath(RelativePath);

        // assert
        Assert.Equal(expected, actual);
    }
}
