using Nameless.WPF.Documents;

namespace Nameless.WPF.Office;

public interface IWordDocument : IDisposable {
    WordDocumentStatus Status { get; }

    string GetContent(bool formatted);

    void SaveAs(string filePath, DocumentType type);
}