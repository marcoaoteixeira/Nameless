namespace Nameless.Windows.Dialogs.FileSystem;

/// <summary>
///     File System Dialog Contract
/// </summary>
public interface IFileSystemDialog {
    /// <summary>
    ///     Opens the directory selection dialog.
    /// </summary>
    /// <param name="configure">
    ///     The configuration delegate.
    /// </param>
    /// <returns>
    ///     A collection of selected directories.
    /// </returns>
    IEnumerable<string> OpenDirectory(Action<DirectorySelectionOptions> configure);

    /// <summary>
    ///     Opens the file selection dialog.
    /// </summary>
    /// <param name="configure">
    ///     The configuration delegate.
    /// </param>
    /// <returns>
    ///     A collection of selected files.
    /// </returns>
    IEnumerable<string> OpenFile(Action<FileSelectionOptions> configure);

    /// <summary>
    ///     Opens the save file dialog.
    /// </summary>
    /// <param name="configure">
    ///     The configuration delegate.
    /// </param>
    /// <returns>
    ///     A path to the file.
    /// </returns>
    string OpenSave(Action<SaveSelectionOptions> configure);
}