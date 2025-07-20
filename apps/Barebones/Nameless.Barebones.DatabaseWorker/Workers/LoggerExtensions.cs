namespace Nameless.Barebones.Worker.Workers;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> MigrationWorkerStartedDelegate =
        LoggerMessage.Define<string>(
            logLevel: LogLevel.Information,
            eventId: Events.MigrationWorkerStartedEvent,
            formatString: "Migrating database '{DatabaseName}' started.");

    private static readonly Action<ILogger, string, Exception?> MigrationWorkerFinishedDelegate =
        LoggerMessage.Define<string>(
            logLevel: LogLevel.Information,
            eventId: Events.MigrationWorkerFinishedEvent,
            formatString: "Migrating database '{DatabaseName}' finished.");

    internal static void MigrationWorkerStarted(this ILogger<DatabaseMigrationWorker> logger, string databaseName) {
        MigrationWorkerStartedDelegate(logger, databaseName, null /* exception */);
    }

    internal static void MigrationWorkerFinished(this ILogger<DatabaseMigrationWorker> logger, string databaseName) {
        MigrationWorkerFinishedDelegate(logger, databaseName, null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId MigrationWorkerStartedEvent = new(1, nameof(MigrationWorkerStarted));
        internal static readonly EventId MigrationWorkerFinishedEvent = new(2, nameof(MigrationWorkerFinished));
    }
}
