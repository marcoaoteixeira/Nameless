using Microsoft.Extensions.Options;
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

    private FileSystemProvider CreateSut(bool allowOutsideRoot = false) {
        var options = Options.Create(new FileSystemProviderOptions {
            Root = _root,
            AllowOperationOutsideRoot = allowOutsideRoot
        });

        return new FileSystemProvider(options);
    }

    [Fact]
    [IntegrationTest]
    public void GetDirectory_WithRelativePath_ReturnsDirectoryWithCorrectPath() {
        // arrange
        var sut = CreateSut();
        const string relativePath = "subdir";
        var expected = Path.Combine(_root, relativePath);

        // act
        var directory = sut.GetDirectory(relativePath);

        // assert
        Assert.Equal(expected, directory.Path);
    }

    [Fact]
    [IntegrationTest]
    public void GetFile_WithRelativePath_ReturnsFileWithCorrectPath() {
        // arrange
        var sut = CreateSut();
        const string relativePath = "test.txt";
        var expected = Path.Combine(_root, relativePath);

        // act
        var file = sut.GetFile(relativePath);

        // assert
        Assert.Equal(expected, file.Path);
    }

    [Fact]
    [IntegrationTest]
    public void GetFullPath_WithRelativePath_ReturnsAbsolutePath() {
        // arrange
        var sut = CreateSut();
        const string relativePath = "some/nested/file.txt";
        var expected = Path.GetFullPath(relativePath, _root);

        // act
        var actual = sut.GetFullPath(relativePath);

        // assert
        Assert.Equal(expected, actual);
    }
}
