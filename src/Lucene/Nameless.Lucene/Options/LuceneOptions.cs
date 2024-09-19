namespace Nameless.Lucene.Options;

/// <summary>
/// Lucene Search Settings.
/// </summary>
public sealed record LuceneOptions {
    public const string DEFAULT_INDEXES_FOLDER_NAME = "Lucene";

    private string? _indexesFolderName;

    public static LuceneOptions Default => new();

    /// <summary>
    /// Gets or sets the indexes folder name.
    /// </summary>
    /// <remarks>
    /// if no folder name is provided, it will use the <see cref="DEFAULT_INDEXES_FOLDER_NAME"/>.
    /// </remarks>
    public string IndexesFolderName {
        get => _indexesFolderName.WithFallback(DEFAULT_INDEXES_FOLDER_NAME);
        set => _indexesFolderName = value;
    }
}