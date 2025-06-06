using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Nameless.Data;

/// <summary>
///     <see cref="IDataRecord" /> extension methods.
/// </summary>
public static class DataRecordExtensions {
    /// <summary>
    ///     Tries retrieve the value for the specific row/colum of a data record.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    /// <param name="self">The data record.</param>
    /// <param name="columnName">The colum name.</param>
    /// <param name="output">The output.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns><c>true</c> if was able to retrieve the data; otherwise <c>false</c>.</returns>
    public static bool TryGet<T>(this IDataRecord self,
                                 string columnName,
                                 [NotNullWhen(true)] out T? output,
                                 IFormatProvider? formatProvider = null) {
        output = default;

        var value = SafeGetValue(self, columnName);
        if (value is null) { return false; }

        object? current = null;

        try {
            current = Transform(value,
                typeof(T),
                formatProvider ?? CultureInfo.CurrentCulture);
        }
        catch { /* ignore */ }

        output = (T?)current;
        return current is not null;
    }

    /// <summary>
    ///     Retrieves a string value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>A string value.</returns>
    public static string GetString(this IDataRecord self, string columnName, IFormatProvider? formatProvider = null) {
        return self.TryGet<string>(columnName, out var result, formatProvider)
            ? result
            : string.Empty;
    }

    /// <summary>
    ///     Retrieves a string value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>A string value.</returns>
    public static string GetString(this IDataRecord self, string columnName, string fallback,
                                   IFormatProvider? formatProvider = null) {
        return self.TryGet<string>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves a boolean value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>A boolean value.</returns>
    public static bool GetBoolean(this IDataRecord self, string columnName, bool fallback = false,
                                  IFormatProvider? formatProvider = null) {
        return self.TryGet<bool>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves a char value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>A char value.</returns>
    public static char GetChar(this IDataRecord self, string columnName, char fallback = char.MinValue,
                               IFormatProvider? formatProvider = null) {
        return self.TryGet<char>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves a sbyte value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>A sbyte value.</returns>
    public static sbyte GetSByte(this IDataRecord self, string columnName, sbyte fallback = 0,
                                 IFormatProvider? formatProvider = null) {
        return self.TryGet<sbyte>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves a byte value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>A byte value.</returns>
    public static byte GetByte(this IDataRecord self, string columnName, byte fallback = 0,
                               IFormatProvider? formatProvider = null) {
        return self.TryGet<byte>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves a short value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>A short value.</returns>
    public static short GetInt16(this IDataRecord self, string columnName, short fallback = 0,
                                 IFormatProvider? formatProvider = null) {
        return self.TryGet<short>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves an ushort value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>An ushort value.</returns>
    public static ushort GetUInt16(this IDataRecord self, string columnName, ushort fallback = 0,
                                   IFormatProvider? formatProvider = null) {
        return self.TryGet<ushort>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves an int value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>An int value.</returns>
    public static int GetInt32(this IDataRecord self, string columnName, int fallback = 0,
                               IFormatProvider? formatProvider = null) {
        return self.TryGet<int>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves an uint value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>An uint value.</returns>
    public static uint GetUInt32(this IDataRecord self, string columnName, uint fallback = 0,
                                 IFormatProvider? formatProvider = null) {
        return self.TryGet<uint>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves a long value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>A long value.</returns>
    public static long GetInt64(this IDataRecord self, string columnName, long fallback = 0L,
                                IFormatProvider? formatProvider = null) {
        return self.TryGet<long>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves an ulong value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>An ulong value.</returns>
    public static ulong GetUInt64(this IDataRecord self, string columnName, ulong fallback = 0UL,
                                  IFormatProvider? formatProvider = null) {
        return self.TryGet<ulong>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves a float value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>A float value.</returns>
    public static float GetSingle(this IDataRecord self, string columnName, float fallback = 0F,
                                  IFormatProvider? formatProvider = null) {
        return self.TryGet<float>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves a double value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>A double value.</returns>
    public static double GetDouble(this IDataRecord self, string columnName, double fallback = 0D,
                                   IFormatProvider? formatProvider = null) {
        return self.TryGet<double>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves a decimal value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>A decimal value.</returns>
    public static decimal GetDecimal(this IDataRecord self, string columnName, decimal fallback = 0M,
                                     IFormatProvider? formatProvider = null) {
        return self.TryGet<decimal>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves a date/time value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>A date/time value.</returns>
    public static DateTime GetDateTime(this IDataRecord self, string columnName, DateTime fallback = default,
                                       IFormatProvider? formatProvider = null) {
        return self.TryGet<DateTime>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves a date/time offset value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>A date/time offset value.</returns>
    public static DateTimeOffset GetDateTimeOffset(this IDataRecord self, string columnName,
                                                   DateTimeOffset fallback = default,
                                                   IFormatProvider? formatProvider = null) {
        return self.TryGet<DateTimeOffset>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves a time span value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture" /></param>
    /// <returns>A time span value.</returns>
    public static TimeSpan GetTimeSpan(this IDataRecord self, string columnName, TimeSpan fallback = default,
                                       IFormatProvider? formatProvider = null) {
        return self.TryGet<TimeSpan>(columnName, out var result, formatProvider)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves a GUID value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <returns>A GUID value.</returns>
    public static Guid GetGuid(this IDataRecord self, string columnName, Guid fallback = default) {
        return self.TryGet<Guid>(columnName, out var result)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves an Enum value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <returns>An Enum value.</returns>
    public static TEnum GetEnum<TEnum>(this IDataRecord self, string columnName, TEnum fallback = default)
        where TEnum : struct {
        if (!typeof(TEnum).IsEnum) {
            throw new InvalidOperationException($"{typeof(TEnum).Name} must be an Enum.");
        }

        return self.TryGet<TEnum>(columnName, out var result)
            ? result
            : fallback;
    }

    /// <summary>
    ///     Retrieves a byte array (BLOB) value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <returns>A byte array value.</returns>
    public static byte[] GetBlob(this IDataRecord self, string columnName) {
        return self.TryGet<byte[]>(columnName, out var result)
            ? result
            : [];
    }

    /// <summary>
    ///     Retrieves a byte array (BLOB) value or the default value.
    /// </summary>
    /// <param name="self">The data record instance.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="fallback">The default value.</param>
    /// <returns>A byte array value.</returns>
    public static byte[] GetBlob(this IDataRecord self, string columnName, byte[] fallback) {
        return self.TryGet<byte[]>(columnName, out var result)
            ? result
            : fallback;
    }

    private static object? SafeGetValue(IDataRecord row, string columnName) {
        object? result;

        try { result = row[columnName]; }
        catch { result = null; }

        return result != DBNull.Value ? result : null;
    }

    private static object? Transform(object? value, Type type, IFormatProvider formatProvider) {
        if (value is null or DBNull) { return null; }

        if (type == typeof(Guid)) {
            return value switch {
                string stringValue => Guid.Parse(stringValue),
                _ => (Guid)value
            };
        }

        if (type == typeof(DateTimeOffset)) {
            return value switch {
                string stringValue => DateTimeOffset.Parse(stringValue, formatProvider),
                _ => (DateTimeOffset)value
            };
        }

        if (type == typeof(TimeSpan)) {
            return value switch {
                string stringValue => TimeSpan.Parse(stringValue, formatProvider),
                _ => (TimeSpan)value
            };
        }

        if (type.IsEnum) {
            return value switch {
                string stringValue => Enum.Parse(type, stringValue, false),
                int or Enum => (int)value,
                _ => null
            };
        }

        if (value is byte[] array) {
            return array;
        }

        return Type.GetTypeCode(type) switch {
            TypeCode.Boolean => Convert.ToBoolean(value, formatProvider),
            TypeCode.Char => Convert.ToChar(value, formatProvider),
            TypeCode.SByte => Convert.ToSByte(value, formatProvider),
            TypeCode.Byte => Convert.ToByte(value, formatProvider),
            TypeCode.Int16 => Convert.ToInt16(value, formatProvider),
            TypeCode.UInt16 => Convert.ToUInt16(value, formatProvider),
            TypeCode.Int32 => Convert.ToInt32(value, formatProvider),
            TypeCode.UInt32 => Convert.ToUInt32(value, formatProvider),
            TypeCode.Int64 => Convert.ToInt64(value, formatProvider),
            TypeCode.UInt64 => Convert.ToUInt64(value, formatProvider),
            TypeCode.Single => Convert.ToSingle(value, formatProvider),
            TypeCode.Double => Convert.ToDouble(value, formatProvider),
            TypeCode.Decimal => Convert.ToDecimal(value, formatProvider),
            TypeCode.DateTime => Convert.ToDateTime(value, formatProvider),
            TypeCode.String => Convert.ToString(value, formatProvider),
            _ => null
        };
    }
}