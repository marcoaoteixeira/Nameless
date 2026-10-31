using Wpf.Ui.Controls;

namespace Nameless.Windows.Navigation;

/// <summary>
///     Navigation View Item Provider
/// </summary>
public interface INavigationViewItemProvider {
    /// <summary>
    ///     Retrieves main navigation items.
    /// </summary>
    /// <returns>
    ///     A collection of navigation view items.
    /// </returns>
    IEnumerable<NavigationViewItem> GetMainItems();

    /// <summary>
    ///     Retrieves footer navigation items.
    /// </summary>
    /// <returns>
    ///     A collection of navigation view items.
    /// </returns>
    IEnumerable<NavigationViewItem> GetFooterItems();
}