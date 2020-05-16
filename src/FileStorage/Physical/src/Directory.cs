using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dasync.Collections;
using Microsoft.Extensions.Primitives;

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
        public IAsyncEnumerable<IFile> GetFilesAsync (bool includeSubDirectories = false) {
            if (!Exists) { return AsyncEnumerable<IFile>.Empty; }

            return new AsyncEnumerable<IFile> (async item => {
                var files = CurrentDirectory.GetFiles (
                    searchPattern: "*",
                    searchOption : includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
                );

                foreach (var file in files) {
                    var path = file.FullName.Substring (Root.Length);
                    await item.ReturnAsync (new File (Root, path, ChangeWatcherFactory));
                }
            });
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IDirectory> GetDirectoriesAsync (bool includeSubDirectories = false) {
            if (!Exists) { return AsyncEnumerable<IDirectory>.Empty; }

            return new AsyncEnumerable<IDirectory> (async item => {
                var directories = CurrentDirectory.GetDirectories (
                    searchPattern: "*",
                    searchOption : includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
                );

                foreach (var directory in directories) {
                    var path = directory.FullName.Substring (Root.Length);
                    await item.ReturnAsync (new Directory (Root, path, ChangeWatcherFactory));
                }
            });
        }

        /// <inheritdoc />
        public Task<bool> DeleteAsync () {
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

            return ChangeWatcherFactory (filter, () => {
                callback (CurrentState.ToChangeEventArgs ());
                FetchDirectoryInfo (CurrentState.NewPath ?? CurrentState.OldPath);
            });
        }

        #endregion
    }
}