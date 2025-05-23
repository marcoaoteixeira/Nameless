﻿using System.Data;
using Microsoft.Extensions.Logging;

namespace Nameless.Data.Internals;

internal static class LoggerExtension {
    private static readonly Action<ILogger, string, string, Exception?> OutputDbCommandDelegate
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Debug,
                                               eventId: default,
                                               formatString: "Command text: {CommandText} | Parameter list: {ParameterList}",
                                               options: null);

    internal static void OutputDbCommand(this ILogger<Database> self, IDbCommand dbCommand)
        => OutputDbCommandDelegate(self, dbCommand.CommandText, dbCommand.GetParameterList(), null /* exception */);

    private static readonly Action<ILogger, Exception> ErrorOnExecuteNonQueryCommandDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "An error occurs while executing non-query command.",
                               options: null);

    internal static void ErrorOnExecuteNonQueryCommand(this ILogger<Database> self, Exception exception)
        => ErrorOnExecuteNonQueryCommandDelegate(self, exception);

    private static readonly Action<ILogger, Exception> ErrorOnExecuteScalarErrorDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "An error occurs while executing scalar command.",
                               options: null);

    internal static void ErrorOnExecuteScalarCommand(this ILogger<Database> self, Exception exception)
        => ErrorOnExecuteScalarErrorDelegate(self, exception);

    private static readonly Action<ILogger, Exception> ErrorOnExecuteReaderCommandDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "An error occurs while executing reader command.",
                               options: null);

    internal static void ErrorOnExecuteReaderCommand(this ILogger<Database> self, Exception exception)
        => ErrorOnExecuteReaderCommandDelegate(self, exception);

    private static string GetParameterList(this IDbCommand self) {
        var list = self.Parameters
                       .OfType<IDbDataParameter>()
                       .Select(parameter => $"[{parameter.DbType}] {parameter.ParameterName} => {parameter.Value}")
                       .ToArray();

        return string.Join(";", list);
    }
}