using System.ComponentModel;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Nameless.WPF.DependencyInjection;
using Nameless.WPF.Messaging;

namespace Nameless.WPF.TaskRunner.UI;

/// <summary>
///     Action handler window.
/// </summary>
[ServiceLifetime(Lifetime = ServiceLifetime.Transient)]
public partial class TaskRunnerWindow : ITaskRunnerWindow {
    private bool _canClose;

    /// <inheritdoc />
    public TaskRunnerWindowViewModel ViewModel { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="TaskRunnerWindow"/> class.
    /// </summary>
    /// <param name="viewModel">
    ///     The view model.
    /// </param>
    public TaskRunnerWindow(TaskRunnerWindowViewModel viewModel) {
        ViewModel = viewModel;

        DataContext = ViewModel;

        InitializeComponent();
    }

    /// <inheritdoc />
    public ITaskRunnerWindow SetName(string name) {
        ViewModel.Title = name;

        return this;
    }

    /// <inheritdoc />
    public ITaskRunnerWindow SetOwner(Window? owner) {
        Dispatcher.Invoke(() => Owner = owner ?? WindowsApplication.Current.MainWindow);

        return this;
    }

    /// <inheritdoc />
    public ITaskRunnerWindow SubscribeFor<TPushNotificationMessage>()
        where TPushNotificationMessage : Message {
        ViewModel.SubscribeFor<TPushNotificationMessage>();

        return this;
    }

    /// <inheritdoc />
    public ITaskRunnerWindow SetDelegate(TaskRunnerDelegate @delegate) {
        ViewModel.SetHandler(@delegate);

        return this;
    }

    /// <inheritdoc />
    public void Show(WindowStartupLocation startupLocation) {
        WindowStartupLocation = startupLocation;

        Dispatcher.Invoke(ShowDialog);
    }

    private void StartupHandler(object sender, RoutedEventArgs args) {
        Dispatcher.InvokeAsync(() => ViewModel.ExecuteCommand.ExecuteAsync(null));
    }

    private void CloseButtonHandler(object sender, RoutedEventArgs args) {
        _canClose = true;

        Close();
    }

    private void ClosingHandler(object sender, CancelEventArgs args) {
        if (_canClose) {
            ViewModel.CleanUp();

            return;
        }

        args.Cancel = true;
    }
}