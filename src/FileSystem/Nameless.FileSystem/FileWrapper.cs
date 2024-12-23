namespace Nameless.FileSystem;

internal sealed class FileWrapper : IFile {
    private readonly FileInfo _file;

    string IFile.Path => _file.FullName;
    
    string IFile.Name => _file.Name;
    
    bool IFile.Exists => _file.Exists;
    
    DateTime IFile.CreatedAt => _file.CreationTimeUtc;

    DateTime IFile.LastWriteAt => _file.LastWriteTimeUtc;

    internal FileWrapper(FileInfo file) {
        _file = Prevent.Argument.Null(file);
    }
}
