using Wpf.Ui.Controls;

namespace Nameless.Windows.Navigation;

public interface INavigationViewItemProvider {
    IEnumerable<NavigationViewItem> GetMainItems();
    IEnumerable<NavigationViewItem> GetFooterItems();
}