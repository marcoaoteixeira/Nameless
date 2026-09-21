using Microsoft.Extensions.Time.Testing;
using Moq;

namespace Nameless.IO.Monitoring;

/// <summary>
/// Wires a <see cref="FileMonitor" /> to a mocked watcher/probe and a fake clock.
/// </summary>
internal sealed class FileMonitorFixture : IDisposable
{
    private readonly Dictionary<string, Queue<FileProbeResult>> _probes = new(StringComparer.OrdinalIgnoreCase);

    public FileMonitorFixture(
        string glob = "**",
        bool register = true,
        bool start = true,
        Action<FileMonitorOptions>? configure = null)
    {
        Root = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "fm-root"));
        Options = new FileMonitorOptions
        {
            QuietPeriod = TimeSpan.FromMilliseconds(500),
            ProbeInterval = TimeSpan.FromMilliseconds(200),
            MaxProbeInterval = TimeSpan.FromMilliseconds(200),
            LockedTooLongAfter = TimeSpan.FromMilliseconds(2000),
            ReplaceGracePeriod = TimeSpan.FromMilliseconds(300),
            InternalBufferSize = 32 * 1024
        };
        configure?.Invoke(Options);

        Watcher.SetupAllProperties();
        Probe.Setup(p => p.Probe(It.IsAny<string>())).Returns((string path) => NextProbe(path));

        Monitor = new FileMonitor(Root, glob, Watcher.Object, Probe.Object, Time, Options);

        if (register)
        {
            Monitor.OnCreated(e => Created.Add(e));
            Monitor.OnChanged(e => Changed.Add(e));
            Monitor.OnDeleted(e => Deleted.Add(e));
            Monitor.OnRenamed(e => Renamed.Add(e));
            Monitor.OnError(e => Errors.Add(e));
        }

        if (start)
        {
            Monitor.Start();
        }
    }

    public string Root { get; }

    public FileMonitorOptions Options { get; }

    public FakeTimeProvider Time { get; } = new();

    public Mock<IFileSystemWatcherAdapter> Watcher { get; } = new();

    public Mock<IFileProbe> Probe { get; } = new();

    public FileMonitor Monitor { get; }

    public List<FileCreatedEvent> Created { get; } = [];

    public List<FileChangedEvent> Changed { get; } = [];

    public List<FileDeletedEvent> Deleted { get; } = [];

    public List<FileRenamedEvent> Renamed { get; } = [];

    public List<FileMonitorErrorEvent> Errors { get; } = [];

    public string FullPath(string relative)
    {
        return Path.Combine(Root, relative);
    }

    /// <summary>
    /// Scripts consecutive probe results for a path. The last result repeats.
    /// </summary>
    public void ProbeReturns(string relative, params FileProbeResult[] results)
    {
        _probes[FullPath(relative)] = new Queue<FileProbeResult>(results);
    }

    /// <summary>
    /// Advances the clock in 1ms steps: <see cref="FakeTimeProvider" /> jumps straight to the target time,
    /// which would hide timers re-armed by callbacks.
    /// </summary>
    public void Advance(int milliseconds)
    {
        for (var i = 0; i < milliseconds; i++)
        {
            Time.Advance(TimeSpan.FromMilliseconds(1));
        }

        FlushIfAuto();
    }

    /// <summary>
    /// Gets or sets whether raising events and advancing time wait for handlers to finish.
    /// Handlers run on the monitor's consumer, so tests must wait before asserting.
    /// </summary>
    public bool AutoFlush { get; set; } = true;

    /// <summary>
    /// Blocks until every notification queued so far was delivered.
    /// </summary>
    public void Flush()
    {
        if (!Monitor.WhenIdleAsync().Wait(TimeSpan.FromSeconds(5)))
        {
            throw new TimeoutException("Monitor did not become idle.");
        }
    }

    public void RaiseCreated(string relative)
    {
        Raise(() => Watcher.Raise(w => w.Created += null, Watcher.Object,
            new FileSystemEventArgs(WatcherChangeTypes.Created, Root, relative)));
    }

    public void RaiseChanged(string relative)
    {
        Raise(() => Watcher.Raise(w => w.Changed += null, Watcher.Object,
            new FileSystemEventArgs(WatcherChangeTypes.Changed, Root, relative)));
    }

    public void RaiseDeleted(string relative)
    {
        Raise(() => Watcher.Raise(w => w.Deleted += null, Watcher.Object,
            new FileSystemEventArgs(WatcherChangeTypes.Deleted, Root, relative)));
    }

    public void RaiseRenamed(string oldRelative, string relative)
    {
        Raise(() => Watcher.Raise(w => w.Renamed += null, Watcher.Object,
            new RenamedEventArgs(WatcherChangeTypes.Renamed, Root, relative, oldRelative)));
    }

    public void RaiseError(Exception exception)
    {
        Raise(() => Watcher.Raise(w => w.Error += null, Watcher.Object, new ErrorEventArgs(exception)));
    }

    private void Raise(Action raise)
    {
        raise();
        FlushIfAuto();
    }

    private void FlushIfAuto()
    {
        if (AutoFlush)
        {
            Flush();
        }
    }

    public void Dispose()
    {
        Monitor.Dispose();
    }

    private FileProbeResult NextProbe(string path)
    {
        if (!_probes.TryGetValue(path, out var queue))
        {
            return FileProbeResult.Available;
        }

        return queue.Count > 1 ? queue.Dequeue() : queue.Peek();
    }
}
