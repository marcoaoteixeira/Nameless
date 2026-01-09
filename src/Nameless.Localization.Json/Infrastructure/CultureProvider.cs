using System.Globalization;
using Microsoft.Extensions.Logging;
using Nameless.Localization.Json.Internals;

namespace Nameless.Localization.Json.Infrastructure;

/// <summary>
///     Default implementation of <see cref="ICultureProvider" />.
/// </summary>
public class CultureProvider : ICultureProvider {
    private static readonly CultureInfo DefaultCulture = new(name: "en-US");

    private readonly ILogger<CultureProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureProvider"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="logger"/> is <see langword="null"/>.
    /// </exception>
    public CultureProvider(ILogger<CultureProvider> logger) {
        _logger = logger;
    }

    /// <inheritdoc />
    public CultureInfo GetCurrentCulture() {
        var culture = Thread.CurrentThread.CurrentUICulture;
        if (!string.IsNullOrWhiteSpace(culture.Name)) {
            _logger.GettingCurrentCultureFromContext(context: "Thread.CurrentThread.CurrentUICulture",
                culture.Name);

            return culture;
        }

        culture = Thread.CurrentThread.CurrentCulture;
        if (!string.IsNullOrWhiteSpace(culture.Name)) {
            _logger.GettingCurrentCultureFromContext(context: "Thread.CurrentThread.CurrentCulture",
                culture.Name);

            return culture;
        }

        _logger.GettingCurrentCultureFromContext(context: "Default",
            DefaultCulture.Name);

        return DefaultCulture;
    }
}