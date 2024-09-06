using System.Globalization;

namespace Nameless.Localization.Json.Infrastructure.Impl;

[Singleton]
public sealed class CultureContext : ICultureContext {
    /// <summary>
    /// Gets the unique instance of <see cref="CultureContext" />.
    /// </summary>
    public static ICultureContext Instance { get; } = new CultureContext();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static CultureContext() { }

    private CultureContext() { }

    public CultureInfo GetCurrentCulture() {
        CultureInfo culture;

        culture = Thread.CurrentThread.CurrentUICulture;
        if (!string.IsNullOrWhiteSpace(culture.Name)) {
            return culture;
        }

        culture = Thread.CurrentThread.CurrentCulture;
        if (!string.IsNullOrWhiteSpace(culture.Name)) {
            return culture;
        }

        culture = new CultureInfo("en-US");

        return culture;
    }
}