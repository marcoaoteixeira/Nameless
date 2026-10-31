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

    private CancellationTokenSource? _cts;
    private Action? _subscribe;
    private Action? _unsubscribe;
    private TaskRunnerDelegate? _delegate;

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
    }

    [RelayCommand]
    private async Task ExecuteAsync() {
        if (_delegate is null) { return; }

        await ToggleRunningAsync();

        _subscribe?.Invoke();

        try { await _delegate(GetCancellationTokenSource().Token); }
        catch (Exception ex) { Entries.Add(LoggerRichTextBoxEntry.Error(ex.Message)); }

        _unsubscribe?.Invoke();

        await ToggleRunningAsync();
    }

    [RelayCommand]
    private Task CancelAsync() {
        return GetCancellationTokenSource().CancelAsync();
    }

    private CancellationTokenSource GetCancellationTokenSource() {
        return _cts ??= new CancellationTokenSource();
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
