using Microsoft.Extensions.Logging;
using Nameless.Contracts;

namespace Nameless.Testing.Tools.Logging;

/// <summary>
///     In-memory <see cref="ILogger"/> implementation for test scenarios.
///     Captures every logged entry, including the scopes (<see cref="BeginScope{TState}"/>)
///     that were active when the entry was logged.
/// </summary>
public class InMemoryLogger : ILogger {
    private readonly Lock _sync = new();
    private readonly List<LogEntry> _entries = [];
    private readonly List<object?> _activeScopes = [];
    private readonly List<object?> _capturedScopes = [];

    /// <summary>
    ///     Gets the entries logged so far, in the order they were logged.
    /// </summary>
    public IReadOnlyList<LogEntry> Entries {
        get {
            lock (_sync) {
                return [.. _entries];
            }
        }
    }

    /// <summary>
    ///     Gets the scope states currently active (outer to inner), i.e.
    ///     the ones whose <see cref="IDisposable"/> has not been disposed yet.
    /// </summary>
    public IReadOnlyList<object?> ActiveScopes {
        get {
            lock (_sync) {
                return [.. _activeScopes];
            }
        }
    }

    /// <summary>
    ///     Gets every scope state ever passed to <see cref="BeginScope{TState}"/>,
    ///     regardless of whether it has since been disposed.
    /// </summary>
    public IReadOnlyList<object?> CapturedScopes {
        get {
            lock (_sync) {
                return [.. _capturedScopes];
            }
        }
    }

    /// <inheritdoc />
    public virtual IDisposable BeginScope<TState>(TState state) where TState : notnull {
        lock (_sync) {
            _capturedScopes.Add(state);
            _activeScopes.Add(state);
        }

        return new Scope(this, state);
    }

    /// <inheritdoc />
    public virtual bool IsEnabled(LogLevel logLevel) {
        return logLevel != LogLevel.None;
    }

    /// <inheritdoc />
    public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
        var message = Throws.When.Null(formatter)(state, exception);

        lock (_sync) {
            _entries.Add(new LogEntry(
                logLevel,
                eventId,
                message,
                exception,
                state,
                [.. _activeScopes]
            ));
        }
    }

    /// <summary>
    ///     Verifies if a <see cref="LogLevel.Trace"/> message was captured.
    /// </summary>
    /// <param name="assertMessage">
    ///     Assert the message content.
    /// </param>
    /// <param name="times">
    ///     How many times the capture occured.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should be exactly number of times or at least.
    /// </param>
    /// <returns>
    ///     The current <see cref="InMemoryLogger"/> instance
    ///     so other actions can be chained.
    /// </returns>
    /// <exception cref="MockVerificationException">
    ///     If the verification fails.
    /// </exception>
    public InMemoryLogger VerifyTrace(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyLog(LogLevel.Trace, assertMessage, times, exactly);
    }

    /// <summary>
    ///     Verifies if a <see cref="LogLevel.Debug"/> message was captured.
    /// </summary>
    /// <param name="assertMessage">
    ///     Assert the message content.
    /// </param>
    /// <param name="times">
    ///     How many times the capture occured.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should be exactly number of times or at least.
    /// </param>
    /// <returns>
    ///     The current <see cref="InMemoryLogger"/> instance
    ///     so other actions can be chained.
    /// </returns>
    /// <exception cref="MockVerificationException">
    ///     If the verification fails.
    /// </exception>
    public InMemoryLogger VerifyDebug(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyLog(LogLevel.Debug, assertMessage, times, exactly);
    }

    /// <summary>
    ///     Verifies if a <see cref="LogLevel.Information"/> message was captured.
    /// </summary>
    /// <param name="assertMessage">
    ///     Assert the message content.
    /// </param>
    /// <param name="times">
    ///     How many times the capture occured.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should be exactly number of times or at least.
    /// </param>
    /// <returns>
    ///     The current <see cref="InMemoryLogger"/> instance
    ///     so other actions can be chained.
    /// </returns>
    /// <exception cref="MockVerificationException">
    ///     If the verification fails.
    /// </exception>
    public InMemoryLogger VerifyInformation(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyLog(LogLevel.Information, assertMessage, times, exactly);
    }

    /// <summary>
    ///     Verifies if a <see cref="LogLevel.Warning"/> message was captured.
    /// </summary>
    /// <param name="assertMessage">
    ///     Assert the message content.
    /// </param>
    /// <param name="times">
    ///     How many times the capture occured.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should be exactly number of times or at least.
    /// </param>
    /// <returns>
    ///     The current <see cref="InMemoryLogger"/> instance
    ///     so other actions can be chained.
    /// </returns>
    /// <exception cref="MockVerificationException">
    ///     If the verification fails.
    /// </exception>
    public InMemoryLogger VerifyWarning(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyLog(LogLevel.Warning, assertMessage, times, exactly);
    }

    /// <summary>
    ///     Verifies if a <see cref="LogLevel.Error"/> message was captured.
    /// </summary>
    /// <param name="assertMessage">
    ///     Assert the message content.
    /// </param>
    /// <param name="times">
    ///     How many times the capture occured.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should be exactly number of times or at least.
    /// </param>
    /// <returns>
    ///     The current <see cref="InMemoryLogger"/> instance
    ///     so other actions can be chained.
    /// </returns>
    /// <exception cref="MockVerificationException">
    ///     If the verification fails.
    /// </exception>
    public InMemoryLogger VerifyError(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyLog(LogLevel.Error, assertMessage, times, exactly);
    }

    /// <summary>
    ///     Verifies if a <see cref="LogLevel.Critical"/> message was captured.
    /// </summary>
    /// <param name="assertMessage">
    ///     Assert the message content.
    /// </param>
    /// <param name="times">
    ///     How many times the capture occured.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should be exactly number of times or at least.
    /// </param>
    /// <returns>
    ///     The current <see cref="InMemoryLogger"/> instance
    ///     so other actions can be chained.
    /// </returns>
    /// <exception cref="MockVerificationException">
    ///     If the verification fails.
    /// </exception>
    public InMemoryLogger VerifyCritical(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyLog(LogLevel.Critical, assertMessage, times, exactly);
    }

    /// <summary>
    ///     Removes every captured entry and scope.
    /// </summary>
    public void Clear() {
        lock (_sync) {
            _entries.Clear();
            _activeScopes.Clear();
            _capturedScopes.Clear();
        }
    }

    private InMemoryLogger VerifyLog(LogLevel level, Func<string, bool>? assertMessage, int times, bool exactly) {
        var predicate = assertMessage ?? (static _ => true);

        int actual;
        lock (_sync) {
            actual = _entries.Count(entry => entry.Level == level && predicate(entry.Message));
        }

        var matches = exactly
            ? actual == times
            : actual >= times;

        if (!matches) {
            throw new MockVerificationException(times, actual);
        }

        return this;
    }

    private void EndScope(object? state) {
        lock (_sync) {
            _activeScopes.Remove(state);
        }
    }

    private sealed class Scope(InMemoryLogger logger, object? state) : IDisposable {
        private bool _disposed;

        public void Dispose() {
            if (_disposed) { return; }

            _disposed = true;
            logger.EndScope(state);
        }
    }
}

/// <summary>
///     In-memory <see cref="ILogger{TCategoryName}"/> implementation for test scenarios.
/// </summary>
/// <typeparam name="TCategoryName">
///     Type of the log category.
/// </typeparam>
public sealed class InMemoryLogger<TCategoryName> : InMemoryLogger, ILogger<TCategoryName>;
