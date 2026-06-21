namespace Nameless.Windows.Documents;

public interface IDocumentConverter {
    DocumentType OutputType { get; }

    bool CanConvert(DocumentType type);

    Task<string> ConvertAsync(string filePath, CancellationToken cancellationToken);
}