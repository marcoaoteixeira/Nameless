namespace Nameless.Windows.Dialogs.FileSystem;

/// <summary>
///     Directory Selection Options
/// </summary>
public class DirectorySelectionOptions : FileSystemDialogOptions {
    /// <summary>
    ///     Whether it should enable multi file selection.
    /// </summary>
    public bool Multiselect { get; set; }
}