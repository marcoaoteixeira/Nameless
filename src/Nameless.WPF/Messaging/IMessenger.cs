namespace Nameless.WPF.Messaging;

public interface IMessenger {
    void Register<TMessage>(object recipient, Action<object, TMessage> handler)
        where TMessage : Message;

    void Unregister<TMessage>(object recipient)
        where TMessage : Message;

    Task PublishAsync<TMessage>(TMessage evt)
        where TMessage : Message;
}