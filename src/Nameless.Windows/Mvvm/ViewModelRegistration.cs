using Nameless.Registration;

namespace Nameless.Windows.Mvvm;

public class ViewModelRegistration : AssemblyScanAware<ViewModelRegistration> {
    private readonly HashSet<Type> _viewModels = [];

    public IReadOnlyCollection<Type> ViewModels => _viewModels;

    public ViewModelRegistration RegisterViewModel<TViewModel>()
        where TViewModel : ViewModel {
        return RegisterViewModel(typeof(TViewModel));
    }

    public ViewModelRegistration RegisterViewModel(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFrom(type, typeof(ViewModel));

        _viewModels.Add(type);

        return this;
    }
}