namespace Nameless.IO.Monitoring;

/// <summary>
///     Base type for events that concern a single file.
/// </summary>
public abstract record FileEvent {
    /// <summary>
    ///     Gets the folder that contains the file.
    /// </summary>
    public string Directory => SysPath.GetDirectoryName(CurrentPath) ?? string.Empty;

    /// <summary>
    ///     Gets the file name, including extension.
    /// </summary>
    public string Name => SysPath.GetFileName(CurrentPath);

    /// <summary>
    ///     The full path of the file.
    /// </summary>
    public string CurrentPath { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="FileEvent"/> class.
    /// </summary>
    /// <param name="currentPath">
    ///     The full currentPath of the file.
    /// </param>
    protected FileEvent(string currentPath) {
        CurrentPath = currentPath;
    }
}

/// <summary>
///     A file was created and is available for exclusive access.
/// </summary>
public sealed record FileCreatedEvent : FileEvent {
    /// <summary>
    ///     Initializes a new instance of <see cref="FileCreatedEvent"/> class.
    /// </summary>
    /// <param name="currentPath">
    ///     The full currentPath of the file.
    /// </param>
    public FileCreatedEvent(string currentPath) : base(currentPath) { }
}

/// <summary>
///     A file changed and the changes settled.
/// </summary>
public sealed record FileChangedEvent : FileEvent {
    /// <summary>
    ///     Initializes a new instance of <see cref="FileChangedEvent"/> class.
    /// </summary>
    /// <param name="currentPath">
    ///     The full currentPath of the file.
    /// </param>
    public FileChangedEvent(string currentPath) : base(currentPath) { }
}

/// <summary>
///     A file was deleted.
/// </summary>
public sealed record FileDeletedEvent : FileEvent {
    /// <summary>
    ///     Initializes a new instance of <see cref="FileDeletedEvent"/> class.
    /// </summary>
    /// <param name="currentPath">
    ///     The full currentPath of the file.
    /// </param>
    public FileDeletedEvent(string currentPath) : base(currentPath) { }
}

/// <summary>
/// A file was renamed or moved inside the monitored root.
/// </summary>
public sealed record FileRenamedEvent : FileEvent {
    /// <summary>
    /// Gets the file name before the rename, including extension.
    /// </summary>
    public string PreviousName => SysPath.GetFileName(PreviousPath);

    /// <summary>The full path before the rename.</summary>
    public string PreviousPath { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="FileRenamedEvent"/> class.
    /// </summary>
    /// <param name="previousPath">
    ///     The full path before the rename.
    /// </param>
    /// <param name="currentPath">
    ///     The full path after the rename.
    /// </param>
    public FileRenamedEvent(string previousPath, string currentPath)
        : base(currentPath) { PreviousPath = previousPath; }
}

/// <summary>
///     The monitoring failed or could not complete an operation.
/// </summary>
public sealed record FileMonitorErrorEvent {
    /// <summary>The failure.</summary>
    public Exception Exception { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="FileMonitorErrorEvent"/>
    ///     class.
    /// </summary>
    /// <param name="exception">
    ///     The failure.
    /// </param>
    public FileMonitorErrorEvent(Exception exception) {
        Exception = exception;
    }
}
