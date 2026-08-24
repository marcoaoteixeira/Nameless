using Nameless.Registration;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     Registration options for RabbitMQ producer/consumer services.
/// </summary>
public class ProducerConsumerRegistration : AssemblyScanAware<ProducerConsumerRegistration> {
    private readonly HashSet<Type> _consumers = [];

    /// <summary>
    ///     Gets the registered consumer types.
    /// </summary>
    public IReadOnlyCollection<Type> Consumers => _consumers;

    /// <summary>
    ///     Registers a consumer by generic type parameters.
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type.</typeparam>
    /// <typeparam name="TMessage">The message type the consumer handles.</typeparam>
    /// <returns>
    ///     The current <see cref="ProducerConsumerRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public ProducerConsumerRegistration RegisterConsumer<TConsumer, TMessage>()
        where TConsumer : Consumer<TMessage> {
        return RegisterConsumer(typeof(TConsumer));
    }

    /// <summary>
    ///     Registers a consumer by type.
    /// </summary>
    /// <param name="type">The consumer type.</param>
    /// <returns>
    ///     The current <see cref="ProducerConsumerRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="type"/> is non-concrete, an open generic, or does not
    ///     derive from <see cref="Consumer{TMessage}"/>.
    /// </exception>
    public ProducerConsumerRegistration RegisterConsumer(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFromGeneric(type, lhs: typeof(Consumer<>));

        _consumers.Add(type);

        return this;
    }
}
