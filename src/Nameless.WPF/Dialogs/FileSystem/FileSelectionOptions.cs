namespace Nameless.WPF.Dialogs.FileSystem;

public class FileSelectionOptions : FileSystemDialogOptions {
    public string Filter { get; set; } = string.Empty;
    public bool Multiselect { get; set; }
}