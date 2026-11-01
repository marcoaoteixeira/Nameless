using Nameless.Contracts;
using Nameless.Registration;

namespace Nameless.Windows.Mvvm;

/// <summary>
///     View Model Feature Options
/// </summary>
public class MvvmFeatureOptions : AssemblyScanAware<MvvmFeatureOptions> {
    private static readonly Type ViewModelType = typeof(ViewModel);

    private readonly HashSet<Type> _viewModels = [];

    /// <summary>
    ///     Gets the registered view models.
    /// </summary>
    public IReadOnlyCollection<Type> ViewModels => UseAssemblyScan
        ? ExecuteAssemblyScan(ViewModelType)
        : _viewModels;

    /// <summary>
    ///     Registers a view model.
    /// </summary>
    /// <typeparam name="TViewModel">
    ///     Type of the view model.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="MvvmFeatureOptions"/> instance so
    ///     other actions can be chained.
    /// </returns>
    public MvvmFeatureOptions WithViewModel<TViewModel>() where TViewModel : ViewModel {
        return WithViewModel(typeof(TViewModel));
    }

    /// <summary>
    ///     Registers a view model.
    /// </summary>
    /// <param name="type">
    ///     Type of the view model.
    /// </param>
    /// <returns>
    ///     The current <see cref="MvvmFeatureOptions"/> instance so
    ///     other actions can be chained.
    /// </returns>
    public MvvmFeatureOptions WithViewModel(Type type) {
        Throws.When.NonConcreteType(type);
        Throws.When.OpenGenericType(type);
        Throws.When.NonAssignableFrom(type, ViewModelType);

        _viewModels.Add(type);

        return this;
    }
}