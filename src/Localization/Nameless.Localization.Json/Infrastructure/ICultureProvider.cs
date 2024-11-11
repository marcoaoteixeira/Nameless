using System.Globalization;

namespace Nameless.Localization.Json.Infrastructure;

/// <summary>
/// Provides methods to retrieve the current culture.
/// </summary>
public interface ICultureProvider {

    /// <summary>
    /// Retrieves the current culture.
    /// </summary>
    /// <returns>The current <see cref="CultureInfo"/> being used.</returns>
    CultureInfo GetCurrentCulture();
}