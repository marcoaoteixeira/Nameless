using Microsoft.Extensions.DependencyInjection;
using Nameless.WinApp.ViewModels.Pages;
using Nameless.Windows.DependencyInjection;
using Nameless.Windows.Navigation;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace Nameless.WinApp.Views.Pages;

[ServiceLifetime(Lifetime = ServiceLifetime.Singleton)]
[NavigationViewItem(
    Icon = SymbolRegular.Settings48,
    Title = Constants.Labels.NavigationMenuItems.Configuration,
    Footer = true
)]
public partial class AppConfigurationPage : INavigableView<AppConfigurationPageViewModel> {
    public AppConfigurationPageViewModel ViewModel { get; }

    public AppConfigurationPage(AppConfigurationPageViewModel viewModel) {
        ViewModel = viewModel;

        DataContext = ViewModel;

        InitializeComponent();
    }
}
