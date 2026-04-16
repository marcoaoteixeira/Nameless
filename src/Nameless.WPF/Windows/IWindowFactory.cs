namespace Nameless.WPF.Windows;

/// <summary>
///     Defines a window factory.
/// </summary>
public interface IWindowFactory {
    /// <summary>
    ///     Create a window.
    /// </summary>
    /// <typeparam name="TWindow">
    ///     Type of the window.
    /// </typeparam>
    /// <returns>
    ///     An instance of type <typeparamref name="TWindow"/> class.
    /// </returns>
    TWindow Create<TWindow>() where TWindow : IWindow;
}