using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nameless.Application;
using Nameless.Mediator;
using Nameless.WinApp.Features.CheckSystemUpdate;
using Nameless.WinApp.Features.OpenCurrentLogFile;
using Nameless.WinApp.Features.PerformApplicationBackup;
using Nameless.WinApp.Features.PerformApplicationRestore;
using Nameless.WinApp.Features.PerformSystemUpdate;
using Nameless.WinApp.Infrastructure;
using Nameless.WinApp.Infrastructure.GitHub;
using Nameless.Windows;
using Nameless.Windows.AppConfiguration;
using Nameless.Windows.Dialogs.FileSystem;
using Nameless.Windows.Dialogs.Message;
using Nameless.Windows.Events;
using Nameless.Windows.Helpers;
using Nameless.Windows.Localization;
using Nameless.Windows.Mvvm;
using Nameless.Windows.Notification;
using Nameless.Windows.Notification.BackupRecovery;
using Nameless.Windows.Notification.SnackBar;
using Nameless.Windows.TaskRunner;
using Nameless.Windows.UI;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using Nameless.WinApp.Infrastructure.Events;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Appearance;

namespace Nameless.WinApp.ViewModels.Pages;

/// <summary>
///     App Configuration Page View Model
/// </summary>
public partial class AppConfigurationPageViewModel : ViewModel, INavigationAware {
    private readonly IAppConfigurationManager _appConfigurationManager;
    private readonly IApplicationContext _applicationContext;
    private readonly IApplicationRelauncher _applicationRelauncher;
    private readonly IEventPublisher _eventPublisher;
    private readonly IFileSystemDialog _fileSystemDialog;
    private readonly IMessageDialog _messageDialog;
    private readonly IMediator _mediator;
    private readonly ISystemUpdateEnvironment _systemUpdateEnvironment;
    private readonly ITaskRunner _taskRunner;

    private bool _initialized;

