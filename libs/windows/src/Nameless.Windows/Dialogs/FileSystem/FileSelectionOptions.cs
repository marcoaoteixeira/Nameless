namespace Nameless.Windows.Dialogs.FileSystem;

/// <summary>
///     File Selection Options.
/// </summary>
public class FileSelectionOptions : FileSystemDialogOptions {
    /// <summary>
    ///     Gets or sets the filter.
    /// </summary>
    public string Filter { get; set; } = string.Empty;

    /// <summary>
    ///     Whether it should enable multi file selection.
    /// </summary>
    public bool Multiselect { get; set; }
}