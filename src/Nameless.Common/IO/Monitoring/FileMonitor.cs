using System.Threading.Channels;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Nameless.IO.Monitoring;

/// <summary>
///     <see cref="IFileMonitor" /> implementation on top of
///     <see cref="IFileSystemWatcherAdapter" />.
/// </summary>
/// <remarks>
///     Created and changed files are tracked per path. Each raw event restarts
///     a quiet-period timer; once it elapses the file is probed for exclusive
///     access, retrying with back-off (from
///     <see cref="FileMonitorOptions.ProbeInterval" /> up to
///     <see cref="FileMonitorOptions.MaxProbeInterval" />) for as long as it
///     stays locked. Past <see cref="FileMonitorOptions.LockedTooLongAfter" />
///     a <see cref="FileLockedTooLongException" /> is reported once and
///     probing continues. Renamed and deleted events pass through.
///     Notifications are queued in decision order and delivered one at a time
///     by a single consumer, so handlers never overlap and a slow handler
///     cannot stall the watcher.
/// </remarks>
public sealed class FileMonitor : IFileMonitor {
    private static readonly StringComparer PathComparer = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    private static readonly StringComparison MatcherComparison = OperatingSystem.IsWindows()
        ? StringComparison.OrdinalIgnoreCase
        : StringComparison.Ordinal;

    private readonly IFileSystemWatcherAdapter _watcher;
    private readonly IFileProbe _probe;
    private readonly TimeProvider _timeProvider;
    private readonly FileMonitorOptions _options;
    private readonly Matcher _matcher;
    private readonly bool _includeSubdirectories;
    private readonly Dictionary<string, PendingFile> _pending = new(PathComparer);
    private readonly Dictionary<string, VacatedPath> _vacated = new(PathComparer);
    private readonly Lock _gate = new();
    private readonly Channel<WorkItem> _queue = Channel.CreateUnbounded<WorkItem>(new UnboundedChannelOptions { SingleReader = true });

    private Action<FileCreatedEvent>? _onCreated;
    private Action<FileChangedEvent>? _onChanged;
    private Action<FileDeletedEvent>? _onDeleted;
    private Action<FileRenamedEvent>? _onRenamed;
    private Action<FileMonitorErrorEvent>? _onError;
    private bool _started;
    private bool _disposed;

    /// <inheritdoc />
    public string Root { get; }

