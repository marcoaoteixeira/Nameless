using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nameless.Infrastructure;
using Nameless.ObjectModel;
using Nameless.Results;
using Nameless.WPF.Client.Sqlite.Data;
using Nameless.WPF.Client.Sqlite.Resources;
using Nameless.WPF.DisasterRecovery;
using Nameless.WPF.Messaging;

namespace Nameless.WPF.Client.Sqlite.DisasterRecovery;

public class SqliteDisasterRecoveryRoutine : DisasterRecoveryRoutineBase<SqliteDisasterRecoveryRoutine> {
    private readonly IApplicationContext _applicationContext;
    private readonly AppDbContext _dbContext;

    public override string Name => Strings.SqliteDisasterRecoveryRoutine_Name;

    public SqliteDisasterRecoveryRoutine(
        IApplicationContext applicationContext,
        AppDbContext dbContext,
        IMessenger messenger,
        ILogger<SqliteDisasterRecoveryRoutine> logger
        ) : base(messenger, logger) {
        _applicationContext = applicationContext;
        _dbContext = dbContext;
    }

    public override async Task<BackupOutput> BackupAsync(BackupInput input, CancellationToken cancellationToken) {
        await NotifyInformationAsync(
            Strings.SqliteDisasterRecoveryRoutine_Report_Backup_Starting
        ).SkipContextSync();

        // since we're using EFCore, we first need to close the current DbContext connection
        await _dbContext.Database.CloseConnectionAsync().SkipContextSync();

        // gets the source file path for the application current database
        var sourceFilePath = await GetApplicationSqliteFilePathAsync().SkipContextSync();
        if (string.IsNullOrWhiteSpace(sourceFilePath)) { return BackupOutput.Empty; }

        // creates a backup file (destination) inside the temporary directory
        var destinationFilePath = _applicationContext.FileSystem
                                                     .GetDirectory(input.TemporaryDirectoryPath)
                                                     .GetFile(SqliteConstants.DatabaseFileName)
                                                     .Path;

        // creates the backup, copying the source file to the destination file.
        var backupResult = await ExecuteDatabaseBackupAsync(
            sourceFilePath,
            destinationFilePath,
            cancellationToken
        ).SkipContextSync();

        // if something goes wrong, return the error.
        if (backupResult.Failure) { return backupResult.Errors; }

        // ok, everything is fine.
        await NotifySuccessAsync(
            Strings.SqliteDisasterRecoveryRoutine_Report_Backup_Success
        ).SkipContextSync();

        return destinationFilePath;
    }

    public override async Task<RestoreOutput> RestoreAsync(RestoreInput input, CancellationToken cancellationToken) {
        await NotifyInformationAsync(
            Strings.SqliteDisasterRecoveryRoutine_Report_Restore_Starting
        ).SkipContextSync();

        // since we're using EFCore, we first need to close the current DbContext connection
        await _dbContext.Database.CloseConnectionAsync().SkipContextSync();

        // The source file is the backup file
        var sourceFilePath = await GetBackupFilePathAsync(
            input.TemporaryDirectoryPath
        ).SkipContextSync();

        if (string.IsNullOrWhiteSpace(sourceFilePath)) {
            return RestoreOutput.Ack;
        }

        // The destination file is the application database
        var destinationFilePath = await GetApplicationSqliteFilePathAsync().SkipContextSync();
        
        if (string.IsNullOrWhiteSpace(destinationFilePath)) {
            return RestoreOutput.Ack;
        }

        // Executes the restore.
        var restoreResult = await ExecuteDatabaseBackupAsync(
            sourceFilePath,
            destinationFilePath,
            cancellationToken
        ).SkipContextSync();

        // if something goes wrong, return the error.
        if (restoreResult.Failure) { return restoreResult.Errors; }

        // ok, everything is fine.
        await NotifySuccessAsync(
            Strings.SqliteDisasterRecoveryRoutine_Report_Backup_Success
        ).SkipContextSync();

        return RestoreOutput.Ack;
    }

    private async Task<Result<bool>> ExecuteDatabaseBackupAsync(string sourceFilePath, string destinationFilePath, CancellationToken cancellationToken) {
        SqliteConnection? sourceDbConnection = null;
        SqliteConnection? destinationDbConnection = null;

        try {
            sourceDbConnection = await CreateConnectionAsync(
                sourceFilePath,
                cancellationToken
            ).SkipContextSync();

            destinationDbConnection = await CreateConnectionAsync(
                destinationFilePath,
                cancellationToken
            ).SkipContextSync();

            sourceDbConnection.BackupDatabase(destinationDbConnection);

            return true;
        }
        catch (Exception ex) {
            await NotifyFailureAsync(
                string.Format(
                    Strings.SqliteDisasterRecoveryRoutine_Report_Backup_Failure,
                    ex.Message
                )
            ).SkipContextSync();

            Logger.BackupFailure(ex);

            return Error.Failure(ex.Message);
        }
        finally {
            if (destinationDbConnection is not null) {
                await destinationDbConnection.CloseAsync().SkipContextSync();
                await destinationDbConnection.DisposeAsync();
            }

            if (sourceDbConnection is not null) {
                await sourceDbConnection.CloseAsync().SkipContextSync();
                await sourceDbConnection.DisposeAsync();
            }
        }
    }

    private static async Task<SqliteConnection> CreateConnectionAsync(string filePath, CancellationToken cancellationToken) {
        var connStr = string.Format(SqliteConstants.ConnStrPattern, filePath);
        var result = new SqliteConnection(connStr);

        await result.OpenAsync(cancellationToken)
                    .SkipContextSync();

        return result;
    }

    private async Task<string> GetApplicationSqliteFilePathAsync() {
        var file = _applicationContext.FileSystem
                                      .GetDatabaseDirectory()
                                      .GetFile(SqliteConstants.DatabaseFileName);

        if (file.Exists) { return file.Path; }

        // If the database file does not exist, it might be because
        // the database was not properly initialized.
        await NotifyFailureAsync(
            Strings.SqliteDisasterRecoveryRoutine_Report_Missing_Application_Database_File
        ).SkipContextSync();

        return string.Empty;
    }

    private async Task<string> GetBackupFilePathAsync(string temporaryDirectoryPath) {
        // the backup file should be inside the temporary directory.
        var file = _applicationContext.FileSystem
                                      .GetDirectory(temporaryDirectoryPath)
                                      .GetFile(SqliteConstants.DatabaseFileName);

        if (file.Exists) { return file.Path; }

        // If the database file does not exist, it might be because
        // the backup file was not deflated
        await NotifyFailureAsync(
            Strings.SqliteDisasterRecoveryRoutine_Report_Missing_Backup_Database_File
        ).SkipContextSync();

        return string.Empty;
    }
}
