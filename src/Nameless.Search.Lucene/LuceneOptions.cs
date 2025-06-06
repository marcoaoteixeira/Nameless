using System.Reflection;

namespace Nameless.Search.Lucene;

/// <summary>
///     Lucene Options
/// </summary>
public sealed record LuceneOptions {
    public const string DefaultLuceneFolderName = "Lucene";

    private string? _indexesFolderName;

    /// <summary>
    ///     Gets or sets the folder name that will store Lucene.NET indexes.
    /// </summary>
    /// <remarks>
    ///     If no folder name is provided, it will use the
    ///     <see cref="DefaultLuceneFolderName" />.
    /// </remarks>
    public string LuceneFolderName {
        get => _indexesFolderName.WithFallback(DefaultLuceneFolderName);
        set => _indexesFolderName = value;
    }

    /// <summary>
    /// Whether to use auto registration for Lucene services.
    /// </summary>
    public bool UseAutoRegistration { get; set; }

    /// <summary>
    /// Gets or sets the assemblies that will be scanned for Lucene services.
    /// </summary>
    public Assembly[] Assemblies { get; set; } = [];

    /// <summary>
    /// Gets or sets the types of analyzers that will be used for indexing and searching.
    /// </summary>
    public Type[] AnalyzerSelectors { get; set; } = [];
}