namespace Nameless.Data.Fixtures;

public class DataRecordInlineData : TheoryData<string, object> {
    public DataRecordInlineData() {
        Add(nameof(DataRecordExtensions.GetString), "Value");
        Add(nameof(DataRecordExtensions.GetBoolean), true);
        Add(nameof(DataRecordExtensions.GetChar), 'X');
        Add(nameof(DataRecordExtensions.GetSByte), (sbyte)16);
        Add(nameof(DataRecordExtensions.GetByte), (byte)32);
        Add(nameof(DataRecordExtensions.GetInt16), (short)64);
        Add(nameof(DataRecordExtensions.GetUInt16), (ushort)128);
        Add(nameof(DataRecordExtensions.GetInt32), 256);
        Add(nameof(DataRecordExtensions.GetUInt32), (uint)512);
        Add(nameof(DataRecordExtensions.GetInt64), (long)1024);
        Add(nameof(DataRecordExtensions.GetUInt64), (ulong)2048);
        Add(nameof(DataRecordExtensions.GetSingle), 3.14f);
        Add(nameof(DataRecordExtensions.GetDouble), 9.10d);
        Add(nameof(DataRecordExtensions.GetDecimal), 100m);
        Add(nameof(DataRecordExtensions.GetDateTime), DateTime.Now);
        Add(nameof(DataRecordExtensions.GetDateTimeOffset), DateTimeOffset.Now);
        Add(nameof(DataRecordExtensions.GetTimeSpan), TimeSpan.FromSeconds(15));
        Add(nameof(DataRecordExtensions.GetGuid), Guid.NewGuid());
        Add(nameof(DataRecordExtensions.GetEnum), DayOfWeek.Wednesday);
    }
}
