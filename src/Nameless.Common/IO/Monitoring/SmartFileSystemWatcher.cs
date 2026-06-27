using System.Collections.Concurrent;
using System.IO.Enumeration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Resilience;

namespace Nameless.IO.Monitoring;

/// <summary>
///     An improved file system watcher that uses <see cref="IFileProvider"/>
///     change tokens and snapshot diffing to detect file events,
///     firing callbacks only after write operations are fully complete
///     (via exclusive-lock probing).
/// </summary>
public sealed class SmartFileSystemWatcher : IFileSystemWatcher {
    private readonly IFileProvider _fileProvider;
    private readonly IRetryPipelineFactory? _retryFactory;
    private readonly SmartFileSystemWatcherOptions _options;
    private readonly ILogger<SmartFileSystemWatcher> _logger;

    private FileWatcherDelegate<FileWatcherEventArgs>? _onCreated;
    private FileWatcherDelegate<FileWatcherEventArgs>? _onDeleted;
    private FileWatcherDelegate<FileWatcherEventArgs>? _onRenamed;
    private FileWatcherDelegate<FileWatcherEventArgs>? _onChanged;
    private FileWatcherDelegate<FileWatcherErrorEventArgs>? _onError;

    private Dictionary<string, IFileInfo> _snapshot = [];
    private readonly ConcurrentDictionary<string, Task> _activeProbes = new();
    private IDisposable? _tokenRegistration;
    private CancellationTokenSource? _cts;
    private bool _disposed;
    
    /// <summary>
    ///     Initializes a new instance of <see cref="SmartFileSystemWatcher"/>.
    /// </summary>
    /// <param name="fileProvider">
    ///     The file provider used to watch for changes and enumerate files.
    /// </param>
    /// <param name="retryFactory">
    ///     Optional factory for retry pipelines; required only when
    ///     <see cref="SmartFileSystemWatcherOptions.EnableRetry"/>
    ///     is <see langword="true"/>.
    /// </param>
    /// <param name="options">
    ///     Configuration for this watcher instance.
    /// </param>
    /// <param name="logger">
    ///     Logger for diagnostics.
    /// </param>
    public SmartFileSystemWatcher(IFileProvider fileProvider, IRetryPipelineFactory? retryFactory, IOptions<SmartFileSystemWatcherOptions> options, ILogger<SmartFileSystemWatcher> logger) {
        _fileProvider = Throws.When.Null(fileProvider);
        _retryFactory = retryFactory;
        _options = Throws.When.Null(options).Value;
        _logger = Throws.When.Null(logger);
    }

    /// <inheritdoc />
    public void OnCreated(FileWatcherDelegate<FileWatcherEventArgs> callback) {
        _onCreated = Throws.When.Null(callback);
    }

    /// <inheritdoc />
    public void OnDeleted(FileWatcherDelegate<FileWatcherEventArgs> callback) {
        _onDeleted = Throws.When.Null(callback);
    }

    /// <inheritdoc />
    public void OnRenamed(FileWatcherDelegate<FileWatcherEventArgs> callback) {
        _onRenamed = Throws.When.Null(callback);
    }

    /// <inheritdoc />
    public void OnChanged(FileWatcherDelegate<FileWatcherEventArgs> callback) {
        _onChanged = Throws.When.Null(callback);
    }

    /// <inheritdoc />
    public void OnError(FileWatcherDelegate<FileWatcherErrorEventArgs> callback) {
        _onError = Throws.When.Null(callback);
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken = default) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _snapshot = TakeSnapshot();

        await RegisterWatchAsync(_cts.Token).SkipContextSync();
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken = default) {
        if (_disposed) { return; }

        if (_cts is not null) {
            await _cts.CancelAsync();
        }

        _tokenRegistration?.Dispose();
        _tokenRegistration = null;

        var probes = _activeProbes.Values.ToArray();
        if (probes.Length > 0) {
            try {
                await Task.WhenAll(probes).WaitAsync(cancellationToken).SkipContextSync();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { _logger.LogError(ex, ex.Message); }
        }

        _activeProbes.Clear();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync() {
        if (_disposed) { return; }

        _disposed = true;

        await StopAsync(CancellationToken.None).SkipContextSync();

        _cts?.Dispose();
        _cts = null;
    }

    private Task RegisterWatchAsync(CancellationToken cancellationToken) {
        if (cancellationToken.IsCancellationRequested) {
            return Task.CompletedTask;
        }

        if (!_options.EnableRetry || _retryFactory is null) {
            return RegisterWatchCore();
        }

        var config = _options.RetryPolicy ?? RetryPolicyConfiguration.CreateDefault(onRetry: (_, _, _, _) => { });
        var pipeline = _retryFactory.Create(config);

        return pipeline.ExecuteAsync(_ => new ValueTask(RegisterWatchCore()), cancellationToken).AsTask();

    }

    private Task RegisterWatchCore() {
        var watchFilter = BuildWatchFilter();
        var token = _fileProvider.Watch(watchFilter);

        if (!token.ActiveChangeCallbacks) {
            throw new SmartFileSystemWatcherException(
                $"""
                 The file provider does not support active change callbacks for filter '{watchFilter}'.
                 Use a provider that returns a change token with ActiveChangeCallbacks = true (e.g. PhysicalFileProvider).
                 """
            );
        }

        _tokenRegistration?.Dispose();
        _tokenRegistration = token.RegisterChangeCallback(OnChangeDetected, state: null);

        return Task.CompletedTask;
    }

    private void OnChangeDetected(object? state) {
        if (_cts?.Token.IsCancellationRequested == true) { return; }

        var cancellationToken = _cts?.Token ?? CancellationToken.None;
        var newSnapshot = TakeSnapshot();

        ProcessChanges(newSnapshot, cancellationToken);

        _snapshot = newSnapshot;

        // Tokens are one-shot; re-arm for the next change.
        _ = RegisterWatchAsync(cancellationToken);
    }

    private void ProcessChanges(Dictionary<string, IFileInfo> newSnapshot, CancellationToken cancellationToken) {
        var added = newSnapshot.Keys.Except(_snapshot.Keys).ToList();
        var removed = _snapshot.Keys.Except(newSnapshot.Keys).ToList();
        var changed = newSnapshot.Keys
            .Intersect(_snapshot.Keys)
            .Where(name => {
                var old = _snapshot[name];
                var next = newSnapshot[name];
                return old.Length != next.Length || old.LastModified != next.LastModified;
            })
            .ToList();

        // Rename heuristic: exactly 1 added + 1 removed in the same batch.
        if (added.Count == 1 && removed.Count == 1) {
            var oldInfo = _snapshot[removed[0]];
            var newInfo = newSnapshot[added[0]];
            var args = new FileWatcherEventArgs(
                CurrentFilePath: GetPath(newInfo),
                PreviousFilePath: GetPath(oldInfo)
            );

            FireCallback(_onRenamed, args, cancellationToken);
            added.Clear();
            removed.Clear();
        }

        foreach (var name in removed) {
            var info = _snapshot[name];
            FireCallback(
                callback: _onDeleted,
                args: new FileWatcherEventArgs(GetPath(info), PreviousFilePath: null),
                cancellationToken: cancellationToken
            );
        }

        foreach (var name in added) {
            ScheduleLockProbe(newSnapshot[name], EventKind.Created, cancellationToken);
        }

        foreach (var name in changed) {
            ScheduleLockProbe(newSnapshot[name], EventKind.Changed, cancellationToken);
        }
    }

    private void ScheduleLockProbe(IFileInfo fileInfo, EventKind kind, CancellationToken cancellationToken) {
        var physicalPath = fileInfo.PhysicalPath;

        if (physicalPath is null) {
            // Cannot probe without a physical path; fire the callback immediately.
            _logger.LogDebug(
                "File '{Name}' has no physical path. Firing callback without lock probe.",
                fileInfo.Name
            );

            var args = new FileWatcherEventArgs(fileInfo.Name, PreviousFilePath: null);
            var callback = kind == EventKind.Created ? _onCreated : _onChanged;

            FireCallback(callback, args, cancellationToken);
            
            return;
        }

        // GetOrAdd ensures only one probe per physical path runs at a time.
        _activeProbes.GetOrAdd(
            physicalPath,
            key => Task.Run(async () => {
                try { await ProbeFileAsync(key, kind, cancellationToken).SkipContextSync(); }
                finally { _activeProbes.TryRemove(key, out _); }
            }, cancellationToken)
        );
    }

    private async Task ProbeFileAsync(string physicalPath, EventKind kind, CancellationToken cancellationToken) {
        var attempts = 0;

        while (attempts < _options.LockProbeMaxAttempts && !cancellationToken.IsCancellationRequested) {
            try {
                await using (File.Open(physicalPath, FileMode.Open, FileAccess.Read, FileShare.None)) { }

                var args = new FileWatcherEventArgs(physicalPath, PreviousFilePath: null);
                var callback = kind == EventKind.Created ? _onCreated : _onChanged;

                FireCallback(callback, args, cancellationToken);
                
                return;
            }
            catch (IOException) {
                attempts++;
                
                await Task.Delay(_options.LockProbeDelay, cancellationToken).SkipContextSync();
            }
        }

        if (!cancellationToken.IsCancellationRequested) {
            var error = new TimeoutException(
                $"File '{physicalPath}' was not available after {_options.LockProbeMaxAttempts} exclusive-lock probe attempts."
            );

            FireCallback(_onError, new FileWatcherErrorEventArgs(error), cancellationToken);
        }
    }

    private void FireCallback<TArgs>(FileWatcherDelegate<TArgs>? callback, TArgs args, CancellationToken cancellationToken) {
        if (callback is null) { return; }

        _ = Task.Run(async () => {
            try { await callback(args, cancellationToken).ConfigureAwait(false); }
            catch (OperationCanceledException) { }
            catch (Exception ex) {
                _logger.LogError(ex, "Unhandled exception in file watcher callback.");
            }
        }, cancellationToken);
    }

    private Dictionary<string, IFileInfo> TakeSnapshot() {
        return _fileProvider.GetDirectoryContents(_options.SubPath)
                            .Where(file => !file.IsDirectory && MatchesFilter(file.Name))
                            .ToDictionary(file => file.Name, file => file);
    }

    private bool MatchesFilter(string fileName) {
        return FileSystemName.MatchesSimpleExpression(_options.Filter, fileName, ignoreCase: true);
    }

    private string BuildWatchFilter() {
        return string.IsNullOrEmpty(_options.SubPath)
            ? _options.Filter
            : $"{_options.SubPath.TrimEnd('/', '\\')}/{_options.Filter}";
    }

    private static string GetPath(IFileInfo fileInfo) {
        return fileInfo.PhysicalPath ?? fileInfo.Name;
    }

    private enum EventKind { Created, Changed }
}
