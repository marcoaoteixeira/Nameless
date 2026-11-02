using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nameless.Application;
using Nameless.Mediator;
using Nameless.WinApp.Features.OpenCurrentLogFile;
using Nameless.Windows;
using Nameless.Windows.AppConfiguration;
using Nameless.Windows.Dialogs.FileSystem;
using Nameless.Windows.Helpers;
using Nameless.Windows.Localization;
using Nameless.Windows.Mvvm;
using Nameless.Windows.TaskRunner;
using Nameless.Windows.UI;
using System.Globalization;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Appearance;

namespace Nameless.WinApp.ViewModels.Pages;

/// <summary>
///     App Configuration Page View Model
/// </summary>
public partial class AppConfigurationPageViewModel : ViewModel, INavigationAware {
    private readonly IAppConfigurationManager _appConfigurationManager;
    private readonly IApplicationContext _applicationContext;
    private readonly IFileSystemDialog _fileSystemDialog;
    private readonly IMediator _mediator;
    private readonly ITaskRunner _taskRunner;

    private bool _initialized;

    private ITranslator T { get; }

    [ObservableProperty]
    public partial ComboBoxItem CurrentTheme { get; set; } = ComboBoxItemHelper.EmptyComboBoxItem;

    [ObservableProperty]
    public partial ComboBoxItem CurrentLanguage { get; set; } = ComboBoxItemHelper.EmptyComboBoxItem;

    [ObservableProperty]
    public partial bool CurrentConfirmBeforeExit { get; set; }

    public string AppVersion { get; private set; } = string.Empty;

    public ComboBoxItem[] AvailableThemes { get; } = [
        Theme.System.ToComboBoxItem(),
        Theme.Light.ToComboBoxItem(),
        Theme.Dark.ToComboBoxItem(),
        Theme.HighContrast.ToComboBoxItem(),
    ];

    public ComboBoxItem[] AvailableLanguages { get; } = [
        ComboBoxItemHelper.Create("pt-BR", "Português (Brasil)"),
        ComboBoxItemHelper.Create("en-GB", "English (United Kingdom)"),
    ];

    public AppConfigurationPageViewModel(
        IAppConfigurationManager appConfigurationManager,
        IApplicationContext applicationContext,
        IFileSystemDialog fileSystemDialog,
        ITranslator translator,
        IMediator mediator,
        ITaskRunner taskRunner) {
        _appConfigurationManager = appConfigurationManager;
        _applicationContext = applicationContext;
        _fileSystemDialog = fileSystemDialog;
        _mediator = mediator;
        _taskRunner = taskRunner;

        T = translator;
    }

    public Task OnNavigatedToAsync() {
        Initialize();

        return Task.CompletedTask;
    }

    public Task OnNavigatedFromAsync() {
        return Task.CompletedTask;
    }

    private void Initialize() {
        if (_initialized) { return; }

        AppVersion = _applicationContext.Version;

        var theme = _appConfigurationManager.Theme;
        var culture = CultureResolver.TryGet(_appConfigurationManager.Language, out var output)
            ? output
            : CultureInfo.CurrentUICulture;

        CurrentTheme = theme.Find(AvailableThemes);
        CurrentConfirmBeforeExit = _appConfigurationManager.ConfirmBeforeExit;
        CurrentLanguage = culture.Find(AvailableLanguages);

        _initialized = true;
    }

    [RelayCommand]
    private Task PerformSystemUpdateAsync() {
        //return _taskRunner.CreateBuilder()
        //                  .SetName(T["AppConfigurationPageViewModel_PerformSystemUpdate_TaskRunnerWindow_Title"])
        //                  .SubscribeFor<UseCaseMessage>()
        //                  .SetDelegate(ExecuteSystemUpdateAsync)
        //                  .RunAsync();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OpenApplicationDataDirectoryAsync() {
        ProcessHelper.OpenDirectory(_applicationContext.FileManager.Root);

        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task OpenApplicationLogFileAsync(CancellationToken cancellationToken) {
        await _mediator.ExecuteAsync(new OpenCurrentLogFileRequest(), cancellationToken);
    }

    [RelayCommand]
    private Task PerformApplicationBackupAsync() {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task PerformApplicationRestoreAsync() {
        return Task.CompletedTask;
    }

    partial void OnCurrentThemeChanged(ComboBoxItem oldValue, ComboBoxItem newValue) {
        if (!_initialized || oldValue.Tag == newValue.Tag) { return; }

        var theme = (Theme)newValue.Tag;

        ApplicationThemeManager.Apply(theme.ToApplicationTheme());

        _appConfigurationManager.Theme = theme;
    }

    partial void OnCurrentLanguageChanged(ComboBoxItem oldValue, ComboBoxItem newValue) {
        if (!_initialized || oldValue.Tag == newValue.Tag) { return; }

        var isoCode = (string)newValue.Tag;

        T.SetCulture(isoCode);

        _appConfigurationManager.Language = isoCode;
    }

    partial void OnCurrentConfirmBeforeExitChanged(bool oldValue, bool newValue) {
        if (!_initialized || oldValue == newValue) { return; }

        _appConfigurationManager.ConfirmBeforeExit = newValue;
    }

    private Task ExecuteApplicationBackupAsync(CancellationToken cancellationToken) {
        //_ = await _mediator.ExecuteAsync(
        //    new PerformApplicationBackupRequest(),
        //    cancellationToken
        //).SkipContextSync();

        return Task.CompletedTask;
    }

    private Task ExecuteSystemUpdateAsync(CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
