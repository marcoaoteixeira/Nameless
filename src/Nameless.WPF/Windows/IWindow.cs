using System.Windows;

namespace Nameless.WPF.Windows;

public interface IWindow {
    /// <summary>
    ///     Shows the window.
    /// </summary>
    /// <param name="startupLocation">
    ///     The window startup location in the screen.
    /// </param>
    void Show(WindowStartupLocation startupLocation);

    /// <summary>
    ///     Closes the window.
    /// </summary>
    void Close();
}