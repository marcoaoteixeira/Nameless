using Nameless.Registration;

namespace Nameless.Bootstrap;

public class BootstrapRegistration : AssemblyScanAware<BootstrapRegistration> {
    private readonly HashSet<Type> _steps = [];

    public IReadOnlyCollection<Type> Steps => _steps;

    public BootstrapRegistration RegisterStep<TStep>()
        where TStep : IStep {
        return RegisterStep(typeof(TStep));
    }

    public BootstrapRegistration RegisterStep(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IStep));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        _steps.Add(type);

        return this;
    }
}
