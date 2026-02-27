using Nameless.Attributes;

namespace Nameless.Lucene;

/// <summary>
///     Lucene Options
/// </summary>
[ConfigurationSectionName("Lucene")]
public class LuceneOptions {
    public string DirectoryName { get; set; } = "indexes";
}