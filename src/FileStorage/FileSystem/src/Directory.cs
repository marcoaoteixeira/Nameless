using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dasync.Collections;
using Microsoft.Extensions.Primitives;

namespace Nameless.FileStorage.FileSystem {
    public sealed class Directory : IDirectory {

        #region Private Properties

        private string Root { get; }

        private DirectoryInfo CurrentDirectory { get; }

        private Func<string, IChangeToken> ChangeTokenFactory { get; }

        #endregion

        #region Public Constructors

        public Directory (string root, string path, Func<string, IChangeToken> changeTokenFactory) {
            Prevent.ParameterNullOrWhiteSpace (root, nameof (root));
            Prevent.ParameterNull (path, nameof (path));
            Prevent.ParameterNull (changeTokenFactory, nameof (changeTokenFactory));

            Root = root;
            Path = path;
            CurrentDirectory = new DirectoryInfo (PathHelper.GetPhysicalPath (root, path));
            ChangeTokenFactory = changeTokenFactory;
        }

        #endregion

        #region IDirectory Members

        public string Name => CurrentDirectory.Name;

        public string Path { get; }

        public bool Exists => CurrentDirectory.Exists;

        public long Length => -1;

        public DateTimeOffset LastWriteTimeUtc => CurrentDirectory.LastWriteTimeUtc;

        public IAsyncEnumerable<IFile> GetFilesAsync (bool includeSubDirectories = false) {
            if (!Exists) { return AsyncEnumerable<IFile>.Empty; }

            return new AsyncEnumerable<IFile> (async item => {
                var files = CurrentDirectory.GetFiles (
                    searchPattern: "*",
                    searchOption : includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
                );

                foreach (var file in files) {
                    var path = file.FullName.Substring (Root.Length);
                    await item.ReturnAsync (new File (Root, path, ChangeTokenFactory));
                }
            });
        }

        public IAsyncEnumerable<IDirectory> GetDirectoriesAsync (bool includeSubDirectories = false) {
            if (!Exists) { return AsyncEnumerable<IDirectory>.Empty; }

            return new AsyncEnumerable<IDirectory> (async item => {
                var directories = CurrentDirectory.GetDirectories (
                    searchPattern: "*",
                    searchOption : includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
                );

                foreach (var directory in directories) {
                    var path = directory.FullName.Substring (Root.Length);
                    await item.ReturnAsync (new Directory (Root, path, ChangeTokenFactory));
                }
            });
        }

        public Task<bool> DeleteAsync () {
            if (!Exists) { return Task.FromResult (false); }

            CurrentDirectory.Delete (recursive: true);

            return Task.FromResult (true);
        }

        public void Watch (Action<ChangeEventArgs> callback, string filters = null) {
            filters = !string.IsNullOrWhiteSpace (filters) ? filters : "*.*";

            ChangeToken.OnChange (
                changeTokenProducer: () => ChangeTokenFactory (filters),
                changeTokenConsumer: (state) => callback (new ChangeEventArgs { Path = state }),
                state : Path
            );
        }

        #endregion
    }
}