using System.Collections.Concurrent;

namespace Nameless.IO.Monitoring;

// Handlers run one at a time, in decision order, on the monitor's own consumer.
public class FileMonitorSerializationTests
{
    private static readonly TimeSpan Wait = TimeSpan.FromSeconds(5);

    private static FileMonitorFixture CreateUnstarted()
    {
        return new FileMonitorFixture(register: false, start: false) { AutoFlush = false };
    }

    [Fact]
    public void SlowHandler_DoesNotBlockTheRaisingThread_AndHandlersNeverOverlap()
    {
        using var fx = CreateUnstarted();
        using var release = new ManualResetEventSlim();
        using var started = new ManualResetEventSlim();
        var running = 0;
        var maxRunning = 0;
        var finished = new ConcurrentQueue<string>();

        void Handle(string name)
        {
            var now = Interlocked.Increment(ref running);
            InterlockedMax(ref maxRunning, now);
            started.Set();
            release.Wait(Wait);
            Interlocked.Decrement(ref running);
            finished.Enqueue(name);
        }

        fx.Monitor.OnChanged(e => Handle(e.Name));
        fx.Monitor.OnCreated(e => Handle(e.Name));
        fx.Monitor.Start();

        fx.RaiseChanged("a.txt");
        fx.RaiseCreated("b.txt");
        fx.Advance(500);

        Assert.Multiple(
            () => Assert.Empty(finished),
            () => Assert.True(started.Wait(Wait))
        );
        Thread.Sleep(100);

        release.Set();
        fx.Flush();

        Assert.Multiple(
            () => Assert.Equal(1, maxRunning),
            () => Assert.Equal(2, finished.Count)
        );
    }

    [Fact]
    public async Task DeliveryOrder_MatchesDecisionOrder_EvenWhileHandlerIsBlocked()
    {
        using var fx = CreateUnstarted();
        using var release = new ManualResetEventSlim();
        using var started = new ManualResetEventSlim();
        var log = new ConcurrentQueue<string>();
        var first = true;

        fx.Monitor.OnRenamed(e =>
        {
            if (first)
            {
                first = false;
                started.Set();
                release.Wait(Wait);
            }

            log.Enqueue($"renamed:{e.Name}");
        });
        fx.Monitor.OnError(e => log.Enqueue($"error:{e.Exception.Message}"));
        fx.Monitor.Start();

        fx.RaiseRenamed("a", "b");
        Assert.True(started.Wait(Wait));

        var intake = Task.Run(() =>
        {
            fx.RaiseRenamed("c", "d");
            fx.RaiseError(new IOException("overflow"));
            fx.RaiseRenamed("e", "f");
        });
        await intake.WaitAsync(Wait); // raw event intake must not wait for handlers

        release.Set();
        fx.Flush();

        Assert.Equal(["renamed:b", "renamed:d", "error:overflow", "renamed:f"], log.ToArray());
    }

    [Fact]
    public void Handlers_RunOnAThreadOtherThanTheRaisingOne()
    {
        using var fx = CreateUnstarted();
        int? handlerThread = null;
        fx.Monitor.OnRenamed(_ => handlerThread = Environment.CurrentManagedThreadId);
        fx.Monitor.Start();

        fx.RaiseRenamed("a", "b");
        fx.Flush();

        Assert.Multiple(
            () => Assert.NotNull(handlerThread),
            () => Assert.NotEqual(Environment.CurrentManagedThreadId, handlerThread)
        );
    }

    [Fact]
    public async Task Dispose_FromInsideHandler_DoesNotDeadlock()
    {
        using var fx = CreateUnstarted();
        var disposed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        fx.Monitor.OnRenamed(_ =>
        {
            fx.Monitor.Dispose();
            disposed.TrySetResult();
        });
        fx.Monitor.Start();

        fx.RaiseRenamed("a", "b");

        await disposed.Task.WaitAsync(Wait);
    }

    [Fact]
    public void Dispose_DiscardsNotificationsStillQueued()
    {
        using var fx = CreateUnstarted();
        using var release = new ManualResetEventSlim();
        using var started = new ManualResetEventSlim();
        var log = new ConcurrentQueue<string>();
        var first = true;

        fx.Monitor.OnRenamed(e =>
        {
            if (first)
            {
                first = false;
                started.Set();
                release.Wait(Wait);
            }

            log.Enqueue(e.Name);
        });
        fx.Monitor.Start();
        fx.RaiseRenamed("a", "b");
        Assert.True(started.Wait(Wait));
        fx.RaiseRenamed("c", "d");

        fx.Monitor.Dispose();
        release.Set();
        Thread.Sleep(200);

        Assert.Equal(["b"], log.ToArray());
    }

    private static void InterlockedMax(ref int target, int value)
    {
        int current;
        while (value > (current = Volatile.Read(ref target)))
        {
            if (Interlocked.CompareExchange(ref target, value, current) == current)
            {
                return;
            }
        }
    }
}
