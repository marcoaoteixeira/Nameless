namespace Nameless.WPF.Documents;

public interface IDocumentReader {
    bool CanRead(string filePath);

    Task<string> GetContentAsync(string filePath, CancellationToken cancellationToken);
}