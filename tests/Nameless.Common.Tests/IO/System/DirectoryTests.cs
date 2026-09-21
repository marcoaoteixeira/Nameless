namespace Nameless.IO.System;

public class DirectoryTests : IDisposable {
    private readonly string _root;

    public DirectoryTests() {
        _root = SysPath.Combine(SysPath.GetTempPath(), $"nameless-test-{Guid.NewGuid():N}");
        SysDirectory.CreateDirectory(_root);
    }

    public void Dispose() {
        if (SysDirectory.Exists(_root)) {
            SysDirectory.Delete(_root, recursive: true);
        }
    }

    private FileProviderOptions CreateOptions() {
        return new FileProviderOptions {
            Root = _root,
            AllowOperationOutsideRoot = false
        };
    }

    private Directory CreateWrapper(string dirName) {
        var info = new DirectoryInfo(SysPath.Combine(_root, dirName));
        return new Directory(info, CreateOptions());
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
        SysDirectory.CreateDirectory(SysPath.Combine(_root, "existing"));
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
        var dirPath = SysPath.Combine(_root, "to-create");
        var sut = CreateWrapper("to-create");

        // act
        sut.Create();

        // assert
        Assert.True(SysDirectory.Exists(dirPath));
    }

    [Fact]
    [IntegrationTest]
    public void Delete_ExistingDirectory_RemovesIt() {
        // arrange
        var dirPath = SysPath.Combine(_root, "to-delete");
        SysDirectory.CreateDirectory(dirPath);
        var sut = CreateWrapper("to-delete");

        // act
        sut.Delete(recursive: false);

        // assert
        Assert.False(SysDirectory.Exists(dirPath));
    }

    [Fact]
    [IntegrationTest]
    public void GetFiles_WithExistingFiles_ReturnsThem() {
        // arrange
        var subDir = SysPath.Combine(_root, "with-files");
        SysDirectory.CreateDirectory(subDir);
        SysFile.WriteAllText(SysPath.Combine(subDir, "a.txt"), "a");
        SysFile.WriteAllText(SysPath.Combine(subDir, "b.txt"), "b");

        var options = new FileProviderOptions {
            Root = _root,
            AllowOperationOutsideRoot = false
        };
        var info = new DirectoryInfo(subDir);
        var sut = new Directory(info, options);

        // act
        var files = sut.GetFiles("*.txt", recursive: false).ToList();

        // assert
        Assert.Equal(2, files.Count);
    }

    [Fact]
    [IntegrationTest]
    public void GetDirectories_WithSubdirectories_ReturnsThem() {
        // arrange
        var parentDir = SysPath.Combine(_root, "parent");
        SysDirectory.CreateDirectory(SysPath.Combine(parentDir, "child1"));
        SysDirectory.CreateDirectory(SysPath.Combine(parentDir, "child2"));

        var options = new FileProviderOptions {
            Root = _root,
            AllowOperationOutsideRoot = false
        };
        var info = new DirectoryInfo(parentDir);
        var sut = new Directory(info, options);

        // act
        var dirs = sut.GetDirectories("*", recursive: false).ToList();

        // assert
        Assert.Equal(2, dirs.Count);
    }
}
