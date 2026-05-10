using Microsoft.Extensions.Options;
using Nameless.IO.FileSystem;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.IO.FileSystem;

[IntegrationTest]
public class FileWrapperExtendedTests : IDisposable {
    private readonly string _root;

    public FileWrapperExtendedTests() {
        _root = Path.Combine(Path.GetTempPath(), $"nameless-fw-ext-{Guid.NewGuid():N}");
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

    private string CreateTempFile(string fileName, string content = "content") {
        var path = Path.Combine(_root, fileName);
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    public void LastWriteTime_ReturnsValidUtcTime() {
        // arrange
        CreateTempFile("last-write.txt");
        var sut = CreateWrapper("last-write.txt");

        // act
        var actual = sut.LastWriteTime;

        // assert — the write time should be a plausible recent UTC timestamp
        Assert.True(actual > DateTime.UtcNow.AddMinutes(-5));
        Assert.Equal(DateTimeKind.Utc, actual.Kind);
    }

    [Fact]
    public void Copy_WithOverwriteFalse_CreatesFileAtDestination() {
        // arrange
        const string sourceFile = "copy-source.txt";
        const string destFile = "copy-dest.txt";

        CreateTempFile(sourceFile, "copy content");
        var sut = CreateWrapper(sourceFile);

        // act
        var copy = sut.Copy(destFile, overwrite: false);

        // assert
        Assert.True(copy.Exists);
        Assert.Equal(Path.Combine(_root, destFile), copy.Path);
    }

    [Fact]
    public void Copy_WithOverwriteTrue_OverwritesExistingFile() {
        // arrange
        const string sourceFile = "over-source.txt";
        const string destFile = "over-dest.txt";

        CreateTempFile(sourceFile, "new content");
        CreateTempFile(destFile, "old content");
        var sut = CreateWrapper(sourceFile);

        // act
        var copy = sut.Copy(destFile, overwrite: true);

        // assert — the file at dest now contains the source content
        using var stream = copy.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(stream);
        var actual = reader.ReadToEnd();

        Assert.Equal("new content", actual);
    }

    [Fact]
    public void Copy_ReturnsFileWrapper_WithCorrectName() {
        // arrange
        const string sourceFile = "nm-source.txt";
        const string destFile = "nm-dest.txt";

        CreateTempFile(sourceFile);
        var sut = CreateWrapper(sourceFile);

        // act
        var copy = sut.Copy(destFile, overwrite: false);

        // assert
        Assert.Equal(destFile, copy.Name);
    }
}
