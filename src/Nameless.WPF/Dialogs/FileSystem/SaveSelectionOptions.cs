namespace Nameless.WPF.Dialogs.FileSystem;

public class SaveSelectionOptions : FileSystemDialogOptions {
    public string Filter { get; set; } = string.Empty;

    public bool EnsureFileExistence { get; set; }

    public bool OverwriteWarning { get; set; }
}