namespace Nameless.Lucene {

    /// <summary>
    /// Enumerator for document index options.
    /// </summary>
    [Flags]
    public enum FieldOptions {
        /// <summary>
        /// No option defined.
        /// </summary>
        None = 0,
        /// <summary>
        /// Just store
        /// </summary>
        Store = 1,
        /// <summary>
        /// Analise and store.
        /// </summary>
        Analyze = 2,
        /// <summary>
        /// Sanitize the stored document.
        /// </summary>
        Sanitize = 4
    }
}
