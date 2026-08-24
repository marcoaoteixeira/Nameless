namespace Nameless.IO.Explorer;

/// <summary>
///     Provides a hub to create <see cref="IFileExplorer"/> instances.
/// </summary>
public interface IFileExplorerProvider {
    /// <summary>
    ///     Creates an instance of <see cref="IFileExplorer"/> class.
    /// </summary>
    /// <param name="root">
    ///     The root path.
    /// </param>
    /// <param name="allowOperationOutsideRoot">
    ///     Whether it should allow File System Provider access files outside
    ///     the root path.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="IFileExplorer"/>, if another
    ///     File Explorer Provider was created before with the same path,
    ///     returns the reference to it.
    /// </returns>
    IFileExplorer Create(string root, bool allowOperationOutsideRoot = false);
}