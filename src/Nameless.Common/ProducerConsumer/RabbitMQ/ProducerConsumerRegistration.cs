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
    public IReadOnlyCollection<Type> Consumers => UseAssemblyScan
        ? ExecuteAssemblyScan(typeof(Consumer<>))
        : _consumers;

    /// <summary>
    ///     Registers a consumer by generic type parameters.
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type.</typeparam>
    /// <typeparam name="T">The message type the consumer handles.</typeparam>
    /// <returns>
    ///     The current <see cref="ProducerConsumerRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public ProducerConsumerRegistration WithConsumer<TConsumer, T>()
        where TConsumer : Consumer<T> {
        return WithConsumer(typeof(TConsumer));
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
    public ProducerConsumerRegistration WithConsumer(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFromGeneric(type, lhs: typeof(Consumer<>));

        _consumers.Add(type);

        return this;
    }
}
