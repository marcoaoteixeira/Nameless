using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nameless.Helpers;
using SysDirectory = System.IO.Directory;
using SysPath = System.IO.Path;
using SysSearchOption = System.IO.SearchOption;

namespace Nameless.FileStorage.FileSystem {
    public sealed class Directory : IDirectory {

        #region Private Properties

        private string Root { get; }
        private string FullPath { get; }
        private Func<string, Action, IDisposable> ChangeWatcherFactory { get; }

        #endregion

        #region Public Constructors

        public Directory (string root, string path, Func<string, Action, IDisposable> changeWatcherFactory) {
            Prevent.ParameterNullOrWhiteSpace (root, nameof (root));
            Prevent.ParameterNull (path, nameof (path));
            Prevent.ParameterNull (changeWatcherFactory, nameof (changeWatcherFactory));

            Root = root;
            FullPath = PathHelper.GetPhysicalPath (root, path);
            ChangeWatcherFactory = changeWatcherFactory;
        }

        #endregion

        #region IDirectory Members

        /// <inheritdoc />
        public string Name => SysPath.GetDirectoryName (FullPath);

        /// <inheritdoc />
        public string Path => FullPath[Root.Length..].TrimStart (SysPath.DirectorySeparatorChar);

        /// <inheritdoc />
        public bool Exists => SysDirectory.Exists (FullPath);

        /// <inheritdoc />
        public DateTimeOffset LastWriteTimeUtc => SysDirectory.GetLastWriteTimeUtc (FullPath);

        /// <inheritdoc />
        public async IAsyncEnumerable<IFile> GetFilesAsync (bool includeSubDirectories = false) {
            if (!Exists) { yield break; }

            var files = SysDirectory.GetFiles (
                path: FullPath,
                searchPattern: "*",
                searchOption : includeSubDirectories ? SysSearchOption.AllDirectories : SysSearchOption.TopDirectoryOnly
            );

            foreach (var file in files) {
                yield return await Task.FromResult (new File (
                    root: Root,
                    path: file[Root.Length..],
                    changeWatcherFactory: ChangeWatcherFactory
                ));
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IDirectory> GetDirectoriesAsync (bool includeSubDirectories = false) {
            if (!Exists) { yield break; }

            var directories = SysDirectory.GetDirectories (
                path: FullPath,
                searchPattern: "*",
                searchOption : includeSubDirectories ? SysSearchOption.AllDirectories : SysSearchOption.TopDirectoryOnly
            );

            foreach (var directory in directories) {
                yield return await Task.FromResult (new Directory (
                    root: Root,
                    path: directory[Root.Length..],
                    changeWatcherFactory: ChangeWatcherFactory
                ));
            }
        }

        /// <inheritdoc />
        public IDisposable Watch (Action<object> callback, string filter = null) {
            filter = !string.IsNullOrWhiteSpace (filter) ? filter : "*.*";
            filter = PathHelper.Normalize (SysPath.Combine (Path, filter));

            return ChangeWatcherFactory (filter, () => callback (Path));
        }

        /// <inheritdoc />
        public Task CopyAsync (string destPath, bool overwrite = false, CancellationToken token = default) {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task MoveAsync (string destPath, CancellationToken token = default) {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<bool> DeleteAsync (CancellationToken token = default) {
            if (!Exists) { return Task.FromResult (false); }

            SysDirectory.Delete (path: FullPath, recursive: true);

            return Task.FromResult (true);
        }

        #endregion
    }
}