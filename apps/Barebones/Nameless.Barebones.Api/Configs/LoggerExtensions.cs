using Nameless.Barebones.Domains;

namespace Nameless.Barebones.Api.Configs;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> MigrationStartedDelegate =
        LoggerMessage.Define<string>(
            logLevel: LogLevel.Information,
            eventId: Events.MigrationStartedEvent,
            formatString: "Migrating database '{DatabaseName}' started.");

    private static readonly Action<ILogger, string, Exception?> MigrationFinishedDelegate =
        LoggerMessage.Define<string>(
            logLevel: LogLevel.Information,
            eventId: Events.MigrationFinishedEvent,
            formatString: "Migrating database '{DatabaseName}' finished.");

    internal static void MigrationStarted(this ILogger<ApplicationDbContext> logger, string databaseName) {
        MigrationStartedDelegate(logger, databaseName, null /* exception */);
    }

    internal static void MigrationFinished(this ILogger<ApplicationDbContext> logger, string databaseName) {
        MigrationFinishedDelegate(logger, databaseName, null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId MigrationStartedEvent = new(1, nameof(MigrationStarted));
        internal static readonly EventId MigrationFinishedEvent = new(2, nameof(MigrationFinished));
    }
}
