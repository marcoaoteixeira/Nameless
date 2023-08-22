namespace Nameless.Lucene {
    /// <summary>
    /// Lucene Search Settings.
    /// </summary>
    public sealed class LuceneOptions {
        #region Public Static Read-Only Properties

        public static LuceneOptions Default => new();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the index directory name.
        /// </summary>
        public string IndexFolder { get; set; } = Path.Combine("App_Data", "Lucene");

        #endregion
    }
}
