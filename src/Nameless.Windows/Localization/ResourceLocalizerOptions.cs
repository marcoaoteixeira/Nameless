using Nameless.Attributes;

namespace Nameless.Windows.Localization;

/// <summary>
///     Configuration options for <see cref="ResourceLocalizer"/>.
///     Bound from the <c>ResourceLocalizer</c> configuration section.
/// </summary>
[ConfigurationSectionName("ResourceLocalizer")]
public record ResourceLocalizerOptions {
    /// <summary>
    ///     Assembly-qualified name of the generated resource class
    ///     (e.g. <c>"MyApp.Resources.Translations, MyApp"</c>).
    ///     Used to locate the <see cref="System.Resources.ResourceManager"/>
    ///     at runtime.
    /// </summary>
    public string? ResourceFullName { get; init; }
}