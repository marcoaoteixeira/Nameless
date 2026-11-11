using Moq;
using Nameless.Application;
using Nameless.Mediator;
using Nameless.ObjectModel;
using Nameless.WinApp.Features.CheckSystemUpdate;
using Nameless.WinApp.Features.PerformSystemUpdate;
using Nameless.WinApp.Infrastructure;
using Nameless.WinApp.Infrastructure.GitHub;
using Nameless.WinApp.Tests.Tooling;
using Nameless.Windows.AppConfiguration;
using Nameless.Windows.Dialogs.FileSystem;
using Nameless.Windows.Dialogs.Message;
using Nameless.Windows.Events;
using Nameless.Windows.Notification;
using Nameless.Windows.Notification.SnackBar;
using Nameless.Windows.TaskRunner;

namespace Nameless.WinApp.ViewModels.Pages;

// Only the early-exit branches that never reach ITaskRunner.CreateBuilder()
// are exercised here. ITaskRunner.CreateBuilder() returns the concrete,
// non-virtual TaskRunnerBuilder — tied to a real WPF window/dispatcher —
// so it cannot be stubbed with Moq. That is a pre-existing limitation: the
// already-shipped PerformApplicationBackupAsync/PerformApplicationRestoreAsync
// have no ViewModel-level tests either. The Completed-outcome relaunch path
// and the "relaunch throws" path therefore stay manual/E2E-only despite the
// new IApplicationRelauncher seam (see the handover report).
public class AppConfigurationPageViewModelTests {
    private sealed class Harness {
        public required Mock<IMediator> MediatorMock { get; init; }
        public required Mock<IMessageDialog> MessageDialogMock { get; init; }
        public required Mock<IEventPublisher> EventPublisherMock { get; init; }
        public required Mock<ITaskRunner> TaskRunnerMock { get; init; }
        public required AppConfigurationPageViewModel ViewModel { get; init; }
    }

    // The view model's constructor builds WPF ComboBoxItem controls
    // (ComboBoxItemHelper.EmptyComboBoxItem), which — like every WPF
    // control — requires an STA thread even when no window is ever shown.
    // This is a pre-existing limitation shared by every RelayCommand on
    // this view model, not something specific to the update flow.
    private static void RunOnStaThread(Func<Task> action) {
        Exception? captured = null;

        var thread = new Thread(() => {
            try { action().GetAwaiter().GetResult(); }
            catch (Exception ex) { captured = ex; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (captured is not null) { throw captured; }
    }

    private static Harness CreateHarness() {
        var mediatorMock = new Mock<IMediator>();
        var messageDialogMock = new Mock<IMessageDialog>();
        var eventPublisherMock = new Mock<IEventPublisher>();
        eventPublisherMock.Setup(x => x.PublishAsync(It.IsAny<SnackBarNotificationEvent>())).Returns(Task.CompletedTask);
        var taskRunnerMock = new Mock<ITaskRunner>();

        var environment = new FakeSystemUpdateEnvironment {
            InstallDirectoryPath = SysPath.Combine(SysPath.GetTempPath(), "install"),
            ExecutablePath = SysPath.Combine(SysPath.GetTempPath(), "install", "Nameless.WinApp.exe")
        };

        var viewModel = new AppConfigurationPageViewModel(
            appConfigurationManager: new Mock<IAppConfigurationManager>().Object,
            applicationContext: new Mock<IApplicationContext>().Object,
            applicationRelauncher: new Mock<IApplicationRelauncher>().Object,
            eventPublisher: eventPublisherMock.Object,
            fileSystemDialog: new Mock<IFileSystemDialog>().Object,
            messageDialog: messageDialogMock.Object,
            translator: FakeTranslator.For<AppConfigurationPageViewModel>(),
            mediator: mediatorMock.Object,
            systemUpdateEnvironment: environment,
            taskRunner: taskRunnerMock.Object
        );

        return new Harness {
            MediatorMock = mediatorMock,
            MessageDialogMock = messageDialogMock,
            EventPublisherMock = eventPublisherMock,
            TaskRunnerMock = taskRunnerMock,
            ViewModel = viewModel
        };
    }

    private static AvailableRelease CreateAvailableRelease() {
        return new AvailableRelease(
            Version: SemVersion.Parse("1.1.0"),
            TagName: "winapp-v1.1.0",
            HtmlUrl: new Uri("https://github.com/marcoaoteixeira/Nameless/releases/tag/winapp-v1.1.0"),
            Package: new ReleaseAsset("Nameless.WinApp-1.1.0-win-x64.zip", 123, new Uri("https://github.com/marcoaoteixeira/Nameless/releases/download/winapp-v1.1.0/Nameless.WinApp-1.1.0-win-x64.zip")),
            Checksum: new ReleaseAsset("Nameless.WinApp-1.1.0-win-x64.zip.sha256", 64, new Uri("https://github.com/marcoaoteixeira/Nameless/releases/download/winapp-v1.1.0/Nameless.WinApp-1.1.0-win-x64.zip.sha256"))
        );
    }

    [Fact]
    [UnitTest]
    public void PerformSystemUpdateAsync_WhenUpToDate_ShowsInformationSnackBarAndNeverShowsConfirmDialog() {
        RunOnStaThread(async () => {
            var harness = CreateHarness();

            harness.MediatorMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CheckSystemUpdateRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SystemUpdateCheckResult(SemVersion.Parse("1.0.0"), Release: null));

            await harness.ViewModel.PerformSystemUpdateCommand.ExecuteAsync(null);

            harness.MessageDialogMock.Verify(x => x.Show(It.IsAny<string>(), It.IsAny<Action<MessageDialogOptions>>()), Times.Never);
            harness.TaskRunnerMock.Verify(x => x.CreateBuilder(), Times.Never);

            harness.EventPublisherMock.Verify(
                x => x.PublishAsync(It.Is<SnackBarNotificationEvent>(e => e.Type == NotificationType.Information)),
                Times.Once
            );
        });
    }

    [Fact]
    [UnitTest]
    public void PerformSystemUpdateAsync_WhenUserDeclinesConfirmation_NeverRunsTheUpdate() {
        RunOnStaThread(async () => {
            var harness = CreateHarness();

            harness.MediatorMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CheckSystemUpdateRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SystemUpdateCheckResult(SemVersion.Parse("1.0.0"), CreateAvailableRelease()));

            harness.MessageDialogMock
                .Setup(x => x.Show(It.IsAny<string>(), It.IsAny<Action<MessageDialogOptions>>()))
                .Returns(MessageDialogResult.No);

            await harness.ViewModel.PerformSystemUpdateCommand.ExecuteAsync(null);

            harness.TaskRunnerMock.Verify(x => x.CreateBuilder(), Times.Never);
            harness.MediatorMock.Verify(x => x.ExecuteAsync(It.IsAny<PerformSystemUpdateRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        });
    }

    [Fact]
    [UnitTest]
    public void PerformSystemUpdateAsync_WhenCheckThrows_ShowsFailureSnackBarWithExceptionMessage() {
        RunOnStaThread(async () => {
            var harness = CreateHarness();

            harness.MediatorMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CheckSystemUpdateRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("could not reach GitHub"));

            await harness.ViewModel.PerformSystemUpdateCommand.ExecuteAsync(null);

            // FakeTranslator echoes the key itself (see its doc comment);
            // the real resx value is formatted with ex.Message at runtime —
            // this pins which key the generic-failure branch uses.
            harness.EventPublisherMock.Verify(
                x => x.PublishAsync(It.Is<SnackBarNotificationEvent>(e =>
                    e.Type == NotificationType.Failure &&
                    e.Message.Contains("AppConfigurationPageViewModel_PerformSystemUpdateAsync_Failure", StringComparison.Ordinal))),
                Times.Once
            );
        });
    }

    [Fact]
    [UnitTest]
    public void PerformSystemUpdateAsync_WhenCheckThrowsRateLimitedException_ShowsRateLimitedSpecificText() {
        RunOnStaThread(async () => {
            var harness = CreateHarness();

            harness.MediatorMock
                .Setup(x => x.ExecuteAsync(It.IsAny<CheckSystemUpdateRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new GitHubReleaseClientException("rate limited", isRateLimited: true, rateLimitResetsAt: DateTimeOffset.UtcNow.AddHours(1)));

            await harness.ViewModel.PerformSystemUpdateCommand.ExecuteAsync(null);

            harness.EventPublisherMock.Verify(
                x => x.PublishAsync(It.Is<SnackBarNotificationEvent>(e =>
                    e.Type == NotificationType.Failure && e.Message.Contains("RateLimited", StringComparison.Ordinal))),
                Times.Once
            );
        });
    }
}
