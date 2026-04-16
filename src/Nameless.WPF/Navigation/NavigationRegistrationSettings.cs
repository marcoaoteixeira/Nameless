using Nameless.Registration;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;

namespace Nameless.WPF.Navigation;

public class NavigationRegistrationSettings : AssemblyScanAware<NavigationRegistrationSettings> {
    private readonly HashSet<Type> _navigationViews = [];

    public Type? NavigationWindow { get; set; }
    
    public IReadOnlyCollection<Type> NavigationViews => _navigationViews;

    public NavigationRegistrationSettings RegisterNavigationWindow<TNavigationWindow>()
        where TNavigationWindow : INavigationWindow {
        return RegisterNavigationWindow(typeof(TNavigationWindow));
    }

    public NavigationRegistrationSettings RegisterNavigationWindow(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFrom(type, typeof(INavigationWindow));

        NavigationWindow = type;

        return this;
    }

    public NavigationRegistrationSettings RegisterNavigationView<TNavigationView, TView>()
        where TNavigationView : INavigableView<TView> {
        return RegisterNavigationView(typeof(TNavigationView));
    }

    public NavigationRegistrationSettings RegisterNavigationView(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFromGeneric(type, typeof(INavigableView<>));

        _navigationViews.Add(type);

        return this;
    }
}