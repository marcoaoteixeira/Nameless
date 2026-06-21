using Nameless.Attributes;

namespace Nameless.Lucene;

/// <summary>
///     Lucene Options
/// </summary>
[ConfigurationSectionName("Lucene")]
public record LuceneOptions {
    /// <summary>
    ///     Gets or initializes the directory name used to store the Lucene index files.
    /// </summary>
    public string DirectoryName { get; init; } = "lucene";
}