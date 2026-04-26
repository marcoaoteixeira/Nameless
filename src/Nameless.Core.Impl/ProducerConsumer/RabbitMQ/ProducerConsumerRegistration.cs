using Nameless.Registration;

namespace Nameless.ProducerConsumer.RabbitMQ;

public class ProducerConsumerRegistration : AssemblyScanAware<ProducerConsumerRegistration> {
    private readonly HashSet<Type> _consumers = [];

    public IReadOnlyCollection<Type> Consumers => _consumers;

    public ProducerConsumerRegistration RegisterConsumer<TConsumer, TMessage>()
        where TConsumer : Consumer<TMessage> {
        return RegisterConsumer(typeof(TConsumer));
    }

    public ProducerConsumerRegistration RegisterConsumer(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFromGeneric(type, lhs: typeof(Consumer<>));

        _consumers.Add(type);

        return this;
    }
}
