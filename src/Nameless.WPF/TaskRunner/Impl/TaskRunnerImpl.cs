using Nameless.WPF.Windows;

namespace Nameless.WPF.TaskRunner.Impl;

public class TaskRunnerImpl : ITaskRunner {
    private readonly IWindowFactory _windowFactory;

    public TaskRunnerImpl(IWindowFactory windowFactory) {
        _windowFactory = windowFactory;
    }

    public TaskRunnerBuilder CreateBuilder() {
        return new TaskRunnerBuilder(_windowFactory);
    }
}