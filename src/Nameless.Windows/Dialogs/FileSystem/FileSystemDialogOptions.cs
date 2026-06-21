using System.Windows;

namespace Nameless.Windows.Dialogs.FileSystem;

public abstract class FileSystemDialogOptions {
    public string? Title { get; set; }
    public string? Root { get; set; }
    public Window? Owner { get; set; }
}