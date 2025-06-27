using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Internals;

/// <summary>
///     <see cref="Parameters"/> extension methods.
/// </summary>
internal static class ParametersExtensions {
    /// <summary>
    ///     Creates a <see cref="BasicProperties"/> from the <see cref="Parameters"/> argument.
    /// </summary>
    /// <param name="self">The <see cref="Parameters"/>.</param>
    /// <returns>
    ///     A new instance of <see cref="BasicProperties"/>.
    /// </returns>
    internal static BasicProperties CreateBasicProperties(this Parameters self) {
        return new BasicProperties().FillWith(self);
    }
}
