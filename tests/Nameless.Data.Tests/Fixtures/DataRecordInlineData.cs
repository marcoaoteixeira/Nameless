using System.Collections;

namespace Nameless.Data.Fixtures;
public class DataRecordInlineData : IEnumerable<object[]> {
    public IEnumerator<object[]> GetEnumerator() {
        yield return [nameof(DataRecordExtensions.GetString), "Value"];
        yield return [nameof(DataRecordExtensions.GetBoolean), true];
        yield return [nameof(DataRecordExtensions.GetChar), 'X'];
        yield return [nameof(DataRecordExtensions.GetSByte), (sbyte)16];
        yield return [nameof(DataRecordExtensions.GetByte), (byte)32];
        yield return [nameof(DataRecordExtensions.GetInt16), (short)64];
        yield return [nameof(DataRecordExtensions.GetUInt16), (ushort)128];
        yield return [nameof(DataRecordExtensions.GetInt32), 256];
        yield return [nameof(DataRecordExtensions.GetUInt32), (uint)512];
        yield return [nameof(DataRecordExtensions.GetInt64), (long)1024];
        yield return [nameof(DataRecordExtensions.GetUInt64), (ulong)2048];
        yield return [nameof(DataRecordExtensions.GetSingle), 3.14f];
        yield return [nameof(DataRecordExtensions.GetDouble), 9.10d];
        yield return [nameof(DataRecordExtensions.GetDecimal), 100m];
        yield return [nameof(DataRecordExtensions.GetDateTime), DateTime.Now];
        yield return [nameof(DataRecordExtensions.GetDateTimeOffset), DateTimeOffset.Now];
        yield return [nameof(DataRecordExtensions.GetTimeSpan), TimeSpan.FromSeconds(15)];
        yield return [nameof(DataRecordExtensions.GetGuid), Guid.NewGuid()];
        yield return [nameof(DataRecordExtensions.GetEnum), DayOfWeek.Wednesday];
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
