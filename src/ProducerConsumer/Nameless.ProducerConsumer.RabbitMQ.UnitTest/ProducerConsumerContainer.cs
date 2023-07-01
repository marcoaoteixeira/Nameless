using Autofac;

namespace Nameless.ProducerConsumer.RabbitMQ.UnitTesting {

    internal static class ProducerConsumerContainer {

        internal static IContainer Create(Action<ContainerBuilder>? config = default) {
            var builder = new ContainerBuilder();

            config?.Invoke(builder);

            builder.RegisterModule<ProducerConsumerModule>();

            return builder.Build();
        }
    }
}
