using Nameless.Application;
using Nameless.Contracts;
using Nameless.WinApp.Infrastructure;
using Nameless.Windows;
using Nameless.Windows.Bootstrap;
using Nameless.Windows.Bootstrap.Notification;
using Nameless.Windows.Localization;

namespace Nameless.WinApp.Bootstrap.Steps;

/// <summary>
///     Ensure Application Directories Existence Step
/// </summary>
public class EnsureApplicationDirectoriesExistenceStep : StepBase {
    private const int STEP_DELAY = 500;

    private readonly IApplicationContext _applicationContext;

    private ITranslator<EnsureApplicationDirectoriesExistenceStep> T { get; }

    /// <inheritdoc />
    public override string DisplayName => T["DisplayName"];

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="EnsureApplicationDirectoriesExistenceStep"/> class.
    /// </summary>
    /// <param name="applicationContext">
    ///     The application context.
    /// </param>
    /// <param name="translator">
    ///     The translator service.
    /// </param>
    public EnsureApplicationDirectoriesExistenceStep(IApplicationContext applicationContext, ITranslator<EnsureApplicationDirectoriesExistenceStep> translator) {
        _applicationContext = Throws.When.Null(applicationContext);

        T = Throws.When.Null(translator);
    }

    /// <inheritdoc />
    public override async Task ExecuteAsync(IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportInformation(DisplayName, T["ExecuteAsync_CreateBackupDirectory"]);
        _applicationContext.FileManager.GetBackupDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);

        progress.ReportInformation(DisplayName, T["ExecuteAsync_CreateDatabaseDirectory"]);
        _applicationContext.FileManager.GetDatabaseDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);

        progress.ReportInformation(DisplayName, T["ExecuteAsync_CreateTemporaryDirectory"]);
        _applicationContext.FileManager.GetTemporaryDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);

        progress.ReportInformation(DisplayName, T["ExecuteAsync_CreateUpdateDirectory"]);
        _applicationContext.FileManager.GetUpdateDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);
    }
}
