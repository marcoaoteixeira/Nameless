namespace Nameless.Lucene;

public class IndexDocumentMissingIDException : Exception {
    public IndexDocumentMissingIDException()
        : this ("Index document is missing its ID field.") { }

    public IndexDocumentMissingIDException(string message)
        : base(message) { }

    public IndexDocumentMissingIDException(string message, Exception inner)
        : base(message, inner) { }
}
