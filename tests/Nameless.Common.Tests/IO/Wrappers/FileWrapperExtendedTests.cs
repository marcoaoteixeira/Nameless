using Nameless.IO.Explorer;
using Nameless.IO.Explorer.Wrappers;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.IO.Wrappers;

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

    private FileExplorerOptions CreateOptions() {
        return new FileExplorerOptions {
            Root = _root,
            AllowOperationOutsideRoot = false
        };
    }

    private FileWrapper CreateWrapper(string fileName) {
        var info = new FileInfo(Path.Combine(_root, fileName));
        return new FileWrapper(info, CreateOptions());
    }

    private void CreateTempFile(string fileName, string content = "content") {
        var path = Path.Combine(_root, fileName);
        File.WriteAllText(path, content);
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
        const string SourceFile = "copy-source.txt";
        const string DestFile = "copy-dest.txt";

        CreateTempFile(SourceFile, "copy content");
        var sut = CreateWrapper(SourceFile);

        // act
        var copy = sut.Copy(DestFile, overwrite: false);

        // assert
        Assert.True(copy.Exists);
        Assert.Equal(Path.Combine(_root, DestFile), copy.Path);
    }

    [Fact]
    public void Copy_WithOverwriteTrue_OverwritesExistingFile() {
        // arrange
        const string SourceFile = "over-source.txt";
        const string DestFile = "over-dest.txt";

        CreateTempFile(SourceFile, "new content");
        CreateTempFile(DestFile, "old content");
        var sut = CreateWrapper(SourceFile);

        // act
        var copy = sut.Copy(DestFile, overwrite: true);

        // assert — the file at dest now contains the source content
        using var stream = copy.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(stream);
        var actual = reader.ReadToEnd();

        Assert.Equal("new content", actual);
    }

    [Fact]
    public void Copy_ReturnsFileWrapper_WithCorrectName() {
        // arrange
        const string SourceFile = "nm-source.txt";
        const string DestFile = "nm-dest.txt";

        CreateTempFile(SourceFile);
        var sut = CreateWrapper(SourceFile);

        // act
        var copy = sut.Copy(DestFile, overwrite: false);

        // assert
        Assert.Equal(DestFile, copy.Name);
    }
}
