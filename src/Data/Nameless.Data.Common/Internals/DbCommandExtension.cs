﻿using System.Data;

namespace Nameless.Data.Internals;

internal static class DbCommandExtension {
    internal static string GetParameterList(this IDbCommand self) {
        Prevent.Argument.Null(self);

        var list = self.Parameters
                       .OfType<IDbDataParameter>()
                       .Select(parameter => $"[{parameter.DbType}] {parameter.ParameterName} => {parameter.Value}")
                       .ToArray();

        return string.Join(Constants.Separators.COMMA, list);
    }
}