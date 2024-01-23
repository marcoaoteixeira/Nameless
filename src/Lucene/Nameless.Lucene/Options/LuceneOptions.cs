namespace Nameless.Lucene.Options {
    /// <summary>
    /// Lucene Search Settings.
    /// </summary>
    public sealed class LuceneOptions {
        #region Public Static Read-Only Properties

        public static LuceneOptions Default => new();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the indexes root folder name.
        /// </summary>
        public string IndexesRootFolderName { get; set; } = "Lucene";

        #endregion
    }
}
