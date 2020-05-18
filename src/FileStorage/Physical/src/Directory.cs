using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Dasync.Collections;

namespace Nameless.FileStorage.Physical {
    public sealed class Directory : IDirectory {

        #region Private Properties

        private string Root { get; }
        private Func<string, Action, IDisposable> ChangeWatcherFactory { get; }
        private DirectoryInfo CurrentDirectory { get; set; }
        private EntryState CurrentState { get; } = new EntryState ();

        #endregion

        #region Public Constructors

        public Directory (string root, string path, Func<string, Action, IDisposable> changeWatcherFactory) {
            Prevent.ParameterNullOrWhiteSpace (root, nameof (root));
            Prevent.ParameterNull (path, nameof (path));
            Prevent.ParameterNull (changeWatcherFactory, nameof (changeWatcherFactory));

            Root = root;
            ChangeWatcherFactory = changeWatcherFactory;

            FetchDirectoryInfo (path);
        }

        #endregion

        #region Private Methods

        private void FetchDirectoryInfo (string relativePath) {
            CurrentDirectory = null;

            if (!string.IsNullOrWhiteSpace (relativePath)) {
                var path = PathHelper.GetPhysicalPath (Root, relativePath);
                CurrentDirectory = new DirectoryInfo (path);
            }
        }

        #endregion

        #region IDirectory Members

        /// <inheritdoc />
        public string Name => CurrentDirectory.Name;

        /// <inheritdoc />
        public string Path => CurrentDirectory.FullName[Root.Length..].TrimStart (System.IO.Path.DirectorySeparatorChar);

        /// <inheritdoc />
        public bool Exists => CurrentDirectory.Exists;

        /// <inheritdoc />
        public DateTimeOffset LastWriteTimeUtc => CurrentDirectory.LastWriteTimeUtc;

        /// <inheritdoc />
        public async IAsyncEnumerable<IFile> GetFilesAsync (bool includeSubDirectories = false) {
            if (!Exists) { yield break; }

            var files = CurrentDirectory.GetFiles (
                searchPattern: "*",
                searchOption : includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
            );

            foreach (var file in files) {
                var path = file.FullName.Substring (Root.Length);
                yield return await Task.FromResult (new File (Root, path, ChangeWatcherFactory));
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IDirectory> GetDirectoriesAsync (bool includeSubDirectories = false) {
            if (!Exists) { yield break; }

            var directories = CurrentDirectory.GetDirectories (
                searchPattern: "*",
                searchOption : includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
            );

            foreach (var directory in directories) {
                var path = directory.FullName.Substring (Root.Length);
                yield return await Task.FromResult (new Directory (Root, path, ChangeWatcherFactory));
            }
        }

        /// <inheritdoc />
        public Task<bool> DeleteAsync (CancellationToken token = default) {
            if (!Exists) { return Task.FromResult (false); }

            CurrentDirectory.Delete (recursive: true);

            // Track changes
            CurrentState.OldPath = Path;
            CurrentState.NewPath = null;
            CurrentState.Action = ChangeEventAction.Deleted;
            // Track changes

            return Task.FromResult (true);
        }

        /// <inheritdoc />
        public IDisposable Watch (Action<ChangeEventArgs> callback, string filter = null) {
            filter = !string.IsNullOrWhiteSpace (filter) ? filter : "*.*";
            filter = System.IO.Path.Combine (Path, filter).Replace ('\\', '/');

            return ChangeWatcherFactory (filter, () => {
                callback (CurrentState.ToChangeEventArgs ());
                FetchDirectoryInfo (CurrentState.NewPath ?? CurrentState.OldPath);
            });
        }

        #endregion
    }
}