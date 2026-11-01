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

    private ITranslator T { get; }

    /// <inheritdoc />
    public override IReadOnlyCollection<string> Dependencies => [nameof(SetUILanguageStep)];

    /// <inheritdoc />
    public override string DisplayName => T.Get(nameof(DisplayName));

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
    public EnsureApplicationDirectoriesExistenceStep(IApplicationContext applicationContext, ITranslator translator) {
        _applicationContext = Throws.When.Null(applicationContext);

        T = Throws.When.Null(translator);
    }

    /// <inheritdoc />
    public override async Task ExecuteAsync(IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportInformation(DisplayName, T.Get("CreateBackupDirectory"));
        _applicationContext.FileManager.GetBackupDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);

        progress.ReportInformation(DisplayName, T.Get("CreateDatabaseDirectory"));
        _applicationContext.FileManager.GetDatabaseDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);

        progress.ReportInformation(DisplayName, T.Get("CreateTemporaryDirectory"));
        _applicationContext.FileManager.GetTemporaryDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);

        progress.ReportInformation(DisplayName, T.Get("CreateUpdateDirectory"));
        _applicationContext.FileManager.GetUpdateDirectory().Create();
        await Task.Delay(STEP_DELAY, cancellationToken);
    }
}
