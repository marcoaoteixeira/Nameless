using Microsoft.Extensions.Localization;

namespace Nameless.Localization.Json;

[Singleton]
public sealed class NullStringLocalizer : IStringLocalizer {
    /// <summary>
    /// Gets the unique instance of <see cref="NullStringLocalizer"/>.
    /// </summary>
    public static IStringLocalizer Instance { get; } = new NullStringLocalizer();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static NullStringLocalizer() { }

    private NullStringLocalizer() { }

    /// <inheritdoc />
    public LocalizedString this[string name]
        => new(name, name);

    /// <inheritdoc />
    public LocalizedString this[string name, params object[] arguments]
        => new(name, string.Format(name, arguments));
    
    /// <inheritdoc />
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => [];
}