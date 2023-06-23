﻿using System.Data;

namespace Nameless.Data {

    /// <summary>
    /// Extension methods for <see cref="IDataRecord"/>
    /// </summary>
    public static class DataRecordExtension {

        #region Public Static Methods

        /// <summary>
        /// Retrieves a string value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A string value.</returns>
        public static string? GetStringOrDefault(this IDataRecord self, string fieldName, string? defaultValue = default) {
            Prevent.NullOrWhiteSpaces(fieldName, nameof(fieldName));

            var value = SafeGetValue(self, fieldName);

            return value != default ? (string)value : defaultValue;
        }

        /// <summary>
        /// Retrieves a boolean value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A boolean value.</returns>
        public static bool? GetBooleanOrDefault(this IDataRecord self, string fieldName, bool? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves a char value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A char value.</returns>
        public static char? GetCharOrDefault(this IDataRecord self, string fieldName, char? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves a sbyte value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A sbyte value.</returns>
        public static sbyte? GetSByteOrDefault(this IDataRecord self, string fieldName, sbyte? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves a byte value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A byte value.</returns>
        public static byte? GetByteOrDefault(this IDataRecord self, string fieldName, byte? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves a short value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A short value.</returns>
        public static short? GetInt16OrDefault(this IDataRecord self, string fieldName, short? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves an ushort value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>An ushort value.</returns>
        public static ushort? GetUInt16OrDefault(this IDataRecord self, string fieldName, ushort? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves an int value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>An int value.</returns>
        public static int? GetInt32OrDefault(this IDataRecord self, string fieldName, int? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves an uint value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>An uint value.</returns>
        public static uint? GetUInt32OrDefault(this IDataRecord self, string fieldName, uint? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves a long value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A long value.</returns>
        public static long? GetInt64OrDefault(this IDataRecord self, string fieldName, long? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves an ulong value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>An ulong value.</returns>
        public static ulong? GetUInt64OrDefault(this IDataRecord self, string fieldName, ulong? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves a float value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A float value.</returns>
        public static float? GetSingleOrDefault(this IDataRecord self, string fieldName, float? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves a double value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A double value.</returns>
        public static double? GetDoubleOrDefault(this IDataRecord self, string fieldName, double? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves a decimal value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A decimal value.</returns>
        public static decimal? GetDecimalOrDefault(this IDataRecord self, string fieldName, decimal? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves a date/time value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A date/time value.</returns>
        public static DateTime? GetDateTimeOrDefault(this IDataRecord self, string fieldName, DateTime? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves a date/time offset value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A date/time offset value.</returns>

        public static DateTimeOffset? GetDateTimeOffsetOrDefault(this IDataRecord self, string fieldName, DateTimeOffset? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves a time span value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A time span value.</returns>

        public static TimeSpan? GetTimeSpanOrDefault(this IDataRecord self, string fieldName, TimeSpan? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves a GUID value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A GUID value.</returns>
        public static Guid? GetGuidOrDefault(this IDataRecord self, string fieldName, Guid? defaultValue = default) => GetStructOrDefault(self, fieldName, defaultValue);

        /// <summary>
        /// Retrieves an Enum value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>An Enum value.</returns>
        public static TEnum? GetEnumOrDefault<TEnum>(this IDataRecord self, string fieldName, TEnum? defaultValue = default) where TEnum : struct {
            Prevent.NullOrWhiteSpaces(fieldName, nameof(fieldName));
            if (!typeof(TEnum).IsEnum) { throw new InvalidOperationException($"{nameof(TEnum)} must be an Enum."); }

            var value = SafeGetValue(self, fieldName);
            if (value == default) { return defaultValue.HasValue ? new TEnum?(defaultValue.Value) : default; }

            return (TEnum)Enum.Parse(typeof(TEnum), value.ToString()!);
        }

        /// <summary>
        /// Retrieves an Enum value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>An Enum value.</returns>
        public static object? GetEnumOrDefault(this IDataRecord self, Type enumType, string fieldName, object? defaultValue = default) {
            Prevent.Null(enumType, nameof(enumType));
            if (!enumType.IsEnum) { throw new InvalidOperationException($"{nameof(enumType)} must be an Enum."); }
            if (defaultValue != default && !defaultValue.GetType().IsEnum) { throw new InvalidOperationException($"{nameof(defaultValue)} must be an Enum."); }

            var value = SafeGetValue(self, fieldName);

#pragma warning disable IDE0054
            value = value ?? defaultValue;
#pragma warning restore IDE0054

            return (value != default && !value.GetType().IsEnum)
                ? Enum.Parse(enumType, value.ToString()!)
                : value;
        }

        /// <summary>
        /// Retrieves a byte array (BLOB) value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A byte array value.</returns>
        public static byte[]? GetBlobOrDefault(this IDataRecord self, string fieldName, byte[]? defaultValue = default) {
            Prevent.NullOrWhiteSpaces(fieldName, nameof(fieldName));

            var value = SafeGetValue(self, fieldName);

            return value != default
                ? (byte[])value
                : defaultValue;
        }

        #endregion

        #region Private Read-Only Methods

        private static TStruct? GetStructOrDefault<TStruct>(this IDataRecord self, string fieldName, TStruct? defaultValue = default)
        where TStruct : struct {
            Prevent.NullOrWhiteSpaces(fieldName, nameof(fieldName));

            var value = SafeGetValue(self, fieldName);

            return value == default
                ? defaultValue.HasValue
                    ? new TStruct?(defaultValue.Value)
                    : default
                : (TStruct?)value;
        }

        private static object? SafeGetValue(this IDataRecord record, string fieldName) {
            if (record == default) { return default; }
            object? result;
            try { result = record[fieldName]; } catch { result = default; }
            return result != DBNull.Value ? result : default;
        }

        #endregion
    }
}