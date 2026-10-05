using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Nameless.Compression.Infrastructure;

/// <summary>
/// In-memory <see cref="ILogger{T}"/> that records every message, for asserting on log output.
/// </summary>
internal sealed class ListLogger<T> : ILogger<T> {
    private readonly ConcurrentQueue<LogRecord> _records = new();

    public IReadOnlyList<LogRecord> Records => [.. _records];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel) {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
        _records.Enqueue(new LogRecord(logLevel, eventId, formatter(state, exception)));
    }

    public bool Contains(LogLevel level, string fragment) {
        return Records.Any(r => r.Level == level && r.Message.Contains(fragment, StringComparison.OrdinalIgnoreCase));
    }
}

internal sealed record LogRecord(LogLevel Level, EventId EventId, string Message);
