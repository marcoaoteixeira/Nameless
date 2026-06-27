using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.IO;

/// <summary>
///     Defines the processing logic to run once a file change is detected.
///     Implement this interface to plug your own business logic into
///     <see cref="FileProcessor"/>.
/// </summary>
public interface IFileHandler {
    /// <summary>
    ///     Called when a directory change is detected over a file.
    /// </summary>
    /// <param name="file">
    ///     The file object.
    /// </param>
    /// <param name="evt">
    ///     The event that triggered the handler.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the asynchronous
    ///     operation execution. Where the result is a
    ///     <see cref="FileHandlerResult"/> object that can carry information
    ///     about errors during processing.
    /// </returns>
    Task<FileHandlerResult> HandleAsync(IFile file, FileEvent evt, CancellationToken cancellationToken);
}

/// <summary>
///     Represents the multiple file events.
/// </summary>
public enum FileEvent {
    /// <summary>
    ///     When a file is created.
    /// </summary>
    Create,

    /// <summary>
    ///     When a file changes.
    /// </summary>
    Change,

    /// <summary>
    ///     When a file is renamed.
    /// </summary>
    Rename,

    /// <summary>
    ///     Then a file is deleted.
    /// </summary>
    Delete,

    /// <summary>
    ///     When an error occurs.
    /// </summary>
    Error
}

/// <summary>
///     Represents the <see cref="IFileHandler"/> execution result.
/// </summary>
public class FileHandlerResult : Result {
    /// <summary>
    ///     Represents a result without errors.
    /// </summary>
    public static FileHandlerResult OK => new(errors: []);

    private FileHandlerResult(Error[] errors)
        : base(errors) { }

    /// <summary>
    ///     Creates a new instance with the specified errors.
    /// </summary>
    /// <param name="errors">
    ///     The errors.
    /// </param>
    public static implicit operator FileHandlerResult(Error[] errors) {
        return new FileHandlerResult(errors);
    }

    /// <summary>
    ///     Creates a new instance with the specified error.
    /// </summary>
    /// <param name="error">
    ///     The error.
    /// </param>
    public static implicit operator FileHandlerResult(Error error) {
        return new FileHandlerResult([error]);
    }
}

/// <summary>
///     Abstracts the OS-level file system watcher so that file watching
///     behaviour can be substituted or mocked in tests without touching
///     the real file system.
/// </summary>
public interface IFileSystemWatcher : IDisposable {
    event FileSystemEventHandler? Created;
}