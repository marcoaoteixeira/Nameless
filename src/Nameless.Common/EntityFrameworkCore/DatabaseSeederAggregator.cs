using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nameless.Logging;
using Nameless.Registration;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     Database Seeder Aggregator.
/// </summary>
[IgnoreAssemblyScan]
public class DatabaseSeederAggregator : IDatabaseSeeder {
    private readonly IEnumerable<IDatabaseSeeder> _seeders;
    private readonly ILogger<DatabaseSeederAggregator> _logger;

    /// <inheritdoc />
    public int Order => int.MinValue;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="DatabaseSeederAggregator"/> class.
    /// </summary>
    /// <param name="seeders">
    ///     The internal database seeders.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public DatabaseSeederAggregator(IEnumerable<IDatabaseSeeder> seeders, ILogger<DatabaseSeederAggregator> logger) {
        _seeders = seeders.OrderBy(seeder => seeder.Order);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task ExecuteAsync(DbContext dbContext, bool storeManagementOperation, CancellationToken cancellationToken) {
        using var logger = _logger.StartStopwatchLogger();

        try {
            foreach (var seeder in _seeders) {
                logger.Write($"Starting seeder: {seeder.GetType().Name}");

                await seeder.ExecuteAsync(dbContext, storeManagementOperation, cancellationToken)
                            .SkipContextSync();

                logger.Write($"Seeder {seeder.GetType().Name} finished.");
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) {
            CommonLog.Error(_logger, ex.Message, ex, GetType().Tag);

            throw;
        }
    }

    /// <inheritdoc />
    public void Execute(DbContext dbContext, bool storeManagementOperation) {
        using var logger = _logger.StartStopwatchLogger();

        try {
            foreach (var seeder in _seeders) {
                logger.Write($"Starting seeder: {seeder.GetType().Name}");

                seeder.Execute(dbContext, storeManagementOperation);

                logger.Write($"Seeder {seeder.GetType().Name} finished.");
            }

            dbContext.SaveChanges();
        }
        catch (Exception ex) {
            CommonLog.Error(_logger, ex.Message, ex, GetType().Tag);

            throw;
        }
    }
}
