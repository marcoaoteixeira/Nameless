using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Nameless.Testing.Tools.Logging;

/// <summary>
///     In-memory <see cref="ILoggerFactory"/> implementation for test scenarios.
///     Keeps track of every <see cref="InMemoryLogger"/> it creates, keyed by
///     category name, so tests can retrieve them after exercising the code
///     under test.
/// </summary>
public sealed class InMemoryLoggerFactory : ILoggerFactory {
    private readonly ConcurrentDictionary<string, InMemoryLogger> _loggers = new(StringComparer.Ordinal);

    /// <summary>
    ///     Gets every logger created so far, keyed by category name.
    /// </summary>
    public IReadOnlyDictionary<string, InMemoryLogger> Loggers => _loggers;

    /// <inheritdoc />
    public ILogger CreateLogger(string categoryName) {
        return GetLogger(categoryName);
    }

    /// <summary>
    ///     Gets (creating it if necessary) the <see cref="InMemoryLogger"/>
    ///     for the specified category name.
    /// </summary>
    /// <param name="categoryName">
    ///     The category name.
    /// </param>
    /// <returns>
    ///     The <see cref="InMemoryLogger"/> instance associated with
    ///     <paramref name="categoryName"/>.
    /// </returns>
    public InMemoryLogger GetLogger(string categoryName) {
        return _loggers.GetOrAdd(categoryName, static _ => new InMemoryLogger());
    }

    /// <summary>
    ///     Gets (creating it if necessary) the <see cref="InMemoryLogger{TCategoryName}"/>
    ///     for the specified category type.
    /// </summary>
    /// <typeparam name="TCategoryName">
    ///     Type of the log category.
    /// </typeparam>
    /// <returns>
    ///     The <see cref="InMemoryLogger{TCategoryName}"/> instance associated
    ///     with <typeparamref name="TCategoryName"/>.
    /// </returns>
    public InMemoryLogger<TCategoryName> GetLogger<TCategoryName>() {
        var categoryName = typeof(TCategoryName).FullName ?? typeof(TCategoryName).Name;

        return (InMemoryLogger<TCategoryName>)_loggers.GetOrAdd(categoryName, static _ => new InMemoryLogger<TCategoryName>());
    }

    /// <inheritdoc />
    public void AddProvider(ILoggerProvider provider) {
        // No-op: this factory does not forward to external providers,
        // it only ever hands out InMemoryLogger instances.
    }

    /// <inheritdoc />
    public void Dispose() {
        _loggers.Clear();
    }
}
