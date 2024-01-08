namespace Nameless.Lucene {
    /// <summary>
    /// Defines methods for an index representation.
    /// </summary>
    public interface IIndex {
        #region Properties

        /// <summary>
        /// Gets the name of the index.
        /// </summary>
        string Name { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Whether an index is empty or not
        /// </summary>
        bool IsEmpty();

        /// <summary>
        /// Gets the number of indexed documents
        /// </summary>
        int CountDocuments();

        /// <summary>
        /// Creates an empty document
        /// </summary>
        /// <returns></returns>
        IDocument NewDocument(string documentID);

        /// <summary>
        /// Stores all documents into the index.
        /// </summary>
        void StoreDocuments(IDocument[] documents);

        /// <summary>
        /// Removes all documents from the index.
        /// </summary>
        void DeleteDocuments(IDocument[] documents);

        /// <summary>
        /// Creates a search builder for this provider
        /// </summary>
        /// <returns>A search builder instance</returns>
        ISearchBuilder CreateSearchBuilder();

        #endregion
    }
}
