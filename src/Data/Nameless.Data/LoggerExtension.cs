﻿using System.Data;
using System.Text;
using Nameless.Logging;

namespace Nameless.Data {
    internal static class LoggerExtension {
        #region Internal Static Methods

        internal static void Debug(this ILogger self, IDbCommand command) {
            if (!self.IsEnabled(LogLevel.Debug)) { return; }

            var sb = new StringBuilder();

            sb.AppendLine($"Command text: {command.CommandText}");
            sb.AppendLine();

            sb.AppendLine($"Parameter list:");
            foreach (var parameter in command.Parameters.OfType<IDbDataParameter>()) {
                sb.AppendLine($"\t[{parameter.DbType}] {parameter.ParameterName} => {parameter.Value}");
            }

            var log = sb.ToString();

            self.Debug(log);
        }

        #endregion
    }
}
