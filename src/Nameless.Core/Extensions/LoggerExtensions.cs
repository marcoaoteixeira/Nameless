using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless;

/// <summary>
/// <see cref="ILogger" /> extension methods. 
/// </summary>
public static class LoggerExtensions {
    /// <param name="self">The current logger.</param>
    /// <typeparam name="TCategory">Type of the category.</typeparam>
    extension<TCategory>(ILogger<TCategory> self) {
        /// <summary>
        /// Logs a message at the specified log level if the condition is <see langword="true"/>.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// The current logger.
        /// </returns>
        public ILogger<TCategory> OnCondition(bool condition) {
            return condition ? self : NullLogger<TCategory>.Instance;
        }
    }
}