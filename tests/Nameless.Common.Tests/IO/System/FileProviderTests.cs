using Microsoft.Extensions.Options;

namespace Nameless.IO.System;

public class FileProviderTests : IDisposable {
    private readonly string _root;

    public FileProviderTests() {
        _root = SysPath.Combine(SysPath.GetTempPath(), $"nameless-test-{Guid.NewGuid():N}");
        SysDirectory.CreateDirectory(_root);
    }

    public void Dispose() {
        if (SysDirectory.Exists(_root)) {
            SysDirectory.Delete(_root, recursive: true);
        }
    }

    private FileProvider CreateSut(bool allowOutsideRoot = false) {
        var options = Options.Create(new FileProviderOptions {
            Root = _root,
            AllowOperationOutsideRoot = allowOutsideRoot
        });

        return new FileProvider(options);
    }

    [Fact]
    [IntegrationTest]
    public void GetDirectory_WithRelativePath_ReturnsDirectoryWithCorrectPath() {
        // arrange
        var sut = CreateSut();
        const string RelativePath = "subdir";
        var expected = SysPath.Combine(_root, RelativePath);

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
        var expected = SysPath.Combine(_root, RelativePath);

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
        var expected = SysPath.GetFullPath(RelativePath, _root);

        // act
        var actual = sut.GetFullPath(RelativePath);

        // assert
        Assert.Equal(expected, actual);
    }
}
