using Nameless.Registration;

namespace Nameless.Bootstrap.Infrastructure;

public class BootstrapRegistrationSettings : AssemblyScanAware<BootstrapRegistrationSettings> {
    private readonly HashSet<Type> _steps = [];

    public IReadOnlyCollection<Type> Steps => UseAssemblyScan
        ? DiscoverImplementationsFor<IStep>()
        : _steps;

    public BootstrapRegistrationSettings RegisterStep<TStep>()
        where TStep : IStep {
        return RegisterStep(typeof(TStep));
    }

    public BootstrapRegistrationSettings RegisterStep(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IStep));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        _steps.Add(type);

        return this;
    }
}
