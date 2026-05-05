using Nameless.Registration;

namespace Nameless.Workers;

/// <summary>
///     The registration configurator for Workers
/// </summary>
public class WorkersRegistration : AssemblyScanAware<WorkersRegistration> {
    private readonly HashSet<Type> _workers = [];

    /// <summary>
    ///     Gets the registered workers.
    /// </summary>
    public IReadOnlyCollection<Type> Workers => _workers;

    /// <summary>
    ///     Registers a worker.
    /// </summary>
    /// <typeparam name="TWorker">
    ///     Type of the worker.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="WorkersRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public WorkersRegistration RegisterWorker<TWorker>()
        where TWorker : Worker {
        return RegisterWorker(typeof(TWorker));
    }

    /// <summary>
    ///     Registers a worker.
    /// </summary>
    /// <param name="type">
    ///     Type of the worker.
    /// </param>
    /// <returns>
    ///     The current <see cref="WorkersRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public WorkersRegistration RegisterWorker(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFrom(type, typeof(Worker));

        _workers.Add(type);

        return this;
    }
}