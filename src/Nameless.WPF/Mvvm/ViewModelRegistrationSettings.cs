using Nameless.Registration;

namespace Nameless.WPF.Mvvm;

public class ViewModelRegistrationSettings : AssemblyScanAware<ViewModelRegistrationSettings> {
    private readonly HashSet<Type> _viewModels = [];

    public IReadOnlyCollection<Type> ViewModels => _viewModels;

    public ViewModelRegistrationSettings RegisterViewModel<TViewModel>()
        where TViewModel : ViewModel {
        return RegisterViewModel(typeof(TViewModel));
    }

    public ViewModelRegistrationSettings RegisterViewModel(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFrom(type, typeof(ViewModel));

        _viewModels.Add(type);

        return this;
    }
}