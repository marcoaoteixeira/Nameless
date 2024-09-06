namespace Nameless.ProducerConsumer;

/// <summary>
/// Defines what a producer should do.
/// </summary>
public interface IProducerService {
    #region Methods

    /// <summary>
    /// Produces a payload for a specific topic.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="cancellationToken"></param>
    Task ProduceAsync(string topic, object payload, ProducerArgs args, CancellationToken cancellationToken);

    #endregion Methods
}