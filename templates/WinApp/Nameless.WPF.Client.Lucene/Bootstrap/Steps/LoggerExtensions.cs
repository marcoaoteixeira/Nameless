using Microsoft.Extensions.Logging;

namespace Nameless.WPF.Client.Lucene.Bootstrap.Steps;

/// <summary>
///     Extension methods for <see cref="ILogger"/> that target
///     the <see cref="AppDbContext"/> implementations.
/// </summary>
internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception?> SkipMigrationForNonRelationalDatabaseDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "DbContext references a non-relational database, skipping migration action."
        );

    internal static void SkipMigrationForNonRelationalDatabase(this ILogger<InitializeLuceneDatabaseStep> self) {
        SkipMigrationForNonRelationalDatabaseDelegate(self, null /* exception */);
    }
}