    private ITranslator<AppConfigurationPageViewModel> T { get; }

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
        IApplicationRelauncher applicationRelauncher,
        IEventPublisher eventPublisher,
        IFileSystemDialog fileSystemDialog,
        IMessageDialog messageDialog,
        ITranslator<AppConfigurationPageViewModel> translator,
        IMediator mediator,
        ISystemUpdateEnvironment systemUpdateEnvironment,
        ITaskRunner taskRunner) {
        _appConfigurationManager = appConfigurationManager;
        _applicationContext = applicationContext;
        _applicationRelauncher = applicationRelauncher;
        _eventPublisher = eventPublisher;
        _fileSystemDialog = fileSystemDialog;
        _messageDialog = messageDialog;
        _mediator = mediator;
        _systemUpdateEnvironment = systemUpdateEnvironment;
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
    private async Task PerformSystemUpdateAsync(CancellationToken cancellationToken) {
        try {
            var check = await _mediator.ExecuteAsync(new CheckSystemUpdateRequest(), cancellationToken);

            if (!check.IsUpdateAvailable) {
                await _eventPublisher.PublishInformationAsync<SnackBarNotificationEvent>(
                    message: T["PerformSystemUpdateAsync_UpToDate", check.Current.Format(includePrefix: true)]
                );

                return;
            }

            var release = check.Release!;

            var confirmation = _messageDialog.ShowQuestion(
                title: T["PerformSystemUpdateAsync_Confirm_Title"],
                message: T[
                    "PerformSystemUpdateAsync_Confirm_Message",
                    check.Current.Format(includePrefix: true),
                    release.Version.Format(includePrefix: true),
                    release.HtmlUrl?.ToString() ?? string.Empty
                ]
            );

            if (confirmation != MessageDialogResult.Yes) { return; }

            SystemUpdateOutcome? outcome = null;

            await _taskRunner.CreateBuilder()
                             .SetName(T["PerformSystemUpdateAsync_TaskRunnerWindow_Title"])
                             .SubscribeFor<SystemUpdateProgressNotificationEvent>()
                             .SetDelegate(async ct => outcome = await _mediator.ExecuteAsync(new PerformSystemUpdateRequest(release), ct))
                             .RunAsync();

            // RolledBack (including a cancelled update, which rolls back
            // too): the handler already published its own failure progress
            // entry and SnackBar toast — nothing left to do here.
            if (outcome is not SystemUpdateOutcome.Completed) { return; }

            // No second prompt: the user already agreed to a restart above,
            // and the running process is now an orphan of its install.
            try {
                _applicationRelauncher.RelaunchAndShutdown(_systemUpdateEnvironment.ExecutablePath, _systemUpdateEnvironment.InstallDirectoryPath);
            }
            catch (InvalidOperationException ex) {
                // The update is already committed. Do not roll back and do
                // not shut down — the old process keeps working from its
                // in-memory image until the user reopens it by hand.
                await _eventPublisher.PublishFailureAsync<SnackBarNotificationEvent>(
                    message: T["PerformSystemUpdateAsync_RelaunchFailed", ex.Message]
                );
            }
        }
        catch (GitHubReleaseClientException ex) when (ex.IsRateLimited) {
            var resetsAt = ex.RateLimitResetsAt?.ToLocalTime().ToString("f", CultureInfo.CurrentCulture) ??
                           T["PerformSystemUpdateAsync_RateLimited_UnknownReset"];

            await _eventPublisher.PublishFailureAsync<SnackBarNotificationEvent>(
                message: T["PerformSystemUpdateAsync_RateLimited", resetsAt]
            );
        }
        catch (Exception ex) {
            await _eventPublisher.PublishFailureAsync<SnackBarNotificationEvent>(
                message: T["PerformSystemUpdateAsync_Failure", ex.Message]
            );
        }
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
    private async Task PerformApplicationBackupAsync(CancellationToken cancellationToken) {
        string? backupFilePath = null;

        try {
            var outcome = await _taskRunner.CreateBuilder()
                                           .SetName(T["PerformApplicationBackupAsync_TaskRunnerWindow_Title"])
                                           .SubscribeFor<BackupRecoveryProgressNotificationEvent>()
                                           .SetDelegate(async ct => backupFilePath = await _mediator.ExecuteAsync(new PerformApplicationBackupRequest(), ct))
                                           .RunAsync();

            if (outcome is TaskRunnerOutcome.Completed && backupFilePath is not null) {
                await _eventPublisher.PublishSuccessAsync<SnackBarNotificationEvent>(
                    message: T["PerformApplicationBackupAsync_Success", backupFilePath]
                );
            }
            // Cancelled: nothing to report, the handler already published a cancellation-aware progress entry.
        } catch (Exception ex) {
            await _eventPublisher.PublishFailureAsync<SnackBarNotificationEvent>(
                message: T["PerformApplicationBackupAsync_Failure", ex.Message]
            );
        }
    }

    [RelayCommand]
    private async Task PerformApplicationRestoreAsync() {
        var filePath = _fileSystemDialog.OpenFile(o => {
            o.Title = T["PerformApplicationRestoreAsync_SelectFile_Title"];
            o.Filter = "Zip files (*.zip)|*.zip";
        }).FirstOrDefault();

        if (filePath is null) { return; }

        RestoreOutcome? restoreOutcome = null;

        try {
            await _taskRunner.CreateBuilder()
                             .SetName(T["PerformApplicationRestoreAsync_TaskRunnerWindow_Title"])
                             .SubscribeFor<BackupRecoveryProgressNotificationEvent>()
                             .SetDelegate(async ct => restoreOutcome = await _mediator.ExecuteAsync(new PerformApplicationRestoreRequest(filePath), ct))
                             .RunAsync();

            // RolledBack (including a cancelled restore, which rolls back
            // too): the handler already published its own failure progress
            // entry and SnackBar toast — nothing left to do here.
            if (restoreOutcome is not RestoreOutcome.Completed) { return; }

            var confirmation = _messageDialog.ShowQuestion(
                title: T["PerformApplicationRestoreAsync_ConfirmRestart_Title"],
                message: T["PerformApplicationRestoreAsync_ConfirmRestart_Message"]
            );

            if (confirmation != MessageDialogResult.Yes) { return; }

            using (Process.Start(Environment.ProcessPath!)) { }

            System.Windows.Application.Current.Shutdown();
        } catch (Exception ex) {
            await _eventPublisher.PublishFailureAsync<SnackBarNotificationEvent>(
                message: T["PerformApplicationRestoreAsync_Failure", ex.Message]
            );
        }
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
        _eventPublisher.PublishAsync(new CultureChangeEvent());

        _appConfigurationManager.Language = isoCode;
    }

    partial void OnCurrentConfirmBeforeExitChanged(bool oldValue, bool newValue) {
        if (!_initialized || oldValue == newValue) { return; }

        _appConfigurationManager.ConfirmBeforeExit = newValue;
    }
}
