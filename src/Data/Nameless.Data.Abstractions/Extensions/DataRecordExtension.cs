using System.Data;

namespace Nameless.Data {
    /// <summary>
    /// <see cref="IDataRecord"/> extension methods.
    /// </summary>
    public static class DataRecordExtension {
        #region Public Static Methods

        /// <summary>
        /// Retrieves a string value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A string value.</returns>
        public static string? GetString(this IDataRecord self, string fieldName, string? fallback = null) {
            Guard.Against.NullOrWhiteSpace(fieldName, nameof(fieldName));

            var value = SafeGetValue(self, fieldName);

            return value is null ? fallback : (string)value;
        }

        /// <summary>
        /// Retrieves a boolean value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A boolean value.</returns>
        public static bool GetBoolean(this IDataRecord self, string fieldName, bool fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves a char value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A char value.</returns>
        public static char GetChar(this IDataRecord self, string fieldName, char fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves a sbyte value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A sbyte value.</returns>
        public static sbyte GetSByte(this IDataRecord self, string fieldName, sbyte fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves a byte value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A byte value.</returns>
        public static byte GetByte(this IDataRecord self, string fieldName, byte fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves a short value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A short value.</returns>
        public static short GetInt16(this IDataRecord self, string fieldName, short fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves an ushort value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>An ushort value.</returns>
        public static ushort GetUInt16(this IDataRecord self, string fieldName, ushort fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves an int value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>An int value.</returns>
        public static int GetInt32(this IDataRecord self, string fieldName, int fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves an uint value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>An uint value.</returns>
        public static uint GetUInt32(this IDataRecord self, string fieldName, uint fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves a long value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A long value.</returns>
        public static long GetInt64(this IDataRecord self, string fieldName, long fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves an ulong value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>An ulong value.</returns>
        public static ulong GetUInt64(this IDataRecord self, string fieldName, ulong fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves a float value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A float value.</returns>
        public static float GetSingle(this IDataRecord self, string fieldName, float fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves a double value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A double value.</returns>
        public static double GetDouble(this IDataRecord self, string fieldName, double fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves a decimal value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A decimal value.</returns>
        public static decimal GetDecimal(this IDataRecord self, string fieldName, decimal fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves a date/time value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A date/time value.</returns>
        public static DateTime GetDateTime(this IDataRecord self, string fieldName, DateTime fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves a date/time offset value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A date/time offset value.</returns>

        public static DateTimeOffset GetDateTimeOffset(this IDataRecord self, string fieldName, DateTimeOffset fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves a time span value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A time span value.</returns>

        public static TimeSpan GetTimeSpan(this IDataRecord self, string fieldName, TimeSpan fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves a GUID value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A GUID value.</returns>
        public static Guid GetGuid(this IDataRecord self, string fieldName, Guid fallback = default)
            => GetStruct(self, fieldName, fallback);

        /// <summary>
        /// Retrieves an Enum value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>An Enum value.</returns>
        public static TEnum GetEnum<TEnum>(this IDataRecord self, string fieldName, TEnum fallback = default)
            where TEnum : struct {
            Guard.Against.NullOrWhiteSpace(fieldName, nameof(fieldName));

            if (!typeof(TEnum).IsEnum) {
                throw new InvalidOperationException($"{nameof(TEnum)} must be an Enum.");
            }

            var value = SafeGetValue(self, fieldName);

            return value switch {
                string => (TEnum)Enum.Parse(typeof(TEnum), (string)value),
                int => (TEnum)value,
                _ => fallback
            };
        }

        /// <summary>
        /// Retrieves a byte array (BLOB) value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A byte array value.</returns>
        public static byte[] GetBlob(this IDataRecord self, string fieldName)
            => self.GetBlob(fieldName, []);

        /// <summary>
        /// Retrieves a byte array (BLOB) value or the default value.
        /// </summary>
        /// <param name="self">The data record instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fallback">The default value.</param>
        /// <returns>A byte array value.</returns>
        public static byte[] GetBlob(this IDataRecord self, string fieldName, byte[] fallback) {
            Guard.Against.NullOrWhiteSpace(fieldName, nameof(fieldName));

            var value = SafeGetValue(self, fieldName);

            return value is null
                ? fallback
                : (byte[])value;
        }

        #endregion

        #region Private Read-Only Methods

        private static TStruct GetStruct<TStruct>(IDataRecord self, string fieldName, TStruct fallback = default)
            where TStruct : struct {
            Guard.Against.NullOrWhiteSpace(fieldName, nameof(fieldName));

            var value = SafeGetValue(self, fieldName);

            return value is null ? fallback : (TStruct)value;
        }

        private static object? SafeGetValue(IDataRecord record, string fieldName) {
            if (record is null) { return null; }

            object? result;

            try { result = record[fieldName]; } catch { result = null; }

            return result == DBNull.Value ? null : result;
        }

        #endregion
    }
}