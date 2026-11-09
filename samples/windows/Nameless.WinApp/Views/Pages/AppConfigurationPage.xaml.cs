using Nameless.WinApp.ViewModels.Pages;
using Nameless.Windows.Navigation;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace Nameless.WinApp.Views.Pages;

// NOTE (#190): previously resolved through NavigationViewItemAttribute's
// resx-backed ResourceType/*ResourceName indirection. That resx pair was
// retired with ResourceTranslator; AutoDiscoverableNavigationViewItemProvider
// now resolves TitleResourceName/ToolTipResourceName through the active
// ITranslator (JsonTranslator) instead when no ResourceType is set — this
// stays culture-aware at startup (App.OnCurrentHostStart resolves the
// translator and applies the configured culture before the nav menu is
// first built) and live on language switch (MainWindowViewModel rebuilds
// the menu on CultureChangeEvent).
[NavigationViewItem(
    Icon = SymbolRegular.Settings48,
    Placement = Placement.Footer,
    TitleResourceName = "AppConfigurationPage_NavigationViewItem_Title",
    ToolTipResourceName = "AppConfigurationPage_NavigationViewItem_ToolTip"
)]
public partial class AppConfigurationPage : INavigableView<AppConfigurationPageViewModel> {
    public AppConfigurationPageViewModel ViewModel { get; }

    public AppConfigurationPage(AppConfigurationPageViewModel viewModel) {
        ViewModel = viewModel;

        DataContext = ViewModel;

        InitializeComponent();
    }
}
