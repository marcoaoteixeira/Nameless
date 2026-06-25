using Nameless.Attributes;

namespace Nameless.Windows.Localization;

[ConfigurationSectionName("ResourceLocalizer")]
public record ResourceLocalizerOptions {
    public string? ResourceFullName { get; init; }
}