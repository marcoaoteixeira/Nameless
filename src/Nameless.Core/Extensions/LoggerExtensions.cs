using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless;

/// <summary>
/// <see cref="ILogger" /> extension methods. 
/// </summary>
public static class LoggerExtensions {
    /// <summary>
    /// Logs a message at the specified log level if the condition is <see langword="true"/>.
    /// </summary>
    /// <typeparam name="TCategory">Type of the category.</typeparam>
    /// <param name="self">The current logger.</param>
    /// <param name="condition">The condition.</param>
    /// <returns>
    /// The current logger.
    /// </returns>
    public static ILogger<TCategory> OnCondition<TCategory>(this ILogger<TCategory> self, bool condition) {
        return condition ? self : NullLogger<TCategory>.Instance;
    }
}