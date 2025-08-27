using Microsoft.Extensions.Options;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.IO.FileSystem;

[UnitTest]
public class DirectoryWrapperTests {
    private static IOptions<FileSystemOptions> CreateOptions(string root = null, bool allowOperationOutsideRoot = false) {
        return OptionsHelper.Create<FileSystemOptions>(opts => {
            opts.Root = root ?? typeof(FileWrapperTests).Assembly.GetDirectoryPath();
            opts.AllowOperationOutsideRoot = allowOperationOutsideRoot;
        });
    }

    [Fact]
    public void WhenGettingName_ThenReturnsDirectoryNameFromUnderlyingDirectory() {
        // arrange
        var directoryPath = Path.Combine(typeof(DirectoryWrapperTests).Assembly.GetDirectoryPath(), nameof(DirectoryWrapper));
        var directory = new DirectoryInfo(directoryPath);
        var sut = new DirectoryWrapper(directory, CreateOptions());

        // act
        var actual = sut.Name;

        // assert
        Assert.Equal(directory.Name, actual);
    }

    [Fact]
    public void WhenGettingPath_ThenReturnsDirectoryFullPathFromUnderlyingDirectory() {
        // arrange
        var directoryPath = Path.Combine(typeof(DirectoryWrapperTests).Assembly.GetDirectoryPath(), nameof(DirectoryWrapper));
        var directory = new DirectoryInfo(directoryPath);
        var sut = new DirectoryWrapper(directory, CreateOptions());

        // act
        var actual = sut.Path;

        // assert
        Assert.Equal(directory.FullName, actual);
    }

    [Fact]
    public void WhenCheckingExistence_WhenDirectoryExists_ThenReturnsTrue() {
        // arrange
        var directoryPath = typeof(DirectoryWrapperTests).Assembly.GetDirectoryPath();
        var directory = new DirectoryInfo(directoryPath);
        var sut = new DirectoryWrapper(directory, CreateOptions());

        // act
        var actual = sut.Exists;

        // assert
        Assert.True(actual);
    }

    [Fact]
    public void WhenCheckingExistence_WhenDirectoryDoesNotExist_ThenReturnsFalse() {
        // arrange
        var directoryPath = Path.Combine(typeof(DirectoryWrapperTests).Assembly.GetDirectoryPath(), $"{Guid.NewGuid():N}");
        var directory = new DirectoryInfo(directoryPath);
        var sut = new DirectoryWrapper(directory, CreateOptions());

        // act
        var actual = sut.Exists;

        // assert
        Assert.False(actual);
    }

    [Fact]
    public void WhenCreatingDirectory_ThenReturnsNewDirectoryFullPath() {
        // arrange
        var directoryPath = Path.Combine(typeof(DirectoryWrapperTests).Assembly.GetDirectoryPath(), nameof(DirectoryWrapper));
        var directory = new DirectoryInfo(directoryPath);
        var sut = new DirectoryWrapper(directory, CreateOptions());

        // act
        var actual = sut.Create();

        // assert
        Assert.Equal(directoryPath, actual);
    }

    [Fact]
    public void WhenCreatingDirectory_WhenAllowOperationOutsideRootIsFalse_WhenUnderlyingDirectoryIsOutsideRoot_ThenThrowsException() {
        // arrange
        var rootPath = Path.Combine(typeof(DirectoryWrapperTests).Assembly.GetDirectoryPath(), nameof(DirectoryWrapper));
        var options = CreateOptions(root: rootPath, allowOperationOutsideRoot: false);

        var directoryPath = typeof(DirectoryWrapperTests).Assembly.GetDirectoryPath();
        var directory = new DirectoryInfo(directoryPath);

        // act
        var actual = Record.Exception(() => new DirectoryWrapper(directory, options));

        // assert
        Assert.IsType<UnauthorizedAccessException>(actual);
    }

    [Fact]
    public void WhenCreatingDirectory_WhenAllowOperationOutsideRootIsTrue_WhenUnderlyingDirectoryIsOutsideRoot_ThenDoNotThrows() {
        // arrange
        var rootPath = Path.Combine(typeof(DirectoryWrapperTests).Assembly.GetDirectoryPath(), nameof(DirectoryWrapper));
        var options = CreateOptions(root: rootPath, allowOperationOutsideRoot: true);

        var directoryPath = typeof(DirectoryWrapperTests).Assembly.GetDirectoryPath();
        var directory = new DirectoryInfo(directoryPath);

        // act
        var actual = Record.Exception(() => new DirectoryWrapper(directory, options));

        // assert
        Assert.Null(actual);
    }
}
