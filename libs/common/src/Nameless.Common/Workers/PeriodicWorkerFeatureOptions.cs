using Nameless.Contracts;
using Nameless.Registration;

namespace Nameless.Workers;

/// <summary>
///     The registration configurator for Workers
/// </summary>
public class PeriodicWorkerFeatureOptions : AssemblyScanAware<PeriodicWorkerFeatureOptions> {
    private readonly HashSet<Type> _workers = [];

    /// <summary>
    ///     Gets the registered workers.
    /// </summary>
    public IReadOnlyCollection<Type> Workers => UseAssemblyScan
        ? ExecuteAssemblyScan(typeof(PeriodicWorker))
        : _workers;

    /// <summary>
    ///     Registers a periodic worker.
    /// </summary>
    /// <typeparam name="TPeriodicWorker">
    ///     Type of the periodic worker.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="PeriodicWorkerFeatureOptions"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public PeriodicWorkerFeatureOptions WithPeriodicWorker<TPeriodicWorker>()
        where TPeriodicWorker : PeriodicWorker {
        return WithPeriodicWorker(typeof(TPeriodicWorker));
    }

    /// <summary>
    ///     Registers a worker.
    /// </summary>
    /// <param name="type">
    ///     Type of the worker.
    /// </param>
    /// <returns>
    ///     The current <see cref="PeriodicWorkerFeatureOptions"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public PeriodicWorkerFeatureOptions WithPeriodicWorker(Type type) {
        Throws.When.NonConcreteType(type);
        Throws.When.OpenGenericType(type);
        Throws.When.NonAssignableFrom(type, typeof(PeriodicWorker));

        _workers.Add(type);

        return this;
    }
}