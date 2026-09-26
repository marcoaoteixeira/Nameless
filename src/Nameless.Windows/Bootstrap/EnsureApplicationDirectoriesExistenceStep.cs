using Nameless.Application;
using Nameless.Windows.Bootstrap.Notification;
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

    public override async Task ExecuteAsync(IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        const string ActionName = nameof(ExecuteAsync);

        progress.ReportInformation(DisplayName, T[$"{CLASS}_{ActionName}_CreateBackupDirectory"]);
        _applicationContext.ApplicationDataFileProvider.GetBackupDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);

        progress.ReportInformation(DisplayName, T[$"{CLASS}_{ActionName}_CreateDatabaseDirectory"]);
        _applicationContext.ApplicationDataFileProvider.GetDatabaseDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);

        progress.ReportInformation(DisplayName, T[$"{CLASS}_{ActionName}_CreateTemporaryDirectory"]);
        _applicationContext.ApplicationDataFileProvider.GetTemporaryDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);

        progress.ReportInformation(DisplayName, T[$"{CLASS}_{ActionName}_CreateUpdateDirectory"]);
        _applicationContext.ApplicationDataFileProvider.GetUpdateDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);
    }
}
