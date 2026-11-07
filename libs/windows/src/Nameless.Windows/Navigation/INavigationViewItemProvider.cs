using Wpf.Ui.Controls;

namespace Nameless.Windows.Navigation;

/// <summary>
///     Navigation View Item Provider
/// </summary>
public interface INavigationViewItemProvider {
    /// <summary>
    ///     Retrieves navigation menu items.
    /// </summary>
    /// <returns>
    ///     A collection of navigation view items.
    /// </returns>
    IEnumerable<NavigationViewItem> GetMenuItems();

    /// <summary>
    ///     Retrieves navigation footer items.
    /// </summary>
    /// <returns>
    ///     A collection of navigation view items.
    /// </returns>
    IEnumerable<NavigationViewItem> GetFooterItems();
}