using System.Windows;
using Microsoft.Win32;
using Nameless.Application;
using Nameless.WPF.Resources;

namespace Nameless.WPF.Dialogs.FileSystem.Impl;

public class FileSystemDialog : IFileSystemDialog {
    private readonly IApplicationContext _applicationContext;

    public FileSystemDialog(IApplicationContext applicationContext) {
        _applicationContext = applicationContext;
    }

    public IEnumerable<string> OpenDirectory(Action<DirectorySelectionOptions> configure) {
        var options = new DirectorySelectionOptions();

        configure(options);

        var dialog = new OpenFolderDialog {
            DefaultDirectory = options.Root ?? _applicationContext.FileSystemProvider.Root,
            Title = options.Title ?? GetFallbackTitle(),
            Multiselect = options.Multiselect,
            ValidateNames = true
        };

        OpenDialog(dialog, options);

        return dialog.FolderNames;

        string GetFallbackTitle() {
            return options.Multiselect
                ? Strings.FileSystemDialog_OpenDirectory_Fallback_Title_Plural
                : Strings.FileSystemDialog_OpenDirectory_Fallback_Title;
        }
    }

    public IEnumerable<string> OpenFile(Action<FileSelectionOptions> configure) {
        var options = new FileSelectionOptions();

        configure(options);

        var dialog = new OpenFileDialog {
            DefaultDirectory = options.Root ?? _applicationContext.FileSystemProvider.Root,
            Title = options.Title ?? GetFallbackTitle(),
            Multiselect = options.Multiselect,
            Filter = options.Filter,
            ValidateNames = true
        };

        OpenDialog(dialog, options);

        return dialog.FileNames;

        string GetFallbackTitle() {
            return options.Multiselect
                ? Strings.FileSystemDialog_OpenFile_Fallback_Title_Plural
                : Strings.FileSystemDialog_OpenFile_Fallback_Title;
        }
    }

    public string OpenSave(Action<SaveSelectionOptions> configure) {
        var options = new SaveSelectionOptions();

        configure(options);

        var dialog = new SaveFileDialog {
            DefaultDirectory = options.Root ?? _applicationContext.FileSystemProvider.Root,
            Title = options.Title ?? Strings.FileSystemDialog_OpenSave_Fallback_Title,
            CheckFileExists = options.EnsureFileExistence,
            Filter = options.Filter,
            OverwritePrompt = options.OverwriteWarning,
            ValidateNames = true
        };

        OpenDialog(dialog, options);

        return dialog.FileName;
    }

    private static void OpenDialog(CommonDialog dialog, FileSystemDialogOptions options) {
        var owner = GetOwner(options);

        if (owner is not null) { dialog.ShowDialog(owner); }
        else { dialog.ShowDialog(); }

        return;

        static Window? GetOwner(FileSystemDialogOptions options) {
            if (options.Owner is not null) {
                return options.Owner;
            }

            try { return WindowsApplication.Current.MainWindow; }
            catch { /* swallow */ }

            return null;
        }
    }
}