namespace Nameless.FileSystem;

internal sealed class DirectoryWrapper : IDirectory {
    private readonly DirectoryInfo _directory;

    string IDirectory.Path => _directory.FullName;

    internal DirectoryWrapper(DirectoryInfo directory) {
        _directory = Prevent.Argument.Null(directory);
    }
}