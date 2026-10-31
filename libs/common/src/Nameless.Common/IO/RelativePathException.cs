namespace Nameless.IO;

/// <summary>
///     Resolve relative path exception.
/// </summary>
public class RelativePathException : Exception {
    /// <summary>
    ///     Gets the relative path it tried to resolve.
    /// </summary>
    public string RelativePath { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="RelativePathException"/> class.
    /// </summary>
    /// <param name="relativePath">
    ///     The relative path it tried to resolve.
    /// </param>
    public RelativePathException(string relativePath)
        : this(message: $"Couldn't resolve relative path: {relativePath}", relativePath, innerException: null) { }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="RelativePathException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The exception message.
    /// </param>
    /// <param name="relativePath">
    ///     The relative path it tried to resolve.
    /// </param>
    public RelativePathException(string message, string relativePath)
        : this(message, relativePath, innerException: null) { }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="RelativePathException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The exception message.
    /// </param>
    /// <param name="relativePath">
    ///     The relative path it tried to resolve.
    /// </param>
    /// <param name="innerException">
    ///     The inner exception.
    /// </param>
    public RelativePathException(string message, string relativePath, Exception? innerException)
        : base(message, innerException) { RelativePath = relativePath; }
}