using Nameless.Windows.Messaging;

namespace Nameless.Windows.TaskRunner;

public interface ITaskRunnerBuilder {
    ITaskRunnerBuilder SetName(string name);

    ITaskRunnerBuilder SetDelegate(TaskRunnerDelegate @delegate);

    ITaskRunnerBuilder SubscribeFor<TMessage>() where TMessage : Message;

    Task RunAsync();
}