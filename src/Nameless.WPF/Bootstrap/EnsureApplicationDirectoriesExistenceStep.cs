using Nameless.Application;
using Nameless.Bootstrap;
using Nameless.Bootstrap.Notification;
using Nameless.WPF.Resources;

namespace Nameless.WPF.Bootstrap;

public class EnsureApplicationDirectoriesExistenceStep : StepBase {
    private readonly IApplicationContext _applicationContext;

    public override string DisplayName => Strings.EnsureApplicationDirectoriesExistenceStep_DisplayName;

    public EnsureApplicationDirectoriesExistenceStep(IApplicationContext applicationContext) {
        _applicationContext = applicationContext;
    }

    public override async Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportInformation(DisplayName, Strings.EnsureApplicationDirectoriesExistenceStep_Progress_CreateBackupDirectory);
        _applicationContext.FileSystemProvider.GetBackupDirectory().Create();
        await Task.Delay(BootstrapConstants.StepDelay, cancellationToken);

        progress.ReportInformation(DisplayName, Strings.EnsureApplicationDirectoriesExistenceStep_Progress_CreateDatabaseDirectory);
        _applicationContext.FileSystemProvider.GetDatabaseDirectory().Create();
        await Task.Delay(BootstrapConstants.StepDelay, cancellationToken);

        progress.ReportInformation(DisplayName, Strings.EnsureApplicationDirectoriesExistenceStep_Progress_CreateTemporaryDirectory);
        _applicationContext.FileSystemProvider.GetTemporaryDirectory().Create();
        await Task.Delay(BootstrapConstants.StepDelay, cancellationToken);

        progress.ReportInformation(DisplayName, Strings.EnsureApplicationDirectoriesExistenceStep_Progress_CreateUpdateDirectory);
        _applicationContext.FileSystemProvider.GetUpdateDirectory().Create();
        await Task.Delay(BootstrapConstants.StepDelay, cancellationToken);
    }
}
