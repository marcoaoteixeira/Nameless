using Microsoft.Win32;
using Nameless.Application;
using Nameless.Contracts;
using Nameless.Windows.Dialogs.FileSystem.Resources;
using System.Windows;

namespace Nameless.Windows.Dialogs.FileSystem;

/// <summary>
///     File System Dialog
/// </summary>
public class FileSystemDialog : IFileSystemDialog {
    private readonly IApplicationContext _applicationContext;

    /// <summary>
    ///     Initializes a new instance of <see cref="FileSystemDialog"/> class.
    /// </summary>
    /// <param name="applicationContext"></param>
    public FileSystemDialog(IApplicationContext applicationContext) {
        _applicationContext = Throws.When.Null(applicationContext);
    }

    /// <inheritdoc />
    public IEnumerable<string> OpenDirectory(Action<DirectorySelectionOptions> configure) {
        var options = new DirectorySelectionOptions();

        configure(options);

        var dialog = new OpenFolderDialog {
            DefaultDirectory = options.Root ?? _applicationContext.FileManager.Root,
            Title = options.Title ?? GetFallbackTitle(),
            Multiselect = options.Multiselect,
            ValidateNames = true
        };

        OpenDialog(dialog, options);

        return dialog.FolderNames;

        string GetFallbackTitle() {
            return options.Multiselect
                ? FileSystemDialogResources.OpenDirectory_FallbackTitlePlural
                : FileSystemDialogResources.OpenDirectory_FallbackTitle;
        }
    }

    /// <inheritdoc />
    public IEnumerable<string> OpenFile(Action<FileSelectionOptions> configure) {
        var options = new FileSelectionOptions();

        configure(options);

        var dialog = new OpenFileDialog {
            DefaultDirectory = options.Root ?? _applicationContext.FileManager.Root,
            Title = options.Title ?? GetFallbackTitle(),
            Multiselect = options.Multiselect,
            Filter = options.Filter,
            ValidateNames = true
        };

        OpenDialog(dialog, options);

        return dialog.FileNames;

        string GetFallbackTitle() {
            return options.Multiselect
                ? FileSystemDialogResources.OpenFile_FallbackTitlePlural
                : FileSystemDialogResources.OpenFile_FallbackTitle;
        }
    }

    /// <inheritdoc />
    public string OpenSave(Action<SaveSelectionOptions> configure) {
        var options = new SaveSelectionOptions();

        configure(options);

        var dialog = new SaveFileDialog {
            DefaultDirectory = options.Root ?? _applicationContext.FileManager.Root,
            Title = options.Title ?? FileSystemDialogResources.OpenSave_FallbackTitle,
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