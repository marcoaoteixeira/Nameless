using Wpf.Ui.Controls;

namespace Nameless.Windows.Navigation;

/// <summary>
///     Navigation View Item Provider
/// </summary>
public interface INavigationViewItemProvider {
    /// <summary>
    ///     Retrieves navigation items.
    /// </summary>
    /// <param name="placement">
    ///     The placement
    /// </param>
    /// <returns>
    ///     A collection of navigation view items.
    /// </returns>
    IEnumerable<NavigationViewItem> GetItems(Placement placement);
}