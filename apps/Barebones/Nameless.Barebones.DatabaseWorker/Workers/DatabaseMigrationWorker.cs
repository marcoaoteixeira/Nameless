using Microsoft.EntityFrameworkCore;
using Nameless.Barebones.Domains;
using Nameless.Barebones.Infrastructure.Monitoring;

namespace Nameless.Barebones.Worker.Workers;

public class DatabaseMigrationWorker : BackgroundService {
    public static readonly string ActivitySourceName = nameof(DatabaseMigrationWorker);

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseMigrationWorker> _logger;
    private readonly IActivitySourceManager _activitySourceManager;

    public DatabaseMigrationWorker(
        IServiceProvider serviceProvider,
        IActivitySourceManager activitySourceManager,
        ILogger<DatabaseMigrationWorker> logger) {

        _serviceProvider = Prevent.Argument.Null(serviceProvider);
        _activitySourceManager = Prevent.Argument.Null(activitySourceManager);
        _logger = Prevent.Argument.Null(logger);
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        const string DatabaseName = "barebones-db";
        const string ActivityName = $"{nameof(DatabaseMigrationWorker)}.{nameof(ExecuteAsync)}";
        using var activity = _activitySourceManager.GetActivitySource(ActivitySourceName)
                                                   .StartActivity(ActivityName);

        try {
            using var scope = _serviceProvider.CreateScope();

            var applicationDbContext = scope.ServiceProvider
                                            .GetRequiredService<ApplicationDbContext>();

            _logger.MigrationWorkerStarted(DatabaseName);

            await ExecuteMigrationAsync(applicationDbContext, stoppingToken);
        }
        catch (Exception ex) {
            activity?.AddException(ex);

            throw;
        }
        finally { _logger.MigrationWorkerFinished(DatabaseName); }
    }

    private static async Task ExecuteMigrationAsync(ApplicationDbContext applicationDbContext, CancellationToken cancellationToken) {
        var strategy = applicationDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () => {
            await using var transaction = await applicationDbContext.Database.BeginTransactionAsync(cancellationToken);

            await applicationDbContext.Database.MigrateAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        });
    }
}
