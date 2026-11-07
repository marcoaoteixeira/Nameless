using Nameless.WinApp.ViewModels.Pages;
using Nameless.Windows.Navigation;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace Nameless.WinApp.Views.Pages;

[NavigationViewItem(
    Icon = SymbolRegular.Settings48,
    Placement = Placement.Footer,
    TitleResourceName = "AppConfigurationPage_NavigationViewItem_Title",
    ToolTipResourceName = "AppConfigurationPage_NavigationViewItem_ToolTip",
    ResourceType = typeof(Resources.loc.TranslationResources)
)]
public partial class AppConfigurationPage : INavigableView<AppConfigurationPageViewModel> {
    public AppConfigurationPageViewModel ViewModel { get; }

    public AppConfigurationPage(AppConfigurationPageViewModel viewModel) {
        ViewModel = viewModel;

        DataContext = ViewModel;

        InitializeComponent();
    }
}
