using Nameless.Registration;

namespace Nameless.Workers;

/// <summary>
///     The registration configurator for Workers
/// </summary>
public class PeriodicWorkersRegistration : AssemblyScanAware<PeriodicWorkersRegistration> {
    private readonly HashSet<Type> _workers = [];

    /// <summary>
    ///     Gets the registered workers.
    /// </summary>
    public IReadOnlyCollection<Type> Workers => _workers;

    /// <summary>
    ///     Registers a periodic worker.
    /// </summary>
    /// <typeparam name="TPeriodicWorker">
    ///     Type of the periodic worker.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="PeriodicWorkersRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public PeriodicWorkersRegistration RegisterPeriodicWorker<TPeriodicWorker>()
        where TPeriodicWorker : PeriodicWorker {
        return RegisterPeriodicWorker(typeof(TPeriodicWorker));
    }

    /// <summary>
    ///     Registers a worker.
    /// </summary>
    /// <param name="type">
    ///     Type of the worker.
    /// </param>
    /// <returns>
    ///     The current <see cref="PeriodicWorkersRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public PeriodicWorkersRegistration RegisterPeriodicWorker(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFrom(type, typeof(PeriodicWorker));

        _workers.Add(type);

        return this;
    }
}