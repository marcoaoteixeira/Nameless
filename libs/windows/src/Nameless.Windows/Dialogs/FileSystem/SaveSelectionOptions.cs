namespace Nameless.Windows.Dialogs.FileSystem;

/// <summary>
///     Save Selection Options
/// </summary>
public class SaveSelectionOptions : FileSystemDialogOptions {
    /// <summary>
    ///     Gets or sets the dialog filter.
    /// </summary>
    public string Filter { get; set; } = string.Empty;

    /// <summary>
    ///     Whether it should ensure the file existence.
    /// </summary>
    public bool EnsureFileExistence { get; set; }

    /// <summary>
    ///     Whether it should overwrite warning messages.
    /// </summary>
    public bool OverwriteWarning { get; set; }
}