using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Nameless.Helpers;
using Nameless.Localization.Json.Schemas;
using Newtonsoft.Json;

namespace Nameless.Localization.Json {
    /// <summary>
    /// Holds all mechanics to read the localization files.
    /// For performance sake, use only one instance of this class
    /// through the application life time.
    /// </summary>
    public sealed class MessageCollectionAggregationProvider : IMessageCollectionAggregationProvider, IDisposable {
        #region Private Read-Only Fields

        private readonly IFileProvider _fileProvider;
        private readonly LocalizationSettings _settings;
        private readonly object _syncLock = new object ();

        #endregion

        #region Private Fields

        private IDictionary<string, MessageCollectionAggregation> _messageCollectionAggregationCache = new Dictionary<string, MessageCollectionAggregation> ();
        private IDictionary<string, IDisposable> _changeTokenCache = new Dictionary<string, IDisposable> ();
        private bool _disposed;

        #endregion

        #region Public Constructors

        public MessageCollectionAggregationProvider (IFileProvider fileProvider, LocalizationSettings settings = null) {
            Prevent.ParameterNull (fileProvider, nameof (fileProvider));

            _fileProvider = fileProvider;
            _settings = settings ?? new LocalizationSettings ();
        }

        #endregion

        #region Destructor

        ~MessageCollectionAggregationProvider () {
            Dispose (disposing: false);
        }

        #endregion

        #region IMessageCollectionAggregationProvider Members

        public MessageCollectionAggregation Create (string cultureName) {
            BlockAccessAfterDispose ();

            lock (_syncLock) {
                // Checks if is a valid culture name.
                if (!CultureInfoHelper.TryGetCultureInfo (cultureName, out _)) {
                    return null;
                }

                // Retrieves the associated file
                var file = _fileProvider
                    .GetDirectoryContents (_settings.ResourceFolderPath)
                    .SingleOrDefault (_ => Path.GetFileName (_.PhysicalPath) == $"{cultureName}.json");

                // If file not exists, return.
                if (file == null || !file.Exists) { return null; }

                if (!_messageCollectionAggregationCache.ContainsKey (cultureName)) {
                    var json = file.GetText (Encoding.UTF8); /* always read files as UTF-8 */
                    var messageCollections = JsonConvert.DeserializeObject<MessageCollection[]> (json, new MessageCollectionJsonConverter ());
                    var messageCollectionAggregation = new MessageCollectionAggregation (cultureName, messageCollections);
                    _messageCollectionAggregationCache.Add (cultureName, messageCollectionAggregation);

                    // Keep an eye in the file changed event, if needed.
                    if (_settings.Watch) {
                        var changeToken = ChangeToken.OnChange (
                            changeTokenProducer: () => _fileProvider.Watch (file.PhysicalPath),
                            changeTokenConsumer : ChangeTokenCallback,
                            file.PhysicalPath
                        );
                        _changeTokenCache.Add (file.PhysicalPath, changeToken);
                    }
                }

                return _messageCollectionAggregationCache[cultureName];
            }
        }

        #endregion

        #region Private Methods

        private void ChangeTokenCallback (string filePath) {
            lock (_syncLock) {
                if (_changeTokenCache.ContainsKey (filePath)) {
                    _changeTokenCache[filePath].Dispose ();
                    _changeTokenCache.Remove (filePath);

                    var cultureName = Path.GetFileNameWithoutExtension (filePath);
                    if (_messageCollectionAggregationCache.ContainsKey (cultureName)) {
                        _messageCollectionAggregationCache.Remove (cultureName);
                    }
                }
            }
        }

        private void BlockAccessAfterDispose () {
            if (_disposed) {
                throw new ObjectDisposedException (GetType ().FullName);
            }
        }

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                lock (_syncLock) {
                    if (_changeTokenCache != null) {
                        foreach (var changeToken in _changeTokenCache.Values) {
                            changeToken.Dispose ();
                        }
                    }
                }
            }

            _messageCollectionAggregationCache.Clear ();
            _messageCollectionAggregationCache = null;

            _changeTokenCache.Clear ();
            _changeTokenCache = null;

            _disposed = true;
        }

        #endregion

        #region IDisposable Members

        public void Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (this);
        }

        #endregion
    }
}