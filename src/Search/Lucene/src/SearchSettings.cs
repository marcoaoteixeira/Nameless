using System.IO;

namespace Nameless.Search.Lucene {

    /// <summary>
    /// Lucene Search Settings.
    /// </summary>
    public sealed class SearchSettings {

        #region Public Properties

        /// <summary>
        /// Gets or sets the index storage directory path.
        /// </summary>
        public string IndexStorageDirectoryPath { get; set; } = Path.Combine (typeof (SearchSettings).Assembly.GetDirectoryPath (), "App_Data", "Lucene");

        #endregion
    }
}