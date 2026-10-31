using Nameless.Contracts;
using Nameless.Windows.UI;

namespace Nameless.Windows.TaskRunner;

/// <summary>
///     Task Runner implementation
/// </summary>
public class TaskRunnerImpl : ITaskRunner {
    private readonly IWindowFactory _windowFactory;

    /// <summary>
    ///     Initializes a new instance of <see cref="TaskRunnerImpl"/> class.
    /// </summary>
    /// <param name="windowFactory">
    ///     The window factory.
    /// </param>
    public TaskRunnerImpl(IWindowFactory windowFactory) {
        _windowFactory = Throws.When.Null(windowFactory);
    }

    /// <inheritdoc />
    public TaskRunnerBuilder CreateBuilder() {
        return new TaskRunnerBuilder(_windowFactory);
    }
}