using System.Data;
using Nameless.Data;

namespace Nameless;

public class DataRecordExtensionsTests {
    // --- GetString ---

    [Fact]
    public void GetString_WhenColumnExists_ReturnsValue() {
        // arrange
        var record = new FakeDataRecord(new Dictionary<string, object?> {
            ["Name"] = "hello"
        });

        // act
        var result = record.GetString("Name");

        // assert
        Assert.Equal("hello", result);
    }

    [Fact]
    public void GetString_WhenColumnMissing_ReturnsFallback() {
        // arrange
        var record = new FakeDataRecord(new Dictionary<string, object?>());

        // act
        var result = record.GetString("Missing", fallback: "default");

        // assert
        Assert.Equal("default", result);
    }

    // --- GetBoolean ---

    [Fact]
    public void GetBoolean_WhenColumnExists_ReturnsValue() {
        // arrange
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["Flag"] = true });

        // act
        var result = record.GetBoolean("Flag");

        // assert
        Assert.True(result);
    }

    [Fact]
    public void GetBoolean_WhenColumnMissing_ReturnsFallback() {
        // arrange
        var record = new FakeDataRecord(new Dictionary<string, object?>());

        // act
        var result = record.GetBoolean("Missing", fallback: true);

        // assert
        Assert.True(result);
    }

    // --- GetInt32 ---

    [Fact]
    public void GetInt32_WhenColumnExists_ReturnsValue() {
        // arrange
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["Age"] = 42 });

        // act
        var result = record.GetInt32("Age");

        // assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void GetInt32_WhenColumnMissing_ReturnsFallback() {
        // arrange
        var record = new FakeDataRecord(new Dictionary<string, object?>());

        // act
        var result = record.GetInt32("Missing", fallback: 99);

        // assert
        Assert.Equal(99, result);
    }

    // --- GetGuid ---

    [Fact]
    public void GetGuid_WhenColumnHasGuid_ReturnsGuid() {
        // arrange
        var id = Guid.NewGuid();
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["Id"] = id });

        // act
        var result = record.GetGuid("Id");

        // assert
        Assert.Equal(id, result);
    }

    [Fact]
    public void GetGuid_WhenColumnHasStringGuid_ReturnsGuid() {
        // arrange
        var id = Guid.NewGuid();
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["Id"] = id.ToString() });

        // act
        var result = record.GetGuid("Id");

        // assert
        Assert.Equal(id, result);
    }

    // --- GetBlob ---

    [Fact]
    public void GetBlob_WhenColumnExists_ReturnsByteArray() {
        // arrange
        var bytes = new byte[] { 1, 2, 3 };
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["Data"] = bytes });

        // act
        var result = record.GetBlob("Data");

        // assert
        Assert.Equal(bytes, result);
    }

    [Fact]
    public void GetBlob_WhenColumnMissing_ReturnsEmpty() {
        // arrange
        var record = new FakeDataRecord(new Dictionary<string, object?>());

        // act
        var result = record.GetBlob("Missing");

        // assert
        Assert.Empty(result);
    }

    // --- TryGet ---

    [Fact]
    public void TryGet_WhenValueIsDBNull_ReturnsFalse() {
        // arrange
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["Col"] = DBNull.Value });

        // act
        var found = record.TryGet<string>("Col", out var result);

        // assert
        Assert.Multiple(() => {
            Assert.False(found);
            Assert.Null(result);
        });
    }

    [Fact]
    public void TryGet_WhenColumnThrows_ReturnsFalse() {
        // arrange
        var record = new FakeDataRecord(new Dictionary<string, object?>());

        // act
        var found = record.TryGet<string>("NonExistent", out var result);

        // assert
        Assert.False(found);
        Assert.Null(result);
    }

    // --- GetChar ---

    [Fact]
    public void GetChar_WhenColumnExists_ReturnsValue() {
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["C"] = 'A' });
        Assert.Equal('A', record.GetChar("C"));
    }

    [Fact]
    public void GetChar_WhenColumnMissing_ReturnsFallback() {
        var record = new FakeDataRecord(new Dictionary<string, object?>());
        Assert.Equal('X', record.GetChar("Missing", fallback: 'X'));
    }

    // --- GetByte ---

    [Fact]
    public void GetByte_WhenColumnExists_ReturnsValue() {
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["B"] = (byte)255 });
        Assert.Equal((byte)255, record.GetByte("B"));
    }

    [Fact]
    public void GetByte_WhenColumnMissing_ReturnsFallback() {
        var record = new FakeDataRecord(new Dictionary<string, object?>());
        Assert.Equal((byte)42, record.GetByte("Missing", fallback: 42));
    }

    // --- GetInt16 ---

    [Fact]
    public void GetInt16_WhenColumnExists_ReturnsValue() {
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["S"] = (short)1000 });
        Assert.Equal((short)1000, record.GetInt16("S"));
    }

    // --- GetInt64 ---

    [Fact]
    public void GetInt64_WhenColumnExists_ReturnsValue() {
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["L"] = 9999999999L });
        Assert.Equal(9999999999L, record.GetInt64("L"));
    }

    [Fact]
    public void GetInt64_WhenColumnMissing_ReturnsFallback() {
        var record = new FakeDataRecord(new Dictionary<string, object?>());
        Assert.Equal(100L, record.GetInt64("Missing", fallback: 100L));
    }

    // --- GetSingle ---

    [Fact]
    public void GetSingle_WhenColumnExists_ReturnsValue() {
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["F"] = 3.14f });
        Assert.Equal(3.14f, record.GetSingle("F"), precision: 2);
    }

    [Fact]
    public void GetSingle_WhenColumnMissing_ReturnsFallback() {
        var record = new FakeDataRecord(new Dictionary<string, object?>());
        Assert.Equal(1.5f, record.GetSingle("Missing", fallback: 1.5f));
    }

    // --- GetDouble ---

    [Fact]
    public void GetDouble_WhenColumnExists_ReturnsValue() {
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["D"] = 3.14 });
        Assert.Equal(3.14, record.GetDouble("D"), precision: 2);
    }

    [Fact]
    public void GetDouble_WhenColumnMissing_ReturnsFallback() {
        var record = new FakeDataRecord(new Dictionary<string, object?>());
        Assert.Equal(2.5, record.GetDouble("Missing", fallback: 2.5));
    }

    // --- GetDecimal ---

    [Fact]
    public void GetDecimal_WhenColumnExists_ReturnsValue() {
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["M"] = 99.99m });
        Assert.Equal(99.99m, record.GetDecimal("M"));
    }

    [Fact]
    public void GetDecimal_WhenColumnMissing_ReturnsFallback() {
        var record = new FakeDataRecord(new Dictionary<string, object?>());
        Assert.Equal(10m, record.GetDecimal("Missing", fallback: 10m));
    }

    // --- GetDateTime ---

    [Fact]
    public void GetDateTime_WhenColumnExists_ReturnsValue() {
        var dt = new DateTime(2024, 1, 1);
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["Date"] = dt });
        Assert.Equal(dt, record.GetDateTime("Date"));
    }

    [Fact]
    public void GetDateTime_WhenColumnMissing_ReturnsFallback() {
        var fallback = new DateTime(2020, 1, 1);
        var record = new FakeDataRecord(new Dictionary<string, object?>());
        Assert.Equal(fallback, record.GetDateTime("Missing", fallback));
    }

    // --- GetTimeSpan ---

    [Fact]
    public void GetTimeSpan_WhenColumnHasStringValue_ReturnsTimeSpan() {
        var ts = TimeSpan.FromHours(2);
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["T"] = ts.ToString() });
        Assert.Equal(ts, record.GetTimeSpan("T"));
    }

    [Fact]
    public void GetTimeSpan_WhenColumnMissing_ReturnsFallback() {
        var fallback = TimeSpan.FromMinutes(30);
        var record = new FakeDataRecord(new Dictionary<string, object?>());
        Assert.Equal(fallback, record.GetTimeSpan("Missing", fallback));
    }

    // --- GetEnum ---

    [Fact]
    public void GetEnum_WhenColumnHasStringValue_ReturnsEnum() {
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["E"] = "Tuesday" });
        Assert.Equal(DayOfWeek.Tuesday, record.GetEnum<DayOfWeek>("E"));
    }

    [Fact]
    public void GetEnum_WhenColumnMissing_ReturnsFallback() {
        var record = new FakeDataRecord(new Dictionary<string, object?>());
        Assert.Equal(DayOfWeek.Monday, record.GetEnum<DayOfWeek>("Missing", DayOfWeek.Monday));
    }

    // --- GetBlob with fallback ---

    [Fact]
    public void GetBlob_WithFallback_WhenColumnMissing_ReturnsFallback() {
        var fallback = new byte[] { 9, 8, 7 };
        var record = new FakeDataRecord(new Dictionary<string, object?>());
        Assert.Equal(fallback, record.GetBlob("Missing", fallback));
    }

    // --- GetSByte ---

    [Fact]
    public void GetSByte_WhenColumnExists_ReturnsValue() {
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["V"] = (sbyte)-5 });
        Assert.Equal((sbyte)-5, record.GetSByte("V"));
    }

    [Fact]
    public void GetSByte_WhenColumnMissing_ReturnsFallback() {
        var record = new FakeDataRecord(new Dictionary<string, object?>());
        Assert.Equal((sbyte)10, record.GetSByte("Missing", fallback: 10));
    }

    // --- GetUInt16 ---

    [Fact]
    public void GetUInt16_WhenColumnExists_ReturnsValue() {
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["V"] = (ushort)500 });
        Assert.Equal((ushort)500, record.GetUInt16("V"));
    }

    [Fact]
    public void GetUInt16_WhenColumnMissing_ReturnsFallback() {
        var record = new FakeDataRecord(new Dictionary<string, object?>());
        Assert.Equal((ushort)7, record.GetUInt16("Missing", fallback: 7));
    }

    // --- GetUInt32 ---

    [Fact]
    public void GetUInt32_WhenColumnExists_ReturnsValue() {
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["V"] = 3000U });
        Assert.Equal(3000U, record.GetUInt32("V"));
    }

    [Fact]
    public void GetUInt32_WhenColumnMissing_ReturnsFallback() {
        var record = new FakeDataRecord(new Dictionary<string, object?>());
        Assert.Equal(99U, record.GetUInt32("Missing", fallback: 99U));
    }

    // --- GetUInt64 ---

    [Fact]
    public void GetUInt64_WhenColumnExists_ReturnsValue() {
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["V"] = 999999UL });
        Assert.Equal(999999UL, record.GetUInt64("V"));
    }

    [Fact]
    public void GetUInt64_WhenColumnMissing_ReturnsFallback() {
        var record = new FakeDataRecord(new Dictionary<string, object?>());
        Assert.Equal(1UL, record.GetUInt64("Missing", fallback: 1UL));
    }

    // --- GetDateTimeOffset ---

    [Fact]
    public void GetDateTimeOffset_WhenColumnHasDirectValue_ReturnsValue() {
        var dto = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["DTO"] = dto });
        Assert.Equal(dto, record.GetDateTimeOffset("DTO"));
    }

    [Fact]
    public void GetDateTimeOffset_WhenColumnHasStringValue_ReturnsValue() {
        var dto = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["DTO"] = dto.ToString("O") });
        Assert.Equal(dto, record.GetDateTimeOffset("DTO"));
    }

    [Fact]
    public void GetDateTimeOffset_WhenColumnMissing_ReturnsFallback() {
        var fallback = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var record = new FakeDataRecord(new Dictionary<string, object?>());
        Assert.Equal(fallback, record.GetDateTimeOffset("Missing", fallback));
    }

    // --- GetTimeSpan (direct value) ---

    [Fact]
    public void GetTimeSpan_WhenColumnHasDirectTimeSpan_ReturnsValue() {
        var ts = TimeSpan.FromSeconds(90);
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["T"] = ts });
        Assert.Equal(ts, record.GetTimeSpan("T"));
    }

    // --- Transform _ => null path ---

    [Fact]
    public void TryGet_WhenTypeIsUnsupported_ReturnsFalse() {
        // Uri has TypeCode.Object which falls through to the _ => null branch in Transform
        var record = new FakeDataRecord(new Dictionary<string, object?> { ["V"] = "http://example.com" });
        var found = record.TryGet<Uri>("V", out var result);
        Assert.False(found);
    }

    // --- test doubles ---

    private sealed class FakeDataRecord(Dictionary<string, object?> data) : IDataRecord {
        public object this[int i] => data.Values.ElementAt(i) ?? DBNull.Value;
        public object this[string name] => data.TryGetValue(name, out var v) ? v ?? DBNull.Value : throw new IndexOutOfRangeException(name);

        public int FieldCount => data.Count;
        public bool GetBoolean(int i) => (bool)data.Values.ElementAt(i)!;
        public byte GetByte(int i) => (byte)data.Values.ElementAt(i)!;
        public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferOffset, int length) => 0;
        public char GetChar(int i) => (char)data.Values.ElementAt(i)!;
        public long GetChars(int i, long fieldOffset, char[]? buffer, int bufferOffset, int length) => 0;
        public IDataReader GetData(int i) => null!;
        public string GetDataTypeName(int i) => string.Empty;
        public DateTime GetDateTime(int i) => (DateTime)data.Values.ElementAt(i)!;
        public decimal GetDecimal(int i) => (decimal)data.Values.ElementAt(i)!;
        public double GetDouble(int i) => (double)data.Values.ElementAt(i)!;
        public Type GetFieldType(int i) => data.Values.ElementAt(i)?.GetType() ?? typeof(object);
        public float GetFloat(int i) => (float)data.Values.ElementAt(i)!;
        public Guid GetGuid(int i) => (Guid)data.Values.ElementAt(i)!;
        public short GetInt16(int i) => (short)data.Values.ElementAt(i)!;
        public int GetInt32(int i) => (int)data.Values.ElementAt(i)!;
        public long GetInt64(int i) => (long)data.Values.ElementAt(i)!;
        public string GetName(int i) => data.Keys.ElementAt(i);
        public int GetOrdinal(string name) => data.Keys.ToList().IndexOf(name);
        public string GetString(int i) => (string)data.Values.ElementAt(i)!;
        public object GetValue(int i) => data.Values.ElementAt(i) ?? DBNull.Value;
        public int GetValues(object[] values) => 0;
        public bool IsDBNull(int i) => data.Values.ElementAt(i) is null or DBNull;
    }
}
