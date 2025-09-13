using Nameless.Data.Mockers;

namespace Nameless.Data;

public class DataRecordExtensionsTests {
    [Fact]
    public void WhenGetString_ThenReturnsStringValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetString);
        const string Value = "This is a test";
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var actual = dataRecord.GetString(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetString_WithFallback_ThenReturnsFallbackStringValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetString);
        const string Value = "This is a test";
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetString(Key, Value);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetBoolean_ThenReturnsBooleanValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetBoolean);
        const bool Value = true;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var actual = dataRecord.GetBoolean(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetBoolean_WithFallback_ThenReturnsFallbackBooleanValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetBoolean);
        const bool Value = true;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetBoolean(Key, Value);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetChar_ThenReturnsCharValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetChar);
        const char Value = 'A';
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var actual = dataRecord.GetChar(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetChar_WithFallback_ThenReturnsFallbackCharValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetChar);
        const char Value = 'Z';
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetChar(Key, Value);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetSByte_ThenReturnsSByteValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetSByte);
        const sbyte Value = 123;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var actual = dataRecord.GetSByte(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetSByte_WithFallback_ThenReturnsFallbackSByteValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetSByte);
        const sbyte Value = -64;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetSByte(Key, Value);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetByte_ThenReturnsByteValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetByte);
        const byte Value = 200;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var actual = dataRecord.GetByte(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetByte_WithFallback_ThenReturnsFallbackByteValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetByte);
        const byte Value = 50;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetByte(Key, Value);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetInt16_ThenReturnsInt16Value() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetInt16);
        const short Value = 1024;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var actual = dataRecord.GetInt16(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetInt16_WithFallback_ThenReturnsFallbackInt16Value() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetInt16);
        const short Value = -4096;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetInt16(Key, Value);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetUInt16_ThenReturnsUInt16Value() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetUInt32);
        const ushort Value = 512;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var actual = dataRecord.GetUInt16(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetUInt16_WithFallback_ThenReturnsFallbackUInt16Value() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetUInt16);
        const ushort Value = 2048;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetUInt16(Key, Value);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetInt32_ThenReturnsInt32Value() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetInt32);
        const int Value = 500_000;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var actual = dataRecord.GetInt32(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetInt32_WithFallback_ThenReturnsFallbackInt32Value() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetInt32);
        const int Value = -1_000_000;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetInt32(Key, Value);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetUInt32_ThenReturnsUInt32Value() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetUInt32);
        const uint Value = 8;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var actual = dataRecord.GetUInt32(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetUInt32_WithFallback_ThenReturnsFallbackUInt32Value() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetUInt32);
        const uint Value = 16;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetUInt32(Key, Value);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetInt64_ThenReturnsInt64Value() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetInt64);
        const long Value = 1000;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var actual = dataRecord.GetInt64(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetInt64_WithFallback_ThenReturnsFallbackInt64Value() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetInt64);
        const long Value = -2000;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetInt64(Key, Value);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetUInt64_ThenReturnsUInt64Value() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetUInt64);
        const ulong Value = 6000;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var actual = dataRecord.GetUInt64(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetUInt64_WithFallback_ThenReturnsFallbackUInt64Value() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetUInt64);
        const ulong Value = 8080;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetUInt64(Key, Value);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetSingle_ThenReturnsSingleValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetSingle);
        const float Value = 3.14F;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var actual = dataRecord.GetSingle(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetSingle_WithFallback_ThenReturnsFallbackSingleValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetSingle);
        const float Value = -0.852F;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetSingle(Key, Value);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetDouble_ThenReturnsDoubleValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetDouble);
        const double Value = double.MaxValue;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var actual = dataRecord.GetDouble(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetDouble_WithFallback_ThenReturnsFallbackDoubleValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetDouble);
        const double Value = double.MinValue;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetDouble(Key, Value);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetDecimal_ThenReturnsDecimalValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetDecimal);
        const decimal Value = 5.55M;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var actual = dataRecord.GetDecimal(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetDecimal_WithFallback_ThenReturnsFallbackDecimalValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetDecimal);
        const decimal Value = -9.87M;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetDecimal(Key, Value);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetDateTime_ThenReturnsDateTimeValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetDateTime);
        var value = new DateTime(new DateOnly(year: 2000, month: 1, day: 1), TimeOnly.MinValue, DateTimeKind.Utc);
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, value)
                         .Build();

        // act
        var actual = dataRecord.GetDateTime(Key);

        // assert
        Assert.Equal(value, actual);
    }

    [Fact]
    public void WhenGetDateTime_WithFallback_ThenReturnsFallbackDateTimeValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetDateTime);
        var value = DateTime.Now;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetDateTime(Key, value);

        // assert
        Assert.Equal(value, actual);
    }

    [Fact]
    public void WhenGetDateTimeOffset_ThenReturnsDateTimeOffsetValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetDateTimeOffset);
        var value = new DateTimeOffset(new DateOnly(year: 2000, month: 1, day: 1), TimeOnly.MinValue, TimeSpan.Zero);
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, value)
                         .Build();

        // act
        var actual = dataRecord.GetDateTimeOffset(Key);

        // assert
        Assert.Equal(value, actual);
    }

    [Fact]
    public void WhenGetDateTimeOffset_WithFallback_ThenReturnsFallbackDateTimeOffsetValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetDateTimeOffset);
        var value = DateTimeOffset.Now.AddDays(days: -7);
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetDateTimeOffset(Key, value);

        // assert
        Assert.Equal(value, actual);
    }

    [Fact]
    public void WhenGetTimeSpan_ThenReturnsTimeSpanValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetTimeSpan);
        var value = TimeSpan.FromDays(days: 1);
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, value)
                         .Build();

        // act
        var actual = dataRecord.GetTimeSpan(Key);

        // assert
        Assert.Equal(value, actual);
    }

    [Fact]
    public void WhenGetTimeSpan_WithFallback_ThenReturnsFallbackTimeSpanValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetTimeSpan);
        var value = TimeSpan.FromDays(days: -5);
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetTimeSpan(Key, value);

        // assert
        Assert.Equal(value, actual);
    }

    [Fact]
    public void WhenGetGuid_ThenReturnsGuidValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetGuid);
        var value = Guid.Parse(input: "cdd83fa1-2332-4dfd-a008-6f2008b0e7af");
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, value)
                         .Build();

        // act
        var actual = dataRecord.GetGuid(Key);

        // assert
        Assert.Equal(value, actual);
    }

    [Fact]
    public void WhenGetGuid_WithFallback_ThenReturnsFallbackGuidValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetGuid);
        var value = Guid.Parse(input: "e2907e77-122b-4f3f-96e5-df1e5d5e6544");
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetGuid(Key, value);

        // assert
        Assert.Equal(value, actual);
    }

    [Fact]
    public void WhenGetEnum_ThenReturnsEnumValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetEnum);
        const DayOfWeek Value = DayOfWeek.Friday;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var actual = dataRecord.GetEnum<DayOfWeek>(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetEnum_WithFallback_ThenReturnsFallbackEnumValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetEnum);
        const DayOfWeek Value = DayOfWeek.Sunday;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetEnum(Key, Value);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetEnum_WhenDataRecordValueIsEnumString_ThenReturnsEnumValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetEnum);
        const DayOfWeek Value = DayOfWeek.Friday;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value.ToString())
                         .Build();

        // act
        var actual = dataRecord.GetEnum<DayOfWeek>(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetEnum_WhenDataRecordValueIsEnumInteger_ThenReturnsEnumValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetEnum);
        const DayOfWeek Value = DayOfWeek.Wednesday;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, (int)Value)
                         .Build();

        // act
        var actual = dataRecord.GetEnum<DayOfWeek>(Key);

        // assert
        Assert.Equal(Value, actual);
    }

    [Fact]
    public void WhenGetBlob_ThenReturnsBlobValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetBlob);
        var value = "This Is A Test"u8.ToArray();
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, value)
                         .Build();

        // act
        var actual = dataRecord.GetBlob(Key);

        // assert
        Assert.Equal(value, actual);
    }

    [Fact]
    public void WhenGetBlob_WithFallback_ThenReturnsFallbackBlobValue() {
        // arrange
        const string Key = nameof(DataRecordExtensions.GetBlob);
        var value = "This Is Also A Test"u8.ToArray();
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetBlob(Key, value);

        // assert
        Assert.Equal(value, actual);
    }

    [Fact]
    public void WhenGetValue_ThenColumnNotExists_ThenReturnsDefaultValue() {
        // arrange
        const string ColumnName = nameof(DataRecordExtensions.GetString);
        const string WrongColumnName = nameof(DataRecordExtensions.GetInt32);
        const string Expected = ""; // DataRecordExtensions string default
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(ColumnName, DBNull.Value)
                         .Build();

        // act
        var actual = dataRecord.GetString(WrongColumnName);

        // assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void WhenTryGet_WhenGenericArgumentDoNotReflectsObjectType_ThenReturnsFalse_ThenOutputIsDefault() {
        // arrange
        const string Key = nameof(Key);
        const int Value = 123;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var result = dataRecord.TryGet<DateTime>(Key, out var actual);

        // assert
        Assert.Multiple(() => {
            Assert.False(result);
            Assert.Equal(expected: default, actual);
        });
    }

    [Fact]
    public void WhenTryGet_WhenGenericArgumentIsNullable_ThenReturnsFalse_ThenOutputIsNull() {
        // arrange
        const string Key = nameof(Key);
        const int Value = 123;
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, Value)
                         .Build();

        // act
        var result = dataRecord.TryGet<DateTime?>(Key, out var actual);

        // assert
        Assert.Multiple(() => {
            Assert.False(result);
            Assert.Null(actual);
        });
    }

    [Fact]
    public void WhenTryGet_WhenRecordValueIsString_WhenTypedArgumentIsGuid_ThenReturnsGuid() {
        // arrange
        const string Key = nameof(Key);
        var expected = Guid.NewGuid();
        var value = expected.ToString();
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, value)
                         .Build();

        // act
        var result = dataRecord.TryGet<Guid>(Key, out var actual);

        // assert
        Assert.Multiple(() => {
            Assert.True(result);
            Assert.Equal(expected, actual);
        });
    }

    [Fact]
    public void WhenTryGet_WhenRecordValueIsString_WhenTypedArgumentIsDateTimeOffset_ThenReturnsGuid() {
        // arrange
        const string Key = nameof(Key);
        var expected = DateTimeOffset.UtcNow;
        var value = expected.ToString(format: "O");
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, value)
                         .Build();

        // act
        var result = dataRecord.TryGet<DateTimeOffset>(Key, out var actual);

        // assert
        Assert.Multiple(() => {
            Assert.True(result);
            Assert.Equal(expected, actual);
        });
    }

    [Fact]
    public void WhenTryGet_WhenRecordValueIsString_WhenTypedArgumentIsTimeSpan_ThenReturnsGuid() {
        // arrange
        const string Key = nameof(Key);
        var expected = TimeSpan.FromMilliseconds(milliseconds: 654123);
        var value = expected.ToString();
        var dataRecord = new DataRecordMocker()
                         .WithIndexer(Key, value)
                         .Build();

        // act
        var result = dataRecord.TryGet<TimeSpan>(Key, out var actual);

        // assert
        Assert.Multiple(() => {
            Assert.True(result);
            Assert.Equal(expected, actual);
        });
    }
}