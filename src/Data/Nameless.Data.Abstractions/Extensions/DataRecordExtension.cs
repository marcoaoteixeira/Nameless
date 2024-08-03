using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Extensions.Primitives;

namespace Nameless.Data {
    /// <summary>
    /// <see cref="IDataRecord"/> extension methods.
    /// </summary>
    public static class DataRecordExtension {
        #region Public Static Methods

        /// <summary>
        /// Tries retrieve the value for the specific row/colum of a data record.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="self">The data record.</param>
        /// <param name="columnName">The colum name.</param>
        /// <param name="output">The output.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns><c>true</c> if was able to retrieve the data; otherwise <c>false</c>.</returns>
        public static bool TryGet<T>(this IDataRecord self, string columnName, [NotNullWhen(returnValue: true)] out T? output, IFormatProvider? formatProvider = null) {
            Guard.Against.NullOrWhiteSpace(columnName, nameof(columnName));

            output = default;

            var value = SafeGetValue(self, columnName);
            if (value is null) { return false; }

            var current = Transform(
                value, 
                typeof(T), 
                formatProvider ?? CultureInfo.CurrentCulture
            );

            output = (T?)current;
            return current is not null;
        }

        /// <summary>
        /// Retrieves a string value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>A string value.</returns>
        public static string GetString(this IDataRecord self, string columnName, IFormatProvider? formatProvider = null)
            => TryGet<string>(self, columnName, out var result, formatProvider)
                ? result
                : string.Empty;

        /// <summary>
        /// Retrieves a string value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>A string value.</returns>
        public static string GetString(this IDataRecord self, string columnName, string fallback, IFormatProvider? formatProvider = null)
            => TryGet<string>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves a boolean value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>A boolean value.</returns>
        public static bool GetBoolean(this IDataRecord self, string columnName, bool fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<bool>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves a char value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>A char value.</returns>
        public static char GetChar(this IDataRecord self, string columnName, char fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<char>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves a sbyte value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>A sbyte value.</returns>
        public static sbyte GetSByte(this IDataRecord self, string columnName, sbyte fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<sbyte>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves a byte value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>A byte value.</returns>
        public static byte GetByte(this IDataRecord self, string columnName, byte fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<byte>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves a short value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>A short value.</returns>
        public static short GetInt16(this IDataRecord self, string columnName, short fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<short>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves an ushort value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>An ushort value.</returns>
        public static ushort GetUInt16(this IDataRecord self, string columnName, ushort fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<ushort>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves an int value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>An int value.</returns>
        public static int GetInt32(this IDataRecord self, string columnName, int fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<int>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves an uint value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>An uint value.</returns>
        public static uint GetUInt32(this IDataRecord self, string columnName, uint fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<uint>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves a long value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>A long value.</returns>
        public static long GetInt64(this IDataRecord self, string columnName, long fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<long>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves an ulong value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>An ulong value.</returns>
        public static ulong GetUInt64(this IDataRecord self, string columnName, ulong fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<ulong>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves a float value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>A float value.</returns>
        public static float GetSingle(this IDataRecord self, string columnName, float fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<float>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves a double value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>A double value.</returns>
        public static double GetDouble(this IDataRecord self, string columnName, double fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<double>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves a decimal value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>A decimal value.</returns>
        public static decimal GetDecimal(this IDataRecord self, string columnName, decimal fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<decimal>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves a date/time value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>A date/time value.</returns>
        public static DateTime GetDateTime(this IDataRecord self, string columnName, DateTime fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<DateTime>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves a date/time offset value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>A date/time offset value.</returns>
        public static DateTimeOffset GetDateTimeOffset(this IDataRecord self, string columnName, DateTimeOffset fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<DateTimeOffset>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves a time span value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <param name="formatProvider">The format provider. Default is <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>A time span value.</returns>
        public static TimeSpan GetTimeSpan(this IDataRecord self, string columnName, TimeSpan fallback = default, IFormatProvider? formatProvider = null)
            => TryGet<TimeSpan>(self, columnName, out var result, formatProvider)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves a GUID value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A GUID value.</returns>
        public static Guid GetGuid(this IDataRecord self, string columnName, Guid fallback = default)
            => TryGet<Guid>(self, columnName, out var result, formatProvider: null)
                ? result
                : fallback;

        /// <summary>
        /// Retrieves an Enum value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>An Enum value.</returns>
        public static TEnum GetEnum<TEnum>(this IDataRecord self, string columnName, TEnum fallback = default)
            where TEnum : struct {
            Guard.Against.NullOrWhiteSpace(columnName, nameof(columnName));

            if (!typeof(TEnum).IsEnum) {
                throw new InvalidOperationException($"{typeof(TEnum).Name} must be an Enum.");
            }

            return TryGet<TEnum>(self, columnName, out var result, formatProvider: null)
                ? result
                : fallback;
        }

        /// <summary>
        /// Retrieves a byte array (BLOB) value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <returns>A byte array value.</returns>
        public static byte[] GetBlob(this IDataRecord self, string columnName)
            => TryGet<byte[]>(self, columnName, out var result, formatProvider: null)
                ? result
                : [];

        /// <summary>
        /// Retrieves a byte array (BLOB) value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A byte array value.</returns>
        public static byte[] GetBlob(this IDataRecord self, string columnName, byte[] fallback)
            => TryGet<byte[]>(self, columnName, out var result, formatProvider: null)
                ? result
                : fallback;

        #endregion

        #region Private Read-Only Methods

        private static object? SafeGetValue(IDataRecord row, string columnName) {
            object? result;

            try { result = row[columnName]; } catch { result = null; }

            return result == DBNull.Value ? null : result;
        }

        private static object? Transform(object? value, Type type, IFormatProvider formatProvider) {
            if (value is null or DBNull) { return default; }

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
                    string stringValue => Enum.Parse(type, stringValue, ignoreCase: false),
                    int or Enum => (int)value,
                    _ => default,
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
                _ => default
            };
        }

        #endregion
    }
}