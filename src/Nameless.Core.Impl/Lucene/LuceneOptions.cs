using Nameless.Attributes;

namespace Nameless.Lucene;

/// <summary>
///     Lucene Options
/// </summary>
[ConfigurationSectionName("Lucene")]
public record LuceneOptions {
    public string DirectoryName { get; init; } = "lucene";
}