using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nameless.Helpers;

namespace Nameless.FileStorage.FileSystem {
    public class Directory : IDirectory {
        #region Private Read-Only Fields

        private readonly string _root;
        private readonly string _path;

        #endregion

        #region Private Fields

        private DirectoryInfo _directoryInfo;

        #endregion

        #region Public Constructors

        public Directory (string root, string path) {
            Prevent.ParameterNullOrWhiteSpace (root, nameof (root));
            Prevent.ParameterNullOrWhiteSpace (path, nameof (path));

            _root = root;
            _path = path;

            var physicalDirectoryPath = PathHelper.GetPhysicalPath (_root, path);
            _directoryInfo = new DirectoryInfo (physicalDirectoryPath);
        }

        #endregion

        #region IDirectory Members

        public string Name => _directoryInfo.Name;

        public string Path => _path;

        public bool Exists => _directoryInfo.Exists;

        public DateTime CreationTimeUtc => _directoryInfo.CreationTimeUtc;

        public DateTime LastModifiedUtc => _directoryInfo.LastWriteTimeUtc;

        public Task<IEnumerable<IFile>> GetContentAsync (bool includeSubDirectories = false) {
            if (!Exists) { return Task.FromResult (Enumerable.Empty<IFile> ()); }

            var result = new List<IFile> ();
            var entries = _directoryInfo.GetFiles (
                searchPattern: "*",
                searchOption: includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
            );

            result.AddRange (
                entries.Select (_ => {
                    var fileRelativePath = _.FullName.Substring (_root.Length);
                    var filePath = PathHelper.NormalizePath (fileRelativePath);
                    return new File (_root, filePath);
                })
            );

            return Task.FromResult ((IEnumerable<IFile>)result);
        }

        public Task<IEnumerable<IDirectory>> GetSubDirectoriesAsync (bool includeSubDirectories = false) {
            if (!Exists) { return Task.FromResult (Enumerable.Empty<IDirectory> ()); }

            var result = new List<IDirectory> ();
            var entries = _directoryInfo.GetDirectories (
                searchPattern: "*",
                searchOption: includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
            );

            result.AddRange (
                entries.Select (_ => {
                    var directoryRelativePath = _.FullName.Substring (_root.Length);
                    var directoryPath = PathHelper.NormalizePath (directoryRelativePath);
                    return new Directory (_root, directoryPath);
                })
            );

            return Task.FromResult ((IEnumerable<IDirectory>)result);
        }

        public Task<bool> TryDeleteAsync () {
            if (!Exists) { return Task.FromResult (false); }

            _directoryInfo.Delete (recursive: true);

            return Task.FromResult (true);
        }

        #endregion
    }
}