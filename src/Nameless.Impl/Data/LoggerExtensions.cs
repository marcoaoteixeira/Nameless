using System.Data;
using Microsoft.Extensions.Logging;

namespace Nameless.Data;

internal static class DatabaseLoggerExtensions {
    private const string TAG = "ADO DATABASE";

    extension(ILogger<Database> self) {
        internal void OutputDbCommand(IDbCommand dbCommand) {
            Log.DatabaseOutputDbCommand(
                self,
                TAG,
                dbCommand.CommandText,
                dbCommand.Parameters
            );
        }

        internal void ExecuteNonQueryFailure(Exception exception) {
            Log.Failure(
                logger: self,
                tag: TAG,
                actionName: $"{nameof(Database)}.{nameof(Database.ExecuteNonQuery)}",
                exception: exception
            );
        }

        internal void ExecuteScalarFailure(Exception exception) {
            Log.Failure(
                logger: self,
                tag: TAG,
                actionName: $"{nameof(Database)}.{nameof(Database.ExecuteScalar)}",
                exception: exception
            );
        }

        internal void ExecuteReaderFailure(Exception exception) {
            Log.Failure(
                logger: self,
                tag: TAG,
                actionName: $"{nameof(Database)}.{nameof(Database.ExecuteReader)}",
                exception: exception
            );
        }
    }
}