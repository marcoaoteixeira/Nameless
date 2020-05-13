using System;
using System.Data;

namespace Nameless.Data {

    /// <summary>
    /// Extension methods for <see cref="IDataRecord"/>
    /// </summary>
    public static class DataRecordExtension {

        #region Public Static Methods

        /// <summary>
        /// Retrieves a string value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>A string value.</returns>
        public static string GetStringOrDefault (this IDataRecord source, string fieldName, string defaultValue = default) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);

            return value != null ? (string)value : defaultValue;
        }

        /// <summary>
        /// Retrieves a boolean value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>A boolean value.</returns>
        public static bool? GetBooleanOrDefault (this IDataRecord source, string fieldName, bool? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new bool? (defaultValue.Value) : null; }

            return (bool?)value;
        }

        /// <summary>
        /// Retrieves a char value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>A char value.</returns>
        public static char? GetCharOrDefault (this IDataRecord source, string fieldName, char? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new char? (defaultValue.Value) : null; }

            return (char?)value;
        }
        /// <summary>
        /// Retrieves a sbyte value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>A sbyte value.</returns>
        public static sbyte? GetSByteOrDefault (this IDataRecord source, string fieldName, sbyte? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new sbyte? (defaultValue.Value) : null; }

            return Convert.ToSByte (value);
        }
        /// <summary>
        /// Retrieves a byte value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>A byte value.</returns>
        public static byte? GetByteOrDefault (this IDataRecord source, string fieldName, byte? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new byte? (defaultValue.Value) : null; }

            return Convert.ToByte (value);
        }
        /// <summary>
        /// Retrieves a short value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>A short value.</returns>
        public static short? GetInt16OrDefault (this IDataRecord source, string fieldName, short? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new short? (defaultValue.Value) : null; }

            return Convert.ToInt16 (value);
        }
        /// <summary>
        /// Retrieves an ushort value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>An ushort value.</returns>
        public static ushort? GetUInt16OrDefault (this IDataRecord source, string fieldName, ushort? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new ushort? (defaultValue.Value) : null; }

            return Convert.ToUInt16 (value);
        }
        /// <summary>
        /// Retrieves an int value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>An int value.</returns>
        public static int? GetInt32OrDefault (this IDataRecord source, string fieldName, int? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new int? (defaultValue.Value) : null; }

            return Convert.ToInt32 (value);
        }
        /// <summary>
        /// Retrieves an uint value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>An uint value.</returns>
        public static uint? GetUInt32OrDefault (this IDataRecord source, string fieldName, uint? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new uint? (defaultValue.Value) : null; }

            return Convert.ToUInt32 (value);
        }
        /// <summary>
        /// Retrieves a long value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>A long value.</returns>
        public static long? GetInt64OrDefault (this IDataRecord source, string fieldName, long? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new long? (defaultValue.Value) : null; }

            return Convert.ToInt64 (value);
        }
        /// <summary>
        /// Retrieves an ulong value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>An ulong value.</returns>
        public static ulong? GetUInt64OrDefault (this IDataRecord source, string fieldName, ulong? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new ulong? (defaultValue.Value) : null; }

            return Convert.ToUInt64 (value);
        }
        /// <summary>
        /// Retrieves a float value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>A float value.</returns>
        public static float? GetSingleOrDefault (this IDataRecord source, string fieldName, float? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new float? (defaultValue.Value) : null; }

            return Convert.ToSingle (value);
        }
        /// <summary>
        /// Retrieves a double value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>A double value.</returns>
        public static double? GetDoubleOrDefault (this IDataRecord source, string fieldName, double? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new double? (defaultValue.Value) : null; }

            return Convert.ToDouble (value);
        }
        /// <summary>
        /// Retrieves a decimal value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>A decimal value.</returns>
        public static decimal? GetDecimalOrDefault (this IDataRecord source, string fieldName, decimal? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new decimal? (defaultValue.Value) : null; }

            return Convert.ToDecimal (value);
        }
        /// <summary>
        /// Retrieves a date/time value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>A date/time value.</returns>
        public static DateTime? GetDateTimeOrDefault (this IDataRecord source, string fieldName, DateTime? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new DateTime? (defaultValue.Value) : null; }

            return (DateTime?)value;
        }
        /// <summary>
        /// Retrieves a date/time offset value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>A date/time offset value.</returns>

        public static DateTimeOffset? GetDateTimeOffsetOrDefault (this IDataRecord source, string fieldName, DateTimeOffset? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new DateTimeOffset? (defaultValue.Value) : null; }

            return (DateTimeOffset?)value;
        }
        /// <summary>
        /// Retrieves a time span value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>A time span value.</returns>

        public static TimeSpan? GetTimeSpanOrDefault (this IDataRecord source, string fieldName, TimeSpan? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new TimeSpan? (defaultValue.Value) : null; }

            return (TimeSpan?)value;
        }
        /// <summary>
        /// Retrieves a GUID value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>A GUID value.</returns>
        public static Guid? GetGuidOrDefault (this IDataRecord source, string fieldName, Guid? defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new Guid? (defaultValue.Value) : null; }

            return Guid.Parse (value.ToString ());
        }
        /// <summary>
        /// Retrieves an Enum value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>An Enum value.</returns>
        public static TEnum? GetEnumOrDefault<TEnum> (this IDataRecord source, string fieldName, TEnum? defaultValue = null) where TEnum : struct {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));
            if (!typeof (TEnum).IsEnum) { throw new InvalidOperationException ($"{nameof (TEnum)} must be an Enum."); }

            var value = SafeGetValue (source, fieldName);
            if (value == null) { return defaultValue.HasValue ? new TEnum? (defaultValue.Value) : null; }

            return (TEnum)Enum.Parse (typeof (TEnum), value.ToString ());
        }
        /// <summary>
        /// Retrieves an Enum value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>An Enum value.</returns>
        public static object GetEnumObjectOrDefault (this IDataRecord source, Type enumType, string fieldName, object defaultValue = null) {
            Prevent.ParameterNull (enumType, nameof (enumType));
            if (!enumType.IsEnum) { throw new InvalidOperationException ($"{enumType.Name} must be an Enum."); }
            if (defaultValue != null && !defaultValue.GetType ().IsEnum) { throw new InvalidOperationException ($"{defaultValue.GetType ().Name} must be an Enum."); }

            return typeof (DataRecordExtension)
                .GetType ()
                .GetMethod (nameof (GetEnumOrDefault))
                .MakeGenericMethod (enumType)
                .Invoke (null /* instance */, new object[] { fieldName, defaultValue });
        }
        /// <summary>
        /// Retrieves a byte array (BLOB) value or the default value.
        /// </summary>
        /// <param name="source">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="defaultValue">The default value value.</param>
        /// <returns>A byte array value.</returns>
        public static byte[] GetBlobOrDefault (this IDataRecord source, string fieldName, byte[] defaultValue = null) {
            Prevent.ParameterNullOrWhiteSpace (fieldName, nameof (fieldName));

            var value = SafeGetValue (source, fieldName);

            return value != null ? (byte[])value : defaultValue;
        }

        #endregion

        #region Private Read-Only Fields

        private static object SafeGetValue (this IDataRecord record, string fieldName) {
            if (record == null) { return null; }
            object result;
            try { result = record[fieldName]; } catch { result = null; }
            return result != DBNull.Value ? result : null;
        }

        #endregion
    }
}