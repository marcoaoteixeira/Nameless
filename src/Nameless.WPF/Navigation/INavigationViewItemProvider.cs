using Wpf.Ui.Controls;

namespace Nameless.WPF.Navigation;

public interface INavigationViewItemProvider {
    IEnumerable<NavigationViewItem> GetMainItems();
    IEnumerable<NavigationViewItem> GetFooterItems();
}