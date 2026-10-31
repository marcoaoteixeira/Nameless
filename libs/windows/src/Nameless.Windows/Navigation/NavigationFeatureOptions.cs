using Nameless.Contracts;
using Nameless.Registration;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;

namespace Nameless.Windows.Navigation;

/// <summary>
///     Navigation Feature Options
/// </summary>
public class NavigationFeatureOptions : AssemblyScanAware<NavigationFeatureOptions> {
    private static readonly Type NavigableViewType = typeof(INavigableView<>);

    private readonly HashSet<Type> _navigationViews = [];

    /// <summary>
    ///     Gets the navigation window implementation.
    /// </summary>
    public Type? NavigationWindow { get; private set; }

    /// <summary>
    ///     Gets the navigation view implementations.
    /// </summary>
    public IReadOnlyCollection<Type> NavigationViews => UseAssemblyScan
        ? ExecuteAssemblyScan(NavigableViewType)
        : _navigationViews;

    /// <summary>
    ///     Registers the navigation window implementation.
    /// </summary>
    /// <typeparam name="TNavigationWindow">
    ///     Type of the navigation window
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="NavigationFeatureOptions"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public NavigationFeatureOptions WithNavigationWindow<TNavigationWindow>() where TNavigationWindow : INavigationWindow {
        return WithNavigationWindow(typeof(TNavigationWindow));
    }

    /// <summary>
    ///     Registers the navigation window implementation.
    /// </summary>
    /// <param name="type">
    ///     Type of the navigation window
    /// </param>
    /// <returns>
    ///     The current <see cref="NavigationFeatureOptions"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public NavigationFeatureOptions WithNavigationWindow(Type type) {
        Throws.When.NonConcreteType(type);
        Throws.When.OpenGenericType(type);
        Throws.When.NonAssignableFrom(type, typeof(INavigationWindow));

        NavigationWindow = type;

        return this;
    }

    /// <summary>
    ///     Registers the navigation view implementation.
    /// </summary>
    /// <typeparam name="TNavigationView">
    ///     Type of the navigation view
    /// </typeparam>
    /// <typeparam name="TView">
    ///     Type of the view
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="NavigationFeatureOptions"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public NavigationFeatureOptions WithNavigationView<TNavigationView, TView>() where TNavigationView : INavigableView<TView> {
        return WithNavigationView(typeof(TNavigationView));
    }

    /// <summary>
    ///     Registers the navigation view implementation.
    /// </summary>
    /// <param name="type">
    ///     Type of the navigation view
    /// </param>
    /// <returns>
    ///     The current <see cref="NavigationFeatureOptions"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public NavigationFeatureOptions WithNavigationView(Type type) {
        Throws.When.NonConcreteType(type);
        Throws.When.OpenGenericType(type);
        Throws.When.NonAssignableFromGeneric(type, NavigableViewType);

        _navigationViews.Add(type);

        return this;
    }
}