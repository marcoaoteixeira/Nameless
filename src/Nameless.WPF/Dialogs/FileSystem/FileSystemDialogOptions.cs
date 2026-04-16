using System.Windows;

namespace Nameless.WPF.Dialogs.FileSystem;

public abstract class FileSystemDialogOptions {
    public string? Title { get; set; }
    public string? Root { get; set; }
    public Window? Owner { get; set; }
}