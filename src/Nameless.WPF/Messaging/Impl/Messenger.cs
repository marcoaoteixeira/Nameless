using CommunityToolkit.Mvvm.Messaging;

namespace Nameless.WPF.Messaging.Impl;

public class Messenger : IMessenger {
    private readonly IMvvmMessenger _messenger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="Messenger"/> class.
    /// </summary>
    /// <param name="messenger">
    ///     The messenger.
    /// </param>
    public Messenger(IMvvmMessenger messenger) {
        _messenger = messenger;
    }

    public void Register<TMessage>(object recipient, Action<object, TMessage> handler) where TMessage : Message {
        if (_messenger.IsRegistered<TMessage>(recipient)) { return; }

        _messenger.Register<TMessage>(
            recipient,
            handler: (destinatary, evt) => handler(destinatary, evt)
        );
    }

    public void Unregister<TMessage>(object recipient) where TMessage : Message {
        _messenger.Unregister<TMessage>(recipient);
    }

    public Task PublishAsync<TMessage>(TMessage evt) where TMessage : Message {
        _messenger.Send(evt);

        return Task.CompletedTask;
    }
}