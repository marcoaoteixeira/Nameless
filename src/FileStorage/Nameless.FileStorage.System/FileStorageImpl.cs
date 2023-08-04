using Nameless.Helpers;
using Nameless.Infrastructure;
using Sys_File = System.IO.File;

namespace Nameless.FileStorage.System {
    public sealed class FileStorageImpl : IFileStorage {
        #region Public Constructors

        public FileStorageImpl(IApplicationContext applicationContext) {
            Prevent.Against.Null(applicationContext, nameof(applicationContext));

            Root = PathHelper.Normalize(applicationContext.DataDirectoryPath);
        }

        #endregion

        #region Private Static Methods

        private static ChangeReason ParseChangeTypes(WatcherChangeTypes types) => types switch {
            WatcherChangeTypes.Changed => ChangeReason.Changed,
            WatcherChangeTypes.Created => ChangeReason.Created,
            WatcherChangeTypes.Deleted => ChangeReason.Deleted,
            WatcherChangeTypes.Renamed => ChangeReason.Renamed,
            _ => ChangeReason.None
        };

        #endregion

        #region Private Methods

        private IDisposable ChangeWatcherFactory(string path, string? filter, Action<ChangeEventArgs> callback) {
            var watcher = new FileSystemWatcher {
                Filter = filter ?? "*.*",
                Path = path,
                NotifyFilter = NotifyFilters.LastAccess |
                               NotifyFilters.LastWrite |
                               NotifyFilters.FileName |
                               NotifyFilters.DirectoryName
            };

            watcher.Changed += (sender, evt) => FileSystemWatcherCallback(sender, evt, callback);
            watcher.Deleted += (sender, evt) => FileSystemWatcherCallback(sender, evt, callback);
            watcher.Renamed += (sender, evt) => FileSystemWatcherCallback(sender, evt, callback);

            watcher.EnableRaisingEvents = true;

            return watcher;
        }

        private void FileSystemWatcherCallback(object sender, FileSystemEventArgs args, Action<ChangeEventArgs> callback) {
            if (sender is not FileSystemInfo obj) { return; }

            var originalPath = obj.FullName;
            var currentPath = args is RenamedEventArgs renamedArgs ? renamedArgs.OldFullPath : args.FullPath;

            var callbackArgs = new ChangeEventArgs {
                OriginalPath = originalPath[Root.Length..].TrimStart(Path.DirectorySeparatorChar),
                CurrentPath = currentPath[Root.Length..].TrimStart(Path.DirectorySeparatorChar),
                Reason = ParseChangeTypes(args.ChangeType)
            };

            callback(callbackArgs);
        }

        #endregion

        #region IFileStorage Members

        /// <inheritdoc />
        public string Root { get; }

        /// <inheritdoc />
        public Task CreateFileAsync(string relativePath, Stream input, bool overwrite = false, CancellationToken token = default) {
            Prevent.Against.NullOrWhiteSpace(relativePath, nameof(relativePath));
            Prevent.Against.Null(input, nameof(input));

            var filePath = PathHelper.GetPhysicalPath(Root, relativePath);

            if (Sys_File.Exists(filePath) && !overwrite) {
                throw new FileStorageException("Cannot create file because the destination path already exists.");
            }

            // Create directory path if it doesn't exist.
            var directoryPath = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(directoryPath!);

            using var output = Sys_File.Create(filePath);
            return input.CopyToAsync(output, token);
        }

        /// <inheritdoc />
        public Task<IFile> GetFileAsync(string relativePath, CancellationToken cancellationToken = default) {
            Prevent.Against.NullOrWhiteSpace(relativePath, nameof(relativePath));

            var currentRelativePath = PathHelper.Normalize(relativePath);

            cancellationToken.ThrowIfCancellationRequested();

            var file = new File(Root, currentRelativePath, ChangeWatcherFactory);

            return Task.FromResult<IFile>(file);
        }

        public IAsyncEnumerable<IFile> GetFilesAsync(string? filter = null, CancellationToken cancellationToken = default) {
            var files = filter != null
                ? Directory.GetFiles(Root, filter)
                : Directory.GetFiles(Root);

            cancellationToken.ThrowIfCancellationRequested();

            var result = files.Select(_ => new File(Root, _, ChangeWatcherFactory));

            return result.AsAsyncEnumerable();
        }

        #endregion
    }
}