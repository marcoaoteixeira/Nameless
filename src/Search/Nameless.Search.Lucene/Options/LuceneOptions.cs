namespace Nameless.Search.Lucene.Options;

/// <summary>
/// Lucene Options
/// </summary>
public sealed record LuceneOptions {
    public const string DEFAULT_LUCENE_FOLDER_NAME = "Lucene";

    private string? _indexesFolderName;
    /// <summary>
    /// Gets or sets the folder name that will store Lucene.NET indexes.
    /// </summary>
    /// <remarks>
    /// If no folder name is provided, it will use the
    /// <see cref="DEFAULT_LUCENE_FOLDER_NAME"/>.
    /// </remarks>
    public string LuceneFolderName {
        get => _indexesFolderName.WithFallback(DEFAULT_LUCENE_FOLDER_NAME);
        set => _indexesFolderName = value;
    }
}