using System.Windows;

namespace Nameless.Windows.Dialogs.FileSystem;

/// <summary>
///     File System Dialog Options.
/// </summary>
public abstract class FileSystemDialogOptions {
    /// <summary>
    ///     Gets or sets the dialog title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    ///     Gets or sets the dialog root path.
    /// </summary>
    public string? Root { get; set; }

    /// <summary>
    ///     Gets or sets the owner window for the dialog.
    /// </summary>
    public Window? Owner { get; set; }
}