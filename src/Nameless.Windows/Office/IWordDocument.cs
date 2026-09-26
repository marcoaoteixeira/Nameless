using Nameless.Windows.Documents;

namespace Nameless.Windows.Office;

public interface IWordDocument : IDisposable {
    WordDocumentStatus Status { get; }

    string GetContent(bool formatted);

    void SaveAs(string filePath, DocumentType type);
}