    /// <inheritdoc />
    public string Glob { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileMonitor" /> class.
    /// </summary>
    /// <param name="root">
    ///     The folder to monitor.
    /// </param>
    /// <param name="glob">
    ///     The glob pattern, relative to <paramref name="root" />, that paths
    ///     must match.
    /// </param>
    /// <param name="watcher">
    ///     The watcher that supplies raw file system events.
    /// </param>
    /// <param name="probe">
    ///     The probe used to check file availability.
    /// </param>
    /// <param name="timeProvider">
    ///     The clock used for quiet-period and retry timers.
    /// </param>
    /// <param name="options">
    ///     The tuning options; defaults are used when
    ///     <see langword="null" />.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     <paramref name="root" /> or <paramref name="glob" /> is null,
    ///     empty or blank.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     A required dependency is <see langword="null" />.
    /// </exception>
    public FileMonitor(string root, string glob, IFileSystemWatcherAdapter watcher, IFileProbe probe, TimeProvider timeProvider, FileMonitorOptions? options = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentException.ThrowIfNullOrWhiteSpace(glob);
        ArgumentNullException.ThrowIfNull(watcher);
        ArgumentNullException.ThrowIfNull(probe);
        ArgumentNullException.ThrowIfNull(timeProvider);

        Root = SysPath.GetFullPath(root);
        Glob = glob;

        _watcher = watcher;
        _probe = probe;
        _timeProvider = timeProvider;
        _options = options ?? new FileMonitorOptions();

        _matcher = new Matcher(MatcherComparison);
        _matcher.AddInclude(glob.Replace('\\', '/'));

        foreach (var pattern in _options.Excludes) {
            _matcher.AddExclude(pattern.Replace('\\', '/'));
        }

        _includeSubdirectories = glob.Contains("**") ||
                                 glob.Contains('/') ||
                                 glob.Contains('\\');

        _watcher.Created += HandleCreated;
        _watcher.Changed += HandleChanged;
        _watcher.Deleted += HandleDeleted;
        _watcher.Renamed += HandleRenamed;
        _watcher.Error += HandleError;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="action" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     The monitoring already started or the event already has a handler.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    ///     The monitoring was disposed.
    /// </exception>
    public void OnCreated(Action<FileCreatedEvent> action) {
        SetHandler(ref _onCreated, action);
    }

    /// <inheritdoc cref="OnCreated(Action{FileCreatedEvent})" />
    public void OnRenamed(Action<FileRenamedEvent> action) {
        SetHandler(ref _onRenamed, action);
    }

    /// <inheritdoc cref="OnCreated(Action{FileCreatedEvent})" />
    public void OnDeleted(Action<FileDeletedEvent> action) {
        SetHandler(ref _onDeleted, action);
    }

    /// <inheritdoc cref="OnCreated(Action{FileCreatedEvent})" />
    public void OnChanged(Action<FileChangedEvent> action) {
        SetHandler(ref _onChanged, action);
    }

    /// <inheritdoc cref="OnCreated(Action{FileCreatedEvent})" />
    public void OnError(Action<FileMonitorErrorEvent> action) {
        SetHandler(ref _onError, action);
    }

    /// <inheritdoc />
    /// <exception cref="ObjectDisposedException">
    ///     The monitoring was disposed.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     The watcher rejected <see cref="Root" />, for example because it
    ///     does not exist.
    /// </exception>
    public void Start() {
        lock (_gate) {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_started) { return; }

            _watcher.Path = Root;
            _watcher.IncludeSubdirectories = _includeSubdirectories;
            _watcher.InternalBufferSize = _options.InternalBufferSize;
            _watcher.EnableRaisingEvents = true;

            _started = true;

            _ = Task.Run(ConsumeAsync);
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        lock (_gate) {
            if (_disposed) { return; }

            _disposed = true;
            _queue.Writer.TryComplete();

            foreach (var pending in _pending.Values) {
                pending.Timer.Dispose();
            }

            _pending.Clear();

            foreach (var vacated in _vacated.Values) {
                vacated.Timer!.Dispose();
            }

            _vacated.Clear();
        }

        _watcher.Created -= HandleCreated;
        _watcher.Changed -= HandleChanged;
        _watcher.Deleted -= HandleDeleted;
        _watcher.Renamed -= HandleRenamed;
        _watcher.Error -= HandleError;

        _watcher.Dispose();
    }

    /// <summary>
    ///     Completes when every notification queued before the call was
    ///     delivered or discarded.
    /// </summary>
    /// <returns>
    ///     A task that completes once the queue drained; already complete
    ///     when not running.
    /// </returns>
    internal Task WhenIdleAsync() {
        var idle = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        lock (_gate) {
            if (!_started || _disposed) { return Task.CompletedTask; }

            _queue.Writer.TryWrite(
                new WorkItem(
                    Run: () => idle.TrySetResult(),
                    RunAfterDispose: true
                )
            );
        }

        return idle.Task;
    }

    private void SetHandler<T>(ref Action<T>? slot, Action<T> action) {
        ArgumentNullException.ThrowIfNull(action);

        lock (_gate) {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_started) {
                throw new InvalidOperationException(
                    "Handlers cannot be registered after the monitoring started."
                );
            }

            if (slot is not null) {
                throw new InvalidOperationException(
                    $"A handler for {typeof(T).Name} is already registered."
                );
            }

            slot = action;
        }
    }

    private void HandleCreated(object? sender, FileSystemEventArgs args) {
        if (!Matches(args.FullPath)) { return; }

        lock (_gate) {
            // A path that was just vacated is being replaced, not created.
            var pending = Reclaim(args.FullPath)
                ? PendingKind.Changed
                : PendingKind.Created;

            Track(args.FullPath, pending);
        }
    }

    private void HandleChanged(object? sender, FileSystemEventArgs args) {
        if (Matches(args.FullPath)) {
            Track(args.FullPath, PendingKind.Changed);
        }
    }

    private void HandleDeleted(object? sender, FileSystemEventArgs args) {
        if (!Matches(args.FullPath)) { return; }

        lock (_gate) {
            if (_disposed) { return; }

            // The file was never announced, so its deletion is not worth
            // announcing either.
            if (Discard(args.FullPath) == PendingKind.Created) { return; }

            if (_options.ReplaceGracePeriod > TimeSpan.Zero) {
                Vacate(args.FullPath, announceDeletion: true);
                return;
            }

            Notify(_onDeleted, new FileDeletedEvent(args.FullPath));
        }
    }

    private void HandleRenamed(object? sender, RenamedEventArgs args) {
        lock (_gate) {
            if (_disposed) { return; }

            var dropped = Discard(args.OldFullPath);

            if (_options.ReplaceGracePeriod > TimeSpan.Zero) {
                if (dropped != PendingKind.Created && Matches(args.OldFullPath)) {
                    Vacate(args.OldFullPath, announceDeletion: false);
                }

                if (Reclaim(args.FullPath)) {
                    Track(args.FullPath, PendingKind.Changed);
                    return;
                }
            }
        }

        if (!Matches(args.FullPath) || TryProbe(args.FullPath) is FileProbeResult.Directory) {
            return;
        }

        lock (_gate) {
            if (!_disposed) {
                Notify(_onRenamed, new FileRenamedEvent(args.OldFullPath, args.FullPath));
            }
        }
    }

    private void HandleError(object? sender, ErrorEventArgs e) {
        EnqueueError(e.GetException());
    }

    private bool Matches(string path) {
        const string Back = "..";

        var relative = SysPath.GetRelativePath(Root, path);
        var outsideRoot = relative == Back
                          || relative.StartsWith($"{Back}{SysPath.DirectorySeparatorChar}", StringComparison.Ordinal)
                          || SysPath.IsPathRooted(relative);

        return !outsideRoot && _matcher.Match(Root, relative.Replace('\\', '/')).HasMatches;
    }

    private void Track(string path, PendingKind kind) {
        lock (_gate) {
            if (_disposed) { return; }

            if (!_pending.TryGetValue(path, out var pending)) {
                var timer = _timeProvider.CreateTimer(
                    callback: OnTimer,
                    state: path,
                    dueTime: Timeout.InfiniteTimeSpan,
                    period: Timeout.InfiniteTimeSpan
                );

                pending = new PendingFile(kind, timer);

                _pending[path] = pending;
            }
            else if (kind == PendingKind.Created) {
                pending.Kind = PendingKind.Created;
            }

            pending.LastActivity = _timeProvider.GetUtcNow();
            pending.ProbingSince = null;
            pending.NextProbeDelay = _options.ProbeInterval;
            pending.LockedTooLongReported = false;
            pending.Version++;
            pending.Timer.Change(_options.QuietPeriod, Timeout.InfiniteTimeSpan);
        }
    }

    private void OnTimer(object? state) {
        var path = (string)state!;
        long version;

        lock (_gate) {
            if (_disposed || !_pending.TryGetValue(path, out var pending)) {
                return;
            }

            var now = _timeProvider.GetUtcNow();
            if (pending.ProbingSince is null) {
                // Timers are re-armed rather than recreated, so a stale
                // firing can arrive early.
                var remaining = pending.LastActivity + _options.QuietPeriod - now;
                if (remaining > TimeSpan.Zero) {
                    pending.Timer.Change(remaining, Timeout.InfiniteTimeSpan);
                    return;
                }

                pending.ProbingSince = now;
            }

            version = pending.Version;
        }

        var result = TryProbe(path);

        lock (_gate) {
            // A newer raw event bumps the version and has already re-armed
            // the timer.
            if (_disposed || !_pending.TryGetValue(path, out var pending) || pending.Version != version) {
                return;
            }

            switch (result) {
                case FileProbeResult.Available:
                    Discard(path);

                    if (pending.Kind == PendingKind.Created) {
                        Notify(_onCreated, new FileCreatedEvent(path));
                    }
                    else {
                        Notify(_onChanged, new FileChangedEvent(path));
                    }

                    break;

                case FileProbeResult.Locked:
                    var lockedFor = _timeProvider.GetUtcNow() - pending.ProbingSince!.Value;
                    if (!pending.LockedTooLongReported && lockedFor >= _options.LockedTooLongAfter) {
                        pending.LockedTooLongReported = true;

                        EnqueueError(new FileLockedTooLongException(path, lockedFor));
                    }

                    pending.Timer.Change(pending.NextProbeDelay, Timeout.InfiniteTimeSpan);
                    pending.NextProbeDelay = Backoff(pending.NextProbeDelay);

                    break;

                default:
                    Discard(path);
                    break;
            }
        }
    }

    private TimeSpan Backoff(TimeSpan delay) {
        var doubled = Math.Min(delay.Ticks * 2, _options.MaxProbeInterval.Ticks);

        return TimeSpan.FromTicks(Math.Max(delay.Ticks, doubled));
    }

    private void Vacate(string path, bool announceDeletion) {
        Reclaim(path);

        var vacated = new VacatedPath(path, announceDeletion);

        vacated.Timer = _timeProvider.CreateTimer(
            callback: OnVacatedExpired,
            state: vacated,
            dueTime: _options.ReplaceGracePeriod,
            period: Timeout.InfiniteTimeSpan
        );

        _vacated[path] = vacated;
    }

    private bool Reclaim(string path) {
        if (!_vacated.Remove(path, out var vacated)) {
            return false;
        }

        vacated.Timer!.Dispose();

        return true;
    }

    private void OnVacatedExpired(object? state) {
        var expired = (VacatedPath)state!;

        lock (_gate) {
            // The entry may have been reclaimed or replaced while this firing was queued.
            if (_disposed || !_vacated.TryGetValue(expired.FullPath, out var current) || !ReferenceEquals(current, expired)) {
                return;
            }

            Reclaim(expired.FullPath);

            if (expired.AnnounceDeletion) {
                Notify(_onDeleted, new FileDeletedEvent(expired.FullPath));
            }
        }
    }

    private PendingKind? Discard(string path) {
        if (!_pending.Remove(path, out var pending)) {
            return null;
        }

        pending.Timer.Dispose();

        return pending.Kind;
    }

    private FileProbeResult? TryProbe(string fullPath) {
        try { return _probe.Probe(fullPath); }
        catch (Exception ex) {
            EnqueueError(ex);

            return null;
        }
    }

    // Callers that decide an outcome under _gate enqueue there too, so queue
    // order is decision order.
    private void Notify<T>(Action<T>? handler, T evt) {
        Enqueue(() => Invoke(handler, evt));
    }

    private void EnqueueError(Exception exception) {
        Enqueue(() => InvokeErrorHandler(exception));
    }

    private void Enqueue(Action work) {
        _queue.Writer.TryWrite(new WorkItem(work, RunAfterDispose: false));
    }

    private async Task ConsumeAsync() {
        await foreach (var item in _queue.Reader.ReadAllAsync().ConfigureAwait(false)) {
            if (Volatile.Read(ref _disposed) && !item.RunAfterDispose) {
                continue;
            }

            item.Run();
        }
    }

    private void Invoke<T>(Action<T>? handler, T evt) {
        try { handler?.Invoke(evt); }
        catch (Exception ex) { InvokeErrorHandler(ex); }
    }

    private void InvokeErrorHandler(Exception exception) {
        try { _onError?.Invoke(new FileMonitorErrorEvent(exception)); }
        catch { /* Nothing left to report to: the error handler itself failed. */ }
    }

    private readonly record struct WorkItem(Action Run, bool RunAfterDispose);

    private enum PendingKind {
        Created,
        Changed
    }

    private sealed class VacatedPath(string fullPath, bool announceDeletion) {
        public string FullPath { get; } = fullPath;

        public bool AnnounceDeletion { get; } = announceDeletion;

        public ITimer? Timer { get; set; }
    }

    private sealed class PendingFile(PendingKind kind, ITimer timer) {
        public PendingKind Kind { get; set; } = kind;

        public ITimer Timer { get; } = timer;

        public DateTimeOffset LastActivity { get; set; }

        public DateTimeOffset? ProbingSince { get; set; }

        public TimeSpan NextProbeDelay { get; set; }

        public bool LockedTooLongReported { get; set; }

        public long Version { get; set; }
    }
}
