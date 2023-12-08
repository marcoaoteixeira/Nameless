using System.Data;
using Microsoft.Extensions.Logging;

namespace Nameless.Data {
    internal static class LoggerExtension {
        #region Internal Static Methods

        internal static void DbCommand(this ILogger self, IDbCommand command) {
            if (!self.IsEnabled(LogLevel.Debug)) { return; }

            self.LogDebug("Command text: {command.CommandText}", command);
            self.LogDebug("Parameter list:");
            foreach (var parameter in command.Parameters.OfType<IDbDataParameter>()) {
                self.LogDebug(
                    "\t[{parameter.DbType}] {parameter.ParameterName} => {parameter.Value}",
                    parameter.DbType,
                    parameter.ParameterName,
                    parameter.Value
                );
            }
        }

        #endregion
    }
}
