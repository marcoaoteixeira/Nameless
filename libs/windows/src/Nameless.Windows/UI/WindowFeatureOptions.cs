using Nameless.Contracts;
using Nameless.Registration;

namespace Nameless.Windows.UI;

/// <summary>
///     Window Feature Options
/// </summary>
public class WindowFeatureOptions : AssemblyScanAware<WindowFeatureOptions> {
    private static readonly Type WindowType = typeof(IWindow);

    private readonly HashSet<Type> _windows = [];

    /// <summary>
    ///     Gets the <see cref="IWindow"/> implementations.
    /// </summary>
    public IReadOnlyCollection<Type> Windows => UseAssemblyScan
        ? ExecuteAssemblyScan(WindowType)
        : _windows;

    /// <summary>
    ///     Registers a <see cref="IWindow"/> implementation.
    /// </summary>
    /// <typeparam name="TWindow">
    ///     Type of the <see cref="IWindow"/> implementation.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="WindowFeatureOptions"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public WindowFeatureOptions WithWindow<TWindow>() where TWindow : class, IWindow {
        return WithWindow(typeof(TWindow));
    }

    /// <summary>
    ///     Registers a <see cref="IWindow"/> implementation.
    /// </summary>
    /// <param name="type">
    ///     Type of the <see cref="IWindow"/> implementation.
    /// </param>
    /// <returns>
    ///     The current <see cref="WindowFeatureOptions"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public WindowFeatureOptions WithWindow(Type type) {
        Throws.When.NonConcreteType(type);
        Throws.When.OpenGenericType(type);
        Throws.When.NonAssignableFrom(type, WindowType);

        _windows.Add(type);

        return this;
    }
}