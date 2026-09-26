using Nameless.Registration;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;

namespace Nameless.Windows.Navigation;

public class NavigationRegistration : AssemblyScanAware<NavigationRegistration> {
    private readonly HashSet<Type> _navigationViews = [];

    public Type? NavigationWindow { get; set; }
    
    public IReadOnlyCollection<Type> NavigationViews => _navigationViews;

    public NavigationRegistration RegisterNavigationWindow<TNavigationWindow>()
        where TNavigationWindow : INavigationWindow {
        return RegisterNavigationWindow(typeof(TNavigationWindow));
    }

    public NavigationRegistration RegisterNavigationWindow(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFrom(type, typeof(INavigationWindow));

        NavigationWindow = type;

        return this;
    }

    public NavigationRegistration RegisterNavigationView<TNavigationView, TView>()
        where TNavigationView : INavigableView<TView> {
        return RegisterNavigationView(typeof(TNavigationView));
    }

    public NavigationRegistration RegisterNavigationView(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFromGeneric(type, typeof(INavigableView<>));

        _navigationViews.Add(type);

        return this;
    }
}