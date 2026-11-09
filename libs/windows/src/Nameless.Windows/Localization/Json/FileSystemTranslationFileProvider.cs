using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Configuration;
using Nameless.Contracts;

namespace Nameless.Windows.Localization.Json;

/// <summary>
///     Disk-backed <see cref="ITranslationFileProvider"/>. Reads each
///     culture's translation file (<c>{culture}.json</c>, or
///     <c>default.json</c> for the literal <c>"default"</c> key) from
///     <see cref="TranslatorOptions.RootPath"/>, and watches that same
///     folder with a <see cref="FileSystemWatcher"/> so edits made while
///     the app is running raise <see cref="Changed"/> (#191).
/// </summary>
/// <remarks>
///     Rapid, successive file-system events for the same file (a single
///     save can raise more than one; so can two quick edits) are coalesced
///     by a per-file debounce timer, configured through
///     <see cref="TranslatorOptions.WatcherDebounceDelay"/>, into a single
///     <see cref="Changed"/> raise.
/// </remarks>
public sealed class FileSystemTranslationFileProvider : ITranslationFileProvider, IDisposable {
    private readonly string _rootPath;
    private readonly TimeSpan _watcherDebounceDelay;
    private readonly ILogger<FileSystemTranslationFileProvider> _logger;
    private readonly ConcurrentDictionary<string, Timer> _debounceTimers = new(StringComparer.OrdinalIgnoreCase);
    private readonly FileSystemWatcher? _watcher;

    private bool _disposed;

    /// <inheritdoc />
    public event EventHandler<TranslationFileChangedEventArgs>? Changed;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="FileSystemTranslationFileProvider"/> class.
    /// </summary>
    /// <param name="options">
    ///     The translator options, providing <see cref="TranslatorOptions.RootPath"/>
    ///     and <see cref="TranslatorOptions.WatcherDebounceDelay"/>.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public FileSystemTranslationFileProvider(IOptions<TranslatorOptions> options, ILogger<FileSystemTranslationFileProvider> logger) {
        var optionsValue = Throws.When.Null(options).Value;
        var rootPath = Throws.When.NullOrWhiteSpace(
            optionsValue.RootPath,
            exceptionCreator: () => new MissingConfigurationException(
                section: ConfigurationSectionNameAttribute.GetSectionName<TranslatorOptions>(),
                key: nameof(TranslatorOptions.RootPath)
            )
        );

        _rootPath = SysPath.IsPathRooted(rootPath)
            ? rootPath
            : SysPath.Combine(AppContext.BaseDirectory, rootPath);
        _watcherDebounceDelay = Throws.When.Negative(optionsValue.WatcherDebounceDelay);
        _logger = Throws.When.Null(logger);

        _watcher = CreateWatcher();
    }

    /// <inheritdoc />
    public bool TryRead(string key, [NotNullWhen(true)] out string? content) {
        var fileName = string.Equals(key, "default", StringComparison.OrdinalIgnoreCase)
            ? "default.json"
            : $"{key}.json";
        var path = SysPath.Combine(_rootPath, fileName);

        if (!SysFile.Exists(path)) {
            content = null;

            return false;
        }

        try {
            content = SysFile.ReadAllText(path);

            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException) {
            // A locked or unreadable file should not crash JsonTranslator's
            // indexer for every caller — treat it like a missing file and
            // let the fallback chain (or the next lookup) take over.
            CommonLog.Error(_logger, message: $"Unable to read translation file '{path}'.", exception, tag: key);

            content = null;

            return false;
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        if (_disposed) {
            return;
        }

        _disposed = true;

        _watcher?.Dispose();

        foreach (var timer in _debounceTimers.Values) {
            timer.Dispose();
        }

        _debounceTimers.Clear();
    }

    // Directory may legitimately not exist yet (e.g. a host configured
    // with a RootPath nobody has populated). TryRead already treats a
    // missing file as "not found" rather than throwing — a missing root
    // folder just means there is nothing to watch; it is not fatal.
    private FileSystemWatcher? CreateWatcher() {
        if (!SysDirectory.Exists(_rootPath)) {
            CommonLog.Warning(_logger, message: $"Translation root path '{_rootPath}' does not exist; hot-reload is disabled.");

            return null;
        }

        var watcher = new FileSystemWatcher(_rootPath, "*.json") {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName,
        };

        watcher.Changed += OnFileSystemEvent;
        watcher.Created += OnFileSystemEvent;
        watcher.Renamed += OnFileSystemEvent;
        watcher.Error += OnWatcherError;

        watcher.EnableRaisingEvents = true;

        return watcher;
    }

    // An internal buffer overflow (too many changes too fast) or the
    // watched folder being deleted out from under the watcher raises
    // this instead of a normal change event, and otherwise fails
    // silently — hot-reload would just stop working with no trace.
    private void OnWatcherError(object sender, ErrorEventArgs e) {
        CommonLog.Error(_logger, message: $"Translation file watcher for '{_rootPath}' reported an error; hot-reload may stop receiving updates.", e.GetException());
    }

    private void OnFileSystemEvent(object sender, FileSystemEventArgs e) {
        var key = SysPath.GetFileNameWithoutExtension(e.Name);
        if (string.IsNullOrEmpty(key)) {
            return;
        }

        ScheduleReload(key);
    }

    // One timer per key, created once and kept for the provider's
    // lifetime — a new file-system event for an already-watched key just
    // re-arms the existing timer's due time via Change(), instead of
    // removing and recreating it. Removing+disposing+recreating left a
    // microsecond window where a trailing edit's reschedule could be lost
    // if the old timer's callback was mid-flight (already past
    // TryRemove, about to invoke Changed) when the new event arrived —
    // the new timer would then be scheduled, but nothing had cancelled
    // the in-flight raise, and the dictionary briefly had no entry for
    // the key at all. Never removing the timer also means a late
    // in-flight callback can no longer resurrect an entry after Dispose()
    // has cleared the dictionary.
    private void ScheduleReload(string key) {
        if (_disposed) {
            return;
        }

        _debounceTimers.AddOrUpdate(
            key,
            addValueFactory: static (addedKey, state) => new Timer(RaiseChanged, (state.Self, addedKey), state.Delay, Timeout.InfiniteTimeSpan),
            updateValueFactory: static (_, existingTimer, state) => {
                existingTimer.Change(state.Delay, Timeout.InfiniteTimeSpan);

                return existingTimer;
            },
            factoryArgument: (Self: this, Delay: _watcherDebounceDelay));
    }

    private static void RaiseChanged(object? state) {
        var (self, key) = ((FileSystemTranslationFileProvider, string))state!;

        if (self._disposed) {
            return;
        }

        self.Changed?.Invoke(self, new TranslationFileChangedEventArgs(key));
    }
}
