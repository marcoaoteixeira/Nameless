using Microsoft.Extensions.Logging;
using Nameless.WinApp.ViewModels.Windows;
using Nameless.Windows;
using Nameless.Windows.AppConfiguration;
using Nameless.Windows.Dialogs.Message;
using Nameless.Windows.Events;
using Nameless.Windows.Localization;
using Nameless.Windows.Notification.SnackBar;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Nameless.WinApp.Views.Windows;

public partial class MainWindow : INavigationWindow {
    private readonly IAppConfigurationManager _appConfigurationManager;
    private readonly IContentDialogService _contentDialogService;
    private readonly IMessageDialog _messageDialog;
    private readonly INavigationService _navigationService;
    private readonly INavigationViewPageProvider _navigationViewPageProvider;
    private readonly IEventPublisher _eventPublisher;
    private readonly ISnackbarService _snackBarService;
    private readonly ILogger<MainWindow> _logger;

    private bool _initialized;

    private ITranslator T { get; }

    public MainWindowViewModel ViewModel { get; }

    public MainWindow(
        MainWindowViewModel viewModel,
        IAppConfigurationManager appConfigurationManager,
        IContentDialogService contentDialogService,
        ITranslator translator,
        IMessageDialog messageDialog,
        INavigationService navigationService,
        INavigationViewPageProvider navigationViewPageProvider,
        IEventPublisher eventPublisher,
        ISnackbarService snackBarService,
        ILogger<MainWindow> logger) {
        _appConfigurationManager = appConfigurationManager;
        _contentDialogService = contentDialogService;
        _messageDialog = messageDialog;
        _navigationService = navigationService;
        _navigationViewPageProvider = navigationViewPageProvider;
        _eventPublisher = eventPublisher;
        _snackBarService = snackBarService;
        _logger = logger;

        ViewModel = viewModel;
        DataContext = ViewModel;
        T = translator;

        InitializeComponent();
        InitializeWindow();
    }

    public INavigationView GetNavigation() {
        return NavigationViewRoot;
    }

    public bool Navigate(Type pageType) {
        return GetNavigation().Navigate(pageType);
    }

    public void SetServiceProvider(IServiceProvider serviceProvider) {
        throw new NotImplementedException();
    }

    public void SetPageService(INavigationViewPageProvider navigationViewPageProvider) {
        GetNavigation().SetPageProviderService(navigationViewPageProvider);
    }

    public void ShowWindow() {
        Show();
    }

    public void CloseWindow() {
        Close();
    }

    private void ClosingHandler(object? _, CancelEventArgs args) {
        if (!_appConfigurationManager.ConfirmBeforeExit) {
            return;
        }

        var result = _messageDialog.ShowQuestion(
            title: T["MainWindow_ConfirmApplicationExit_MessageBox_Title"],
            message: T["MainWindow_ConfirmApplicationExit_MessageBox_Message"],
            buttons: MessageDialogButtons.YesNoCancel
        );

        if (result == MessageDialogResult.No) {
            _appConfigurationManager.ConfirmBeforeExit = false;
        }

        args.Cancel = result == MessageDialogResult.Cancel;
    }

    private void InitializeWindow() {
        if (_initialized) { return; }

        SetApplicationTheme();
        SetContentPresenter();
        SetNavigationView();
        SetPageService(_navigationViewPageProvider);
        SetSnackBarPresenter();
        SetWindowIcon();
        SubscribeForNotifications();

        _initialized = true;
    }

    private void SetApplicationTheme() {
        SystemThemeWatcher.Watch(this);
        var currentTheme = _appConfigurationManager.Theme;
        ApplicationThemeManager.Apply(currentTheme.ToApplicationTheme());
    }

    private void SetContentPresenter() {
        _contentDialogService.SetDialogHost(ContentDialogHostRoot);
    }

    private void SetNavigationView() {
        _navigationService.SetNavigationControl(NavigationViewRoot);
    }

    private void SetSnackBarPresenter() {
        _snackBarService.SetSnackbarPresenter(SnackBarPresenterRoot);
    }

    private void SetWindowIcon() {
        try { Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/imgs/application_64x64.png")); }
        catch (Exception ex) { CommonLog.Error(_logger, ex.Message, ex, tag: GetType().Tag); }
    }

    private void SubscribeForNotifications() {
        _eventPublisher.Register<SnackBarNotificationEvent>(this, ShowNotificationInSnackBar);
    }

    private void ShowNotificationInSnackBar(object sender, SnackBarNotificationEvent evt) {
        Dispatcher.InvokeAsync(() => _snackBarService.Show(evt));
    }
}