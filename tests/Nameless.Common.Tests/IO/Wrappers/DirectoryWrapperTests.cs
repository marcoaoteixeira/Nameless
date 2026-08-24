using Microsoft.Extensions.Options;
using Nameless.IO.Explorer;
using Nameless.IO.Explorer.Wrappers;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.IO.Wrappers;

public class DirectoryWrapperTests : IDisposable {
    private readonly string _root;

    public DirectoryWrapperTests() {
        _root = Path.Combine(Path.GetTempPath(), $"nameless-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_root);
    }

    public void Dispose() {
        if (Directory.Exists(_root)) {
            Directory.Delete(_root, recursive: true);
        }
    }

    private IOptions<FileExplorerOptions> CreateOptions() {
        return Options.Create(new FileExplorerOptions {
            Root = _root,
            AllowOperationOutsideRoot = false
        });
    }

    private DirectoryWrapper CreateWrapper(string dirName) {
        var info = new DirectoryInfo(Path.Combine(_root, dirName));
        return new DirectoryWrapper(info, CreateOptions());
    }

    [Fact]
    [IntegrationTest]
    public void Exists_NonexistentDirectory_ReturnsFalse() {
        // arrange
        var sut = CreateWrapper("nonexistent");

        // act
        var actual = sut.Exists;

        // assert
        Assert.False(actual);
    }

    [Fact]
    [IntegrationTest]
    public void Exists_ExistingDirectory_ReturnsTrue() {
        // arrange
        Directory.CreateDirectory(Path.Combine(_root, "existing"));
        var sut = CreateWrapper("existing");

        // act
        var actual = sut.Exists;

        // assert
        Assert.True(actual);
    }

    [Fact]
    [IntegrationTest]
    public void Create_CreatesDirectoryOnDisk() {
        // arrange
        var dirPath = Path.Combine(_root, "to-create");
        var sut = CreateWrapper("to-create");

        // act
        sut.Create();

        // assert
        Assert.True(Directory.Exists(dirPath));
    }

    [Fact]
    [IntegrationTest]
    public void Delete_ExistingDirectory_RemovesIt() {
        // arrange
        var dirPath = Path.Combine(_root, "to-delete");
        Directory.CreateDirectory(dirPath);
        var sut = CreateWrapper("to-delete");

        // act
        sut.Delete(recursive: false);

        // assert
        Assert.False(Directory.Exists(dirPath));
    }

    [Fact]
    [IntegrationTest]
    public void GetFiles_WithExistingFiles_ReturnsThem() {
        // arrange
        var subDir = Path.Combine(_root, "with-files");
        Directory.CreateDirectory(subDir);
        File.WriteAllText(Path.Combine(subDir, "a.txt"), "a");
        File.WriteAllText(Path.Combine(subDir, "b.txt"), "b");

        var options = Options.Create(new FileExplorerOptions {
            Root = _root,
            AllowOperationOutsideRoot = false
        });
        var info = new DirectoryInfo(subDir);
        var sut = new DirectoryWrapper(info, options);

        // act
        var files = sut.GetFiles("*.txt", recursive: false).ToList();

        // assert
        Assert.Equal(2, files.Count);
    }

    [Fact]
    [IntegrationTest]
    public void GetDirectories_WithSubdirectories_ReturnsThem() {
        // arrange
        var parentDir = Path.Combine(_root, "parent");
        Directory.CreateDirectory(Path.Combine(parentDir, "child1"));
        Directory.CreateDirectory(Path.Combine(parentDir, "child2"));

        var options = Options.Create(new FileExplorerOptions {
            Root = _root,
            AllowOperationOutsideRoot = false
        });
        var info = new DirectoryInfo(parentDir);
        var sut = new DirectoryWrapper(info, options);

        // act
        var dirs = sut.GetDirectories("*", recursive: false).ToList();

        // assert
        Assert.Equal(2, dirs.Count);
    }
}
