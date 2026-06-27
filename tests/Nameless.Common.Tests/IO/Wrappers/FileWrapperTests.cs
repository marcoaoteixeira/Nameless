using Microsoft.Extensions.Options;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.IO.Wrappers;

public class FileWrapperTests : IDisposable {
    private readonly string _root;

    public FileWrapperTests() {
        _root = Path.Combine(Path.GetTempPath(), $"nameless-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_root);
    }

    public void Dispose() {
        if (Directory.Exists(_root)) {
            Directory.Delete(_root, recursive: true);
        }
    }

    private IOptions<FileSystemProviderOptions> CreateOptions() {
        return Options.Create(new FileSystemProviderOptions {
            Root = _root,
            AllowOperationOutsideRoot = false
        });
    }

    private FileWrapper CreateWrapper(string fileName) {
        var info = new FileInfo(Path.Combine(_root, fileName));
        return new FileWrapper(info, CreateOptions());
    }

    private string CreateTempFile(string fileName, string content = "test") {
        var path = Path.Combine(_root, fileName);
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    [IntegrationTest]
    public void Exists_NonexistentFile_ReturnsFalse() {
        // arrange
        var sut = CreateWrapper("does-not-exist.txt");

        // act
        var actual = sut.Exists;

        // assert
        Assert.False(actual);
    }

    [Fact]
    [IntegrationTest]
    public void Exists_ExistingFile_ReturnsTrue() {
        // arrange
        CreateTempFile("exists.txt");
        var sut = CreateWrapper("exists.txt");

        // act
        var actual = sut.Exists;

        // assert
        Assert.True(actual);
    }

    [Fact]
    [IntegrationTest]
    public void Name_ReturnsFileName() {
        // arrange
        const string fileName = "named-file.txt";
        var sut = CreateWrapper(fileName);

        // act
        var actual = sut.Name;

        // assert
        Assert.Equal(fileName, actual);
    }

    [Fact]
    [IntegrationTest]
    public void Path_ReturnsFullPath() {
        // arrange
        const string fileName = "path-file.txt";
        var expected = System.IO.Path.Combine(_root, fileName);
        var sut = CreateWrapper(fileName);

        // act
        var actual = sut.Path;

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    [IntegrationTest]
    public void Delete_ExistingFile_RemovesFile() {
        // arrange
        var filePath = CreateTempFile("to-delete.txt");
        var sut = CreateWrapper("to-delete.txt");

        // act
        sut.Delete();

        // assert
        Assert.False(File.Exists(filePath));
    }

    [Fact]
    [IntegrationTest]
    public void Open_ExistingFile_ReturnsReadableStream() {
        // arrange
        const string content = "hello world";
        CreateTempFile("readable.txt", content);
        var sut = CreateWrapper("readable.txt");

        // act
        using var stream = sut.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(stream);
        var actual = reader.ReadToEnd();

        // assert
        Assert.Equal(content, actual);
    }
}
