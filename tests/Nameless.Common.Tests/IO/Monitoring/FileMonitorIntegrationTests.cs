using System.Collections.Concurrent;

namespace Nameless.IO.Monitoring;

/// <summary>
/// Runs the monitor against the real <see cref="FileSystemWatcher" /> and disk.
/// </summary>
[Trait("Category", "Integration")]
public sealed class FileMonitorIntegrationTests : IDisposable
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    private readonly string _dir = SysDirectory.CreateTempSubdirectory("monitor-").FullName;
    private FileMonitor _monitor;
    private readonly ConcurrentQueue<string> _log = new();
    private readonly TaskCompletionSource<bool> _created = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<bool> _changed = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<bool> _deleted = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<bool> _renamed = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public FileMonitorIntegrationTests()
    {
        _monitor = Build("**");
    }

    private FileMonitor Build(string glob, Action<FileMonitorOptions>? configure = null)
    {
        var options = new FileMonitorOptions
        {
            QuietPeriod = TimeSpan.FromMilliseconds(300),
            ProbeInterval = TimeSpan.FromMilliseconds(100),
            MaxProbeInterval = TimeSpan.FromMilliseconds(200)
        };
        configure?.Invoke(options);

        var monitor = new FileMonitor(
            _dir,
            glob,
            new FileSystemWatcherAdapter(),
            new FileProbe(),
            TimeProvider.System,
            options);

        monitor.OnCreated(e => { _log.Enqueue($"created:{e.Name}"); _created.TrySetResult(true); });
        monitor.OnChanged(e => { _log.Enqueue($"changed:{e.Name}"); _changed.TrySetResult(true); });
        monitor.OnDeleted(e => { _log.Enqueue($"deleted:{e.Name}"); _deleted.TrySetResult(true); });
        monitor.OnRenamed(e => { _log.Enqueue($"renamed:{e.PreviousName}->{e.Name}"); _renamed.TrySetResult(true); });
        monitor.OnError(e => _log.Enqueue($"error:{e.Exception.GetType().Name}"));

        return monitor;
    }

    public void Dispose()
    {
        _monitor.Dispose();
        SysDirectory.Delete(_dir, recursive: true);
    }

    [Fact]
    public async Task Created_SlowWrite_RaisedOnceAfterWriterCloses()
    {
        _monitor.Start();
        var path = Path.Combine(_dir, "big.bin");

        await using (var writer = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
            for (var i = 0; i < 6; i++)
            {
                await writer.WriteAsync(new byte[4096]);
                await writer.FlushAsync();
                await Task.Delay(100);
            }

            Assert.False(_created.Task.IsCompleted);
            _log.Enqueue("writer-closed");
        }

        await _created.Task.WaitAsync(Timeout);
        await Task.Delay(800);

        Assert.Equal(["writer-closed", "created:big.bin"], _log.ToArray());
    }

    [Fact]
    public async Task Changed_FileHeldByEditor_RaisedOnceAfterRelease()
    {
        var path = Path.Combine(_dir, "doc.txt");
        File.WriteAllText(path, "v0");
        _monitor.Start();

        await using (var editor = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.Read))
        {
            for (var i = 0; i < 4; i++)
            {
                await editor.WriteAsync(new byte[] { 1, 2, 3 });
                await editor.FlushAsync();
                await Task.Delay(150);
            }

            _log.Enqueue("editor-closed");
        }

        await _changed.Task.WaitAsync(Timeout);
        await Task.Delay(800);

        Assert.Equal(["editor-closed", "changed:doc.txt"], _log.ToArray());
    }

    [Fact]
    public async Task Renamed_SameFolder_RaisesRenamedOnly()
    {
        var oldPath = Path.Combine(_dir, "old.txt");
        File.WriteAllText(oldPath, "x");
        _monitor.Start();

        File.Move(oldPath, Path.Combine(_dir, "new.txt"));

        await _renamed.Task.WaitAsync(Timeout);
        await Task.Delay(800);

        Assert.Equal(["renamed:old.txt->new.txt"], _log.ToArray());
    }

    [Fact]
    public async Task Deleted_File_RaisesDeleted()
    {
        var path = Path.Combine(_dir, "gone.txt");
        File.WriteAllText(path, "x");
        _monitor.Start();

        File.Delete(path);

        await _deleted.Task.WaitAsync(Timeout);
        await Task.Delay(800);

        Assert.Equal(["deleted:gone.txt"], _log.ToArray());
    }

    [Fact]
    public async Task Changed_DeleteThenMoveTempOverOriginal_RaisesSingleChanged()
    {
        var path = Path.Combine(_dir, "doc.txt");
        var temp = Path.Combine(_dir, "doc.txt.tmp");
        File.WriteAllText(path, "v0");
        _monitor.Start();

        File.WriteAllText(temp, "v1");
        File.Delete(path);
        File.Move(temp, path);

        await _changed.Task.WaitAsync(Timeout);
        await Task.Delay(1000);

        Assert.Equal(["changed:doc.txt"], _log.ToArray());
    }

    [Fact]
    public async Task Changed_FileReplace_WithMatchAllGlob_RaisesSingleChanged()
    {
        var path = Path.Combine(_dir, "doc.txt");
        var temp = Path.Combine(_dir, "doc.txt.tmp");
        File.WriteAllText(path, "v0");
        _monitor.Start();

        File.WriteAllText(temp, "v1");
        File.Replace(temp, path, destinationBackupFileName: null);

        await _changed.Task.WaitAsync(Timeout);
        await Task.Delay(1000);

        Assert.Equal(["changed:doc.txt"], _log.ToArray());
    }

    [Fact]
    public async Task Changed_FileHeldLongerThanThreshold_WarnsOnceThenRaisesChangedWhenReleased()
    {
        _monitor.Dispose();
        _monitor = Build("**", o => o.LockedTooLongAfter = TimeSpan.FromSeconds(1));
        var path = Path.Combine(_dir, "doc.txt");
        File.WriteAllText(path, "v0");
        _monitor.Start();

        await using (var editor = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.Read))
        {
            await editor.WriteAsync(new byte[] { 1, 2, 3 });
            await editor.FlushAsync();

            // Windows publishes metadata of an open file lazily; touch it so the "save" is noticed right away.
            File.SetLastWriteTimeUtc(path, DateTime.UtcNow.AddMinutes(1));
            await Task.Delay(2500);

            _log.Enqueue("editor-closed");
        }

        await _changed.Task.WaitAsync(Timeout);
        await Task.Delay(800);

        Assert.Equal(["error:FileLockedTooLongException", "editor-closed", "changed:doc.txt"], _log.ToArray());
    }
}
