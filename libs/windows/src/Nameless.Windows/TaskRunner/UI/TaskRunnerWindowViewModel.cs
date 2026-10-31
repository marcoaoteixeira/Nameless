using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nameless.Contracts;
using Nameless.Windows.Controls;
using Nameless.Windows.Events;
using Nameless.Windows.Localization;
using Nameless.Windows.Mvvm;
using Nameless.Windows.Notification;
using System.Collections.ObjectModel;

namespace Nameless.Windows.TaskRunner.UI;

/// <summary>
///     Task Runner Window View Model (partial)
/// </summary>
public partial class TaskRunnerWindowViewModel : ViewModel {
    private readonly IEventPublisher _eventPublisher;
    private readonly TaskCompletionSource<TaskRunnerOutcome> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private CancellationTokenSource? _cts;
    private Action? _subscribe;
    private Action? _unsubscribe;
    private TaskRunnerDelegate? _delegate;
    private bool _closeBlockedNotified;

    /// <summary>
    ///     Gets or sets the window title.
    /// </summary>
    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the running state.
    /// </summary>
    [ObservableProperty]
    public partial bool Running { get; set; }

    /// <summary>
    ///     Gets or sets the idle state.
    /// </summary>
    [ObservableProperty]
    public partial bool Idle { get; set; } = true;

    /// <summary>
    ///     Gets or sets the logger entries.
    /// </summary>
    [ObservableProperty]
    public partial ObservableCollection<LoggerRichTextBoxEntry> Entries { get; set; } = [];

    /// <summary>
    ///     Gets the translator.
    /// </summary>
    public ITranslator T { get; }

    /// <summary>
    ///     Gets the task that completes once per view model, when the run
    ///     finishes, is cancelled, or the window is torn down. Completes
    ///     with <see cref="TaskRunnerOutcome.Completed"/> or
    ///     <see cref="TaskRunnerOutcome.Cancelled"/>, or faults with the
    ///     delegate's own exception (other than a cancellation tied to the
    ///     run's own token).
    /// </summary>
    public Task<TaskRunnerOutcome> Completion => _completion.Task;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="TaskRunnerWindowViewModel"/> class.
    /// </summary>
    /// <param name="eventPublisher">
    ///     The notification service.
    /// </param>
    /// <param name="translator">
    ///     The translator.
    /// </param>
    public TaskRunnerWindowViewModel(IEventPublisher eventPublisher, ITranslator translator) {
        _eventPublisher = Throws.When.Null(eventPublisher);

        T = Throws.When.Null(translator);
    }

    /// <summary>
    ///     Subscribes for a specific notification type.
    /// </summary>
    /// <typeparam name="TNotificationEvent">
    ///     The event type.
    /// </typeparam>
    public void SubscribeFor<TNotificationEvent>() where TNotificationEvent : NotificationEvent {
        _subscribe += () => _eventPublisher.Register<TNotificationEvent>(this, Update);
        _unsubscribe += () => _eventPublisher.Unregister<TNotificationEvent>(this);
    }

    /// <summary>
    ///     Sets the async delegate to be executed.
    /// </summary>
    /// <param name="delegate">
    ///     The async delegate.
    /// </param>
    public void SetHandler(TaskRunnerDelegate @delegate) {
        _delegate = @delegate;
    }

    /// <summary>
    ///     Cleans up all resources used by the task runner.
    /// </summary>
    public void CleanUp() {
        if (Running) {
            _cts?.Cancel(throwOnFirstException: false);
        }

        _cts?.Dispose();
        _cts = null;
        _subscribe = null;
        _unsubscribe = null;
        _delegate = null;

        // Safety net: ensures RunAsync can never hang if the window is
        // closed before or while the delegate runs. No-op once the run
        // already resolved the completion source.
        _completion.TrySetResult(TaskRunnerOutcome.Cancelled);
    }

    /// <summary>
    ///     Notifies that a close request (title bar, Alt+F4) was blocked
    ///     because the run has not completed yet. Adds a Warning log entry
    ///     at most once per run.
    /// </summary>
    internal void NotifyCloseBlocked() {
        if (_closeBlockedNotified) { return; }

        _closeBlockedNotified = true;

        Entries.Add(LoggerRichTextBoxEntry.Warning(T["TaskRunnerWindow_CloseBlocked_Message"]));
    }

    [RelayCommand]
    private async Task ExecuteAsync() {
        if (Completion.IsCompleted) { return; }

        var @delegate = _delegate;
        if (@delegate is null) {
            _completion.TrySetResult(TaskRunnerOutcome.Completed);

            return;
        }

        await ToggleRunningAsync();

        // The window may have been torn down during the 200ms delay above.
        // Re-check before touching any state the close path may have
        // already nulled out.
        if (Completion.IsCompleted) { return; }

        _closeBlockedNotified = false;

        var cts = _cts = new CancellationTokenSource();
        var unsubscribe = _unsubscribe;

        _subscribe?.Invoke();

        var outcome = TaskRunnerOutcome.Completed;
        Exception? failure = null;

        try { await @delegate(cts.Token); }
        catch (OperationCanceledException) when (cts.IsCancellationRequested) {
            outcome = TaskRunnerOutcome.Cancelled;

            Entries.Add(LoggerRichTextBoxEntry.Warning(T["TaskRunnerWindow_Cancelled_Message"]));
        }
        catch (Exception ex) {
            failure = ex;

            Entries.Add(LoggerRichTextBoxEntry.Error(ex.Message));
        }
        finally {
            unsubscribe?.Invoke();
        }

        await ToggleRunningAsync();

        if (failure is not null) { _completion.TrySetException(failure); }
        else { _completion.TrySetResult(outcome); }
    }

    [RelayCommand]
    private Task CancelAsync() {
        return _cts is { IsCancellationRequested: false } cts
            ? cts.CancelAsync()
            : Task.CompletedTask;
    }

    private void Update<TNotificationEvent>(object sender, TNotificationEvent evt)
        where TNotificationEvent : NotificationEvent {
        var entry = evt.Type switch {
            NotificationType.Success => LoggerRichTextBoxEntry.Success(evt.Message),
            NotificationType.Warning => LoggerRichTextBoxEntry.Warning(evt.Message),
            NotificationType.Failure => LoggerRichTextBoxEntry.Error(evt.Message),
            _ => LoggerRichTextBoxEntry.Information(evt.Message)
        };

        Entries.Add(entry);
    }

    private async Task ToggleRunningAsync() {
        // Small delay so any UI control can be
        // rendered properly.
        await Task.Delay(200);

        Running = !Running;
        Idle = !Idle;
    }
}
