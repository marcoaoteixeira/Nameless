using Nameless.Windows.TaskRunner.Impl;

namespace Nameless.Windows.TaskRunner;

public interface ITaskRunner {
    TaskRunnerBuilder CreateBuilder();
}