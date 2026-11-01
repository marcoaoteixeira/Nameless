using Wpf.Ui.Controls;

namespace Nameless.Windows.Navigation;

/// <summary>
///     Navigation View Item Provider Extension Methods
/// </summary>
public static class NavigationViewItemProviderExtensions {
    extension(INavigationViewItemProvider self) {
        /// <summary>
        ///     Retrieves navigation items for default
        ///     placement <see cref="Placement.Top"/>.
        /// </summary>
        /// <returns>
        ///     A collection of navigation view items.
        /// </returns>
        public IEnumerable<NavigationViewItem> GetItems() {
            return self.GetItems(Placement.Top);
        }
    }
}