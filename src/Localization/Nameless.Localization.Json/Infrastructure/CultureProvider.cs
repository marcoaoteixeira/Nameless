using System.Globalization;
using Microsoft.Extensions.Logging;

namespace Nameless.Localization.Json.Infrastructure;

/// <summary>
/// Default implementation of <see cref="ICultureProvider"/>.
/// </summary>
public sealed class CultureProvider : ICultureProvider {
    private static readonly CultureInfo DefaultCulture = new("en-US");

    private readonly ILogger<CultureProvider> _logger;

    public CultureProvider(ILogger<CultureProvider> logger) {
        _logger = Prevent.Argument.Null(logger);
    }

    public CultureInfo GetCurrentCulture() {
        var culture = Thread.CurrentThread.CurrentUICulture;
        if (!string.IsNullOrWhiteSpace(culture.Name)) {
            _logger.GettingCurrentCultureFromContext(context: "Thread.CurrentThread.CurrentUICulture",
                                                     cultureName: culture.Name);

            return culture;
        }

        culture = Thread.CurrentThread.CurrentCulture;
        if (!string.IsNullOrWhiteSpace(culture.Name)) {
            _logger.GettingCurrentCultureFromContext(context: "Thread.CurrentThread.CurrentCulture",
                                                     cultureName: culture.Name);

            return culture;
        }

        _logger.GettingCurrentCultureFromContext(context: "Default",
                                                 cultureName: DefaultCulture.Name);

        return DefaultCulture;
    }
}