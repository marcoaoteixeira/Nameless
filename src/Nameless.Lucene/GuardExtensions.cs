using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Nameless.Lucene;

/// <summary>
///     <see cref="Guard"/> extension methods.
/// </summary>
internal static class GuardExtensions {
    /// <summary>
    ///     Guards against parameters that do not match the specified
    ///     indexable type.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="Guard"/>.
    /// </param>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="type">
    ///     The parameter indexable type.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name.
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if the type of the <paramref name="paramValue"/> do not match
    ///     the indexable type.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="paramValue"/> is <see langword="null"/>.
    /// </exception>
    [DebuggerStepThrough]
    internal static object NullOrNonMatchingType(this Guard self, object paramValue, IndexableType type,
        [CallerArgumentExpression(nameof(paramValue))] string? paramName = null) {
        self.Null(paramValue, paramName);

        var paramValueType = paramValue.GetType();
        var matchIndexableType = type switch {
            IndexableType.Boolean => paramValueType == typeof(bool),
            IndexableType.String => paramValueType == typeof(string),
            IndexableType.Byte => paramValueType == typeof(byte),
            IndexableType.Short => paramValueType == typeof(short),
            IndexableType.Integer => paramValueType == typeof(int),
            IndexableType.Long => paramValueType == typeof(long),
            IndexableType.Float => paramValueType == typeof(float),
            IndexableType.Double => paramValueType == typeof(double),
            IndexableType.DateTimeOffset => paramValueType == typeof(DateTimeOffset),
            IndexableType.DateTime => paramValueType == typeof(DateTime),
            IndexableType.DateOnly => paramValueType == typeof(DateOnly),
            IndexableType.TimeOnly => paramValueType == typeof(TimeOnly),
            IndexableType.TimeSpan => paramValueType == typeof(TimeSpan),
            IndexableType.Enum => typeof(Enum).IsAssignableFrom(paramValueType),
            _ => false
        };

        if (!matchIndexableType) {
            throw new InvalidOperationException(
                $"{nameof(IndexableType)} '{type}' does not match underlying type of parameter '{paramName}'");
        }

        return paramValue;
    }
}