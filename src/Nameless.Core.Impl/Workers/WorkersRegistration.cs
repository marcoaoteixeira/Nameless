using Nameless.Registration;

namespace Nameless.Workers;

public class WorkersRegistration : AssemblyScanAware<WorkersRegistration> {
    private readonly HashSet<Type> _workers = [];

    public IReadOnlyCollection<Type> Workers => _workers;

    public WorkersRegistration RegisterWorker<TWorker>()
        where TWorker : Worker {
        return RegisterWorker(typeof(TWorker));
    }

    public WorkersRegistration RegisterWorker(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFrom(type, typeof(Worker));

        _workers.Add(type);

        return this;
    }
}