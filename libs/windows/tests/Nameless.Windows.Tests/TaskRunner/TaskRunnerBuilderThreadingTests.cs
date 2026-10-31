using System.Threading;
using System.Windows;
using Moq;
using Nameless.Testing.Tools.Attributes;
using Nameless.Windows.Events;
using Nameless.Windows.Localization;
using Nameless.Windows.TaskRunner;
using Nameless.Windows.TaskRunner.UI;
using Nameless.Windows.UI;
using WindowsApplication = System.Windows.Application;

namespace Nameless.Windows.Tests.TaskRunner;

/// <summary>
///     Proves the threading contract documented on
///     <see cref="ITaskRunnerBuilder"/>: builder calls (including
///     <see cref="ITaskRunnerBuilder.RunAsync"/> itself) may run on any
///     thread, while window creation and the dialog itself are marshalled
///     to the application's own STA dispatcher thread.
/// </summary>
[IntegrationTest]
public class TaskRunnerBuilderThreadingTests {
    [Fact]
    public async Task RunAsync_CalledFromBackgroundThread_MarshalsWindowCreationToUIThread_AndResolvesOutcome() {
        // arrange: a dedicated STA thread running its own WPF Application
        // and dispatcher, standing in for a real desktop app's UI thread.
        Exception? uiThreadException = null;
        WindowsApplication? app = null;
        var appReady = new ManualResetEventSlim();

        var uiThread = new Thread(() => {
            try {
                app = new WindowsApplication { ShutdownMode = ShutdownMode.OnExplicitShutdown };

                // Application.MainWindow defaults to the first window the
                // builder creates when nothing else sets it; that collides
                // with TaskRunnerBuilder owning the task-runner window to
                // MainWindow ("cannot set Owner to itself"). A real desktop
                // app always has its own main window by the time it opens a
                // task runner, so give this fake Application one too.
                var mainWindow = new Window();
                mainWindow.Show();
                app.MainWindow = mainWindow;

                app.Dispatcher.BeginInvoke(() => appReady.Set());
                app.Run();
            }
            catch (Exception ex) {
                uiThreadException = ex;

                appReady.Set();
            }
        }) {
            IsBackground = true
        };
        uiThread.SetApartmentState(ApartmentState.STA);
        uiThread.Start();

        Assert.True(
            appReady.Wait(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken),
            "UI thread did not start in time.");
        Assert.Null(uiThreadException);

        try {
            var eventPublisher = new Mock<IEventPublisher>(MockBehavior.Loose);
            var windowFactory = new FakeWindowFactory(eventPublisher.Object);
            var builder = new TaskRunnerBuilder(windowFactory);

            // act: every call below, including RunAsync itself, runs on a
            // thread-pool thread, never on the UI thread created above. If
            // window creation were not marshalled to the UI thread, WPF
            // itself would throw (a Window cannot be built on a non-STA
            // thread), so a clean pass here is already evidence of correct
            // marshalling; the thread-id assertion below makes it explicit.
            var outcome = await Task.Run(async () => {
                builder.SetName("Background thread run");

                var window = windowFactory.CreatedWindow;
                Assert.NotNull(window);

                // Completion resolving does not close the window by itself
                // (a real user clicks Close); simulate that so RunAsync's
                // ShowDialog loop can unwind once the run settles.
                _ = window.ViewModel.Completion.ContinueWith(_ =>
                    window.Dispatcher.BeginInvoke(new Action(window.Close)));

                builder.SetDelegate(_ => Task.CompletedTask);

                return await builder.RunAsync();
            });

            // assert
            Assert.Equal(TaskRunnerOutcome.Completed, outcome);
            Assert.Equal(uiThread.ManagedThreadId, windowFactory.CreationThreadId);
        }
        finally {
            app?.Dispatcher.InvokeShutdown();
            uiThread.Join(TimeSpan.FromSeconds(5));
        }
    }

    private sealed class FakeWindowFactory(IEventPublisher eventPublisher) : IWindowFactory {
        public TaskRunnerWindow? CreatedWindow { get; private set; }

        public int? CreationThreadId { get; private set; }

        public TWindow Create<TWindow>() where TWindow : IWindow {
            CreationThreadId = Environment.CurrentManagedThreadId;

            var window = new TaskRunnerWindow(
                new TaskRunnerWindowViewModel(eventPublisher, NullTranslator.Instance));

            CreatedWindow = window;

            return (TWindow)(IWindow)window;
        }
    }
}
