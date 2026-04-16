using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nameless.WPF.Controls;
using Nameless.WPF.Messaging;
using Nameless.WPF.Mvvm;
using Nameless.WPF.Resources;

namespace Nameless.WPF.TaskRunner.UI;

public partial class TaskRunnerWindowViewModel : ViewModel {
    private readonly IMessenger _messenger;

    private CancellationTokenSource? _cts;
    private Action? _subscribe;
    private Action? _unsubscribe;
    private TaskRunnerDelegate? _delegate;

    [ObservableProperty]
    private string _title = Strings.TaskRunnerWindow_Title;

    [ObservableProperty]
    private bool _running;

    [ObservableProperty]
    private bool _idle = true;

    [ObservableProperty]
    private ObservableCollection<LoggerRichTextBoxEntry> _entries = [];

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="TaskRunnerWindowViewModel"/> class.
    /// </summary>
    /// <param name="messenger">
    ///     The notification service.
    /// </param>
    public TaskRunnerWindowViewModel(IMessenger messenger) {
        _messenger = messenger;
    }

    /// <summary>
    ///     Subscribes for a specific notification type.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The event type.
    /// </typeparam>
    public void SubscribeFor<TMessage>()
        where TMessage : Message {
        _subscribe += () => _messenger.Register<TMessage>(this, Update);
        _unsubscribe += () => _messenger.Unregister<TMessage>(this);
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

    private void Update<TMessage>(object sender, TMessage evt)
        where TMessage : Message {
        var entry = evt.Type switch {
            MessageType.Success => LoggerRichTextBoxEntry.Success(evt.Content),
            MessageType.Warning => LoggerRichTextBoxEntry.Warning(evt.Content),
            MessageType.Failure => LoggerRichTextBoxEntry.Error(evt.Content),
            _ => LoggerRichTextBoxEntry.Information(evt.Content)
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
