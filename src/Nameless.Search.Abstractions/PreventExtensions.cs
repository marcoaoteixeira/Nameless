using System.Diagnostics;

namespace Nameless.Search;

public static class PreventExtensions {
    [DebuggerStepThrough]
    public static object NullOrNonMatchingType(this Guard self, object paramValue, IndexableType type, string paramName) {
        self.Null(paramValue, paramName);

        var valueType = paramValue.GetType();
        var matchesIndexableType = type switch {
            IndexableType.Boolean => valueType == typeof(bool),
            IndexableType.String => valueType == typeof(string),
            IndexableType.Byte => valueType == typeof(byte),
            IndexableType.Short => valueType == typeof(short),
            IndexableType.Integer => valueType == typeof(int),
            IndexableType.Long => valueType == typeof(long),
            IndexableType.Float => valueType == typeof(float),
            IndexableType.Double => valueType == typeof(double),
            IndexableType.DateTimeOffset => valueType == typeof(DateTimeOffset),
            IndexableType.DateTime => valueType == typeof(DateTime),
            _ => false
        };

        if (!matchesIndexableType) {
            throw new InvalidOperationException($"{nameof(IndexableType)} '{type}' does not match underlying type of parameter '{paramName}'");
        }

        return paramValue;
    }
}