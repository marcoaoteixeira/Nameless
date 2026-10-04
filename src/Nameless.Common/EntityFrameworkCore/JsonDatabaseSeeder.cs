using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nameless.EntityFrameworkCore.Entities;
using Nameless.IO;
using Nameless.Logging;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     JSON Database Seeder
/// </summary>
/// <typeparam name="TEntity">
///     Type of the entity.
/// </typeparam>
public abstract class JsonDatabaseSeeder<TEntity> : IDatabaseSeeder
    where TEntity : EntityBase {
    private readonly IFileProvider _fileProvider;
    
    /// <summary>
    ///     Gets the logger.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    ///     Gets the options.
    /// </summary>
    public abstract JsonDatabaseSeederOptions Options { get; }

    /// <inheritdoc/>
    public abstract int Order { get; }

    /// <summary>
    ///     Base class constructor.
    /// </summary>
    /// <param name="fileProvider">
    ///     The file provider.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    protected JsonDatabaseSeeder(IFileProvider fileProvider, ILogger logger) {
        _fileProvider = fileProvider;

        Logger = logger;
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(DbContext dbContext, bool storeManagementOperation, CancellationToken cancellationToken) {
        using var logger = Logger.StartStopwatchLogger();

        try {
            var seeds = GetSeedsFromJsonFile();

            await ExecuteAsyncCore(dbContext, seeds, storeManagementOperation, cancellationToken).SkipContextSync();
        }
        catch (Exception ex) {
            CommonLog.Error(Logger, ex.Message, ex, GetType().Tag);

            throw;
        }
    }

    /// <inheritdoc/>
    public void Execute(DbContext dbContext, bool storeManagementOperation) {
        using var logger = Logger.StartStopwatchLogger();

        try {
            var seeds = GetSeedsFromJsonFile();

            ExecuteCore(dbContext, seeds, storeManagementOperation);
        }
        catch (Exception ex) {
            CommonLog.Error(Logger, ex.Message, ex, GetType().Tag);

            throw;
        }
    }

    /// <summary>
    ///     Executes the database seeder async.
    /// </summary>
    /// <param name="dbContext">
    ///     The database context.
    /// </param>
    /// <param name="seeds">
    ///     The seeds.
    /// </param>
    /// <param name="storeManagementOperation">
    ///     Whether it should store management operations.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the asynchronous method execution.
    /// </returns>
    protected abstract Task ExecuteAsyncCore(DbContext dbContext, TEntity[] seeds, bool storeManagementOperation, CancellationToken cancellationToken);

    /// <summary>
    ///     Executes the database seeder.
    /// </summary>
    /// <param name="dbContext">
    ///     The database context.
    /// </param>
    /// <param name="seeds">
    ///     The seeds.
    /// </param>
    /// <param name="storeManagementOperation">
    ///     Whether it should store management operations.
    /// </param>
    protected abstract void ExecuteCore(DbContext dbContext, TEntity[] seeds, bool storeManagementOperation);

    private TEntity[] GetSeedsFromJsonFile() {
        var stream = GetResourceStream();
        
        if (stream is null) {
            return Options.ThrowOnMissing
                ? throw new MissingDatabaseSeederResourceException(path: Options.RelativePath)
                : [];
        }

        try {
            return JsonSerializer.Deserialize<TEntity[]>(stream, Options.JsonOptions) ??
                   throw new InvalidOperationException($"Unable to deserialize seeds to type '{typeof(TEntity)}'.");
        }
        finally { stream.Dispose(); }
    }

    private Stream? GetResourceStream() {
        var file = _fileProvider.GetFile(Options.RelativePath);

        return file.Exists ? file.Open(FileMode.Open, FileAccess.Read) : null;
    }
}