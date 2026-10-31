using CommunityToolkit.Mvvm.Messaging;
using Nameless.Contracts;

namespace Nameless.Windows.Events;

/// <summary>
///     In-Process Event Publisher
/// </summary>
public class InProcessEventPublisher : IEventPublisher {
    private readonly IMvvmMessenger _messenger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="InProcessEventPublisher"/> class.
    /// </summary>
    /// <param name="messenger">
    ///     The messenger.
    /// </param>
    public InProcessEventPublisher(IMvvmMessenger messenger) {
        _messenger = Throws.When.Null(messenger);
    }

    /// <inheritdoc />
    public void Register<TEvent>(object recipient, Action<object, TEvent> handler) where TEvent : Event {
        if (_messenger.IsRegistered<TEvent>(recipient)) { return; }

        _messenger.Register<TEvent>(
            recipient,
            handler: (destinatary, evt) => handler(destinatary, evt)
        );
    }

    /// <inheritdoc />
    public void Unregister<TEvent>(object recipient) where TEvent : Event {
        _messenger.Unregister<TEvent>(recipient);
    }

    /// <inheritdoc />
    public Task PublishAsync<TEvent>(TEvent evt) where TEvent : Event {
        _messenger.Send(evt);

        return Task.CompletedTask;
    }
}