using Nameless.Application;
using Nameless.Bootstrap;
using Nameless.Bootstrap.Notification;
using Nameless.Windows.Localization;

namespace Nameless.Windows.Bootstrap;

public class EnsureApplicationDirectoriesExistenceStep : StepBase {
    private const string CLASS = nameof(EnsureApplicationDirectoriesExistenceStep);
    private const int STEP_DELAY = 250;

    private ILocalizer T { get; }

    private readonly IApplicationContext _applicationContext;

    public override string DisplayName => T[$"{CLASS}_DisplayName"];

    public EnsureApplicationDirectoriesExistenceStep(IApplicationContext applicationContext, ILocalizer localizer) {
        _applicationContext = applicationContext;
        T = localizer;
    }

    public override async Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        const string ActionName = nameof(ExecuteAsync);

        progress.ReportInformation(DisplayName, T[$"{CLASS}_{ActionName}_CreateBackupDirectory"]);
        _applicationContext.FileSystemProvider.GetBackupDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);

        progress.ReportInformation(DisplayName, T[$"{CLASS}_{ActionName}_CreateDatabaseDirectory"]);
        _applicationContext.FileSystemProvider.GetDatabaseDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);

        progress.ReportInformation(DisplayName, T[$"{CLASS}_{ActionName}_CreateTemporaryDirectory"]);
        _applicationContext.FileSystemProvider.GetTemporaryDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);

        progress.ReportInformation(DisplayName, T[$"{CLASS}_{ActionName}_CreateUpdateDirectory"]);
        _applicationContext.FileSystemProvider.GetUpdateDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);
    }
}
