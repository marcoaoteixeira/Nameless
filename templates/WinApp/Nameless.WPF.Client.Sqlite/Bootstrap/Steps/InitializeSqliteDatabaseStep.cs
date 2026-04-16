using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Bootstrap;
using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Notification;
using Nameless.WPF.Bootstrap;
using Nameless.WPF.Client.Sqlite.Data;
using Nameless.WPF.Client.Sqlite.Resources;

namespace Nameless.WPF.Client.Sqlite.Bootstrap.Steps;

/// <summary>
///     Step to initialize the DbContext.
/// </summary>
public class InitializeSqliteDatabaseStep : StepBase {
    private readonly IServiceProvider _provider;

    public override IReadOnlyCollection<string> Dependencies => [
        nameof(EnsureApplicationDirectoriesExistenceStep)
    ];

    /// <inheritdoc />
    public override string DisplayName => Strings.InitializeSqliteDatabaseStep_DisplayName;
    
    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="InitializeSqliteDatabaseStep"/> class.
    /// </summary>
    /// <param name="provider">
    ///     The service provider.
    /// </param>
    public InitializeSqliteDatabaseStep(IServiceProvider provider) {
        _provider = provider;
    }

    /// <inheritdoc />
    public override async Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportStart(DisplayName, Strings.InitializeSqliteDatabaseStep_Progress_Start);
        await Task.Delay(BootstrapConstants.StepDelay, cancellationToken);

        var dbContext = _provider.GetRequiredService<AppDbContext>();
        var logger = _provider.GetLogger<InitializeSqliteDatabaseStep>();

        if (!dbContext.Database.IsRelational()) {
            progress.ReportComplete(DisplayName, Strings.InitializeSqliteDatabaseStep_Progress_NonRelationalDatabase);
            await Task.Delay(BootstrapConstants.StepDelay, cancellationToken);

            logger.SkipMigrationForNonRelationalDatabase();

            return;
        }

        progress.ReportInformation(DisplayName, Strings.InitializeSqliteDatabaseStep_Progress_EnsureDatabaseExistence);
        await Task.Delay(BootstrapConstants.StepDelay, cancellationToken);

        await dbContext.Database.EnsureCreatedAsync(cancellationToken).SkipContextSync();

        progress.ReportInformation(DisplayName, Strings.InitializeSqliteDatabaseStep_Progress_ApplyingMigrations);
        await Task.Delay(BootstrapConstants.StepDelay, cancellationToken);

        await dbContext.Database.MigrateAsync(cancellationToken).SkipContextSync();

        progress.ReportComplete(DisplayName, Strings.InitializeSqliteDatabaseStep_Progress_Complete);
        await Task.Delay(BootstrapConstants.StepDelay, cancellationToken);
    }
}
