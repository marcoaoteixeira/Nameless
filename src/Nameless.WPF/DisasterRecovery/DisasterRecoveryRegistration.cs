using Nameless.Registration;

namespace Nameless.WPF.DisasterRecovery;

public class DisasterRecoveryRegistration : AssemblyScanAware<DisasterRecoveryRegistration> {
    private readonly HashSet<Type> _disasterRecoveryRoutines = [];

    public IReadOnlyCollection<Type> DisasterRecoveryRoutines => _disasterRecoveryRoutines;

    public DisasterRecoveryRegistration RegisterDisasterRecoveryRoutine<TDisasterRecoveryRoutine>()
        where TDisasterRecoveryRoutine : IDisasterRecoveryRoutine {
        return RegisterDisasterRecoveryRoutine(typeof(TDisasterRecoveryRoutine));
    }

    public DisasterRecoveryRegistration RegisterDisasterRecoveryRoutine(Type type) {
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFrom(type, typeof(IDisasterRecoveryRoutine));

        _disasterRecoveryRoutines.Add(type);

        return this;
    }
}