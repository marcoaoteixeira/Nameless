using Nameless.WPF.TaskRunner.Impl;

namespace Nameless.WPF.TaskRunner;

public interface ITaskRunner {
    TaskRunnerBuilder CreateBuilder();
}