namespace Nameless.WPF.Dialogs.FileSystem;

public interface IFileSystemDialog {
    IEnumerable<string> OpenDirectory(Action<DirectorySelectionOptions> configure);

    IEnumerable<string> OpenFile(Action<FileSelectionOptions> configure);

    string OpenSave(Action<SaveSelectionOptions> configure);
}