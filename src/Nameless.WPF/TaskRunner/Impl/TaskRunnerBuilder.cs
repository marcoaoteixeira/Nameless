using System.Windows;
using Nameless.WPF.Messaging;
using Nameless.WPF.TaskRunner.UI;
using Nameless.WPF.Windows;

namespace Nameless.WPF.TaskRunner.Impl;

public class TaskRunnerBuilder : ITaskRunnerBuilder {
    private readonly IWindowFactory _windowFactory;
    private readonly Lazy<ITaskRunnerWindow> _taskRunnerWindow;

    private ITaskRunnerWindow Window => _taskRunnerWindow.Value;

    public TaskRunnerBuilder(IWindowFactory windowFactory) {
        _windowFactory = windowFactory;
        _taskRunnerWindow = new Lazy<ITaskRunnerWindow>(CreateTaskRunnerWindow);
    }

    public ITaskRunnerBuilder SetName(string name) {
        Window.SetName(name);

        return this;
    }

    public ITaskRunnerBuilder SetDelegate(TaskRunnerDelegate @delegate) {
        Window.SetDelegate(@delegate);

        return this;
    }

    public ITaskRunnerBuilder SubscribeFor<TMessage>() where TMessage : Message {
        Window.SubscribeFor<TMessage>();

        return this;
    }

    public Task RunAsync() {
        Window.Show(WindowStartupLocation.CenterScreen);

        return Task.CompletedTask;
    }

    private ITaskRunnerWindow CreateTaskRunnerWindow() {
        var window = _windowFactory.Create<ITaskRunnerWindow>();

        window.SetOwner(WindowsApplication.Current.MainWindow);

        return window;
    }
}