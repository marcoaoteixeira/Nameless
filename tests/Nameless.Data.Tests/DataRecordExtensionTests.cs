using System.Data;
using System.Globalization;
using System.Text;
using Moq;

namespace Nameless.Data;

public class DataRecordExtensionsTests {
    [Fact]
    public void GetSomething_Should_Return_Value_From_DataRecord_Happy_Path() {
        // arrange
        var extensions = new Dictionary<string, object> {
            { nameof(DataRecordExtensions.GetString), "Value" },
            { nameof(DataRecordExtensions.GetBoolean), true },
            { nameof(DataRecordExtensions.GetChar), 'X' },
            { nameof(DataRecordExtensions.GetSByte), (sbyte)16 },
            { nameof(DataRecordExtensions.GetByte), (byte)32 },
            { nameof(DataRecordExtensions.GetInt16), (short)64 },
            { nameof(DataRecordExtensions.GetUInt16), (ushort)128 },
            { nameof(DataRecordExtensions.GetInt32), 256 },
            { nameof(DataRecordExtensions.GetUInt32), (uint)512 },
            { nameof(DataRecordExtensions.GetInt64), (long)1024 },
            { nameof(DataRecordExtensions.GetUInt64), (ulong)2048 },
            { nameof(DataRecordExtensions.GetSingle), 3.14f },
            { nameof(DataRecordExtensions.GetDouble), 9.10d },
            { nameof(DataRecordExtensions.GetDecimal), 100m },
            { nameof(DataRecordExtensions.GetDateTime), DateTime.Now },
            { nameof(DataRecordExtensions.GetDateTimeOffset), DateTimeOffset.Now },
            { nameof(DataRecordExtensions.GetTimeSpan), TimeSpan.FromSeconds(15) },
            { nameof(DataRecordExtensions.GetGuid), Guid.NewGuid() },
            { nameof(DataRecordExtensions.GetEnum), DayOfWeek.Wednesday }
        };
        var dataRecordMock = new Mock<IDataRecord>();
        foreach (var extension in extensions) {
            dataRecordMock
               .Setup(mock => mock[extension.Key])
               .Returns(extension.Value);
        }

        // act
        var actualString = dataRecordMock.Object.GetString(nameof(DataRecordExtensions.GetString));
        var actualBoolean = dataRecordMock.Object.GetBoolean(nameof(DataRecordExtensions.GetBoolean));
        var actualChar = dataRecordMock.Object.GetChar(nameof(DataRecordExtensions.GetChar));
        var actualSByte = dataRecordMock.Object.GetSByte(nameof(DataRecordExtensions.GetSByte));
        var actualByte = dataRecordMock.Object.GetByte(nameof(DataRecordExtensions.GetByte));
        var actualInt16 = dataRecordMock.Object.GetInt16(nameof(DataRecordExtensions.GetInt16));
        var actualUInt16 = dataRecordMock.Object.GetUInt16(nameof(DataRecordExtensions.GetUInt16));
        var actualInt32 = dataRecordMock.Object.GetInt32(nameof(DataRecordExtensions.GetInt32));
        var actualUInt32 = dataRecordMock.Object.GetUInt32(nameof(DataRecordExtensions.GetUInt32));
        var actualInt64 = dataRecordMock.Object.GetInt64(nameof(DataRecordExtensions.GetInt64));
        var actualUInt64 = dataRecordMock.Object.GetUInt64(nameof(DataRecordExtensions.GetUInt64));
        var actualSingle = dataRecordMock.Object.GetSingle(nameof(DataRecordExtensions.GetSingle));
        var actualDouble = dataRecordMock.Object.GetDouble(nameof(DataRecordExtensions.GetDouble));
        var actualDecimal = dataRecordMock.Object.GetDecimal(nameof(DataRecordExtensions.GetDecimal));
        var actualDateTime = dataRecordMock.Object.GetDateTime(nameof(DataRecordExtensions.GetDateTime));
        var actualDateTimeOffset =
            dataRecordMock.Object.GetDateTimeOffset(nameof(DataRecordExtensions.GetDateTimeOffset));
        var actualTimeSpan = dataRecordMock.Object.GetTimeSpan(nameof(DataRecordExtensions.GetTimeSpan));
        var actualGuid = dataRecordMock.Object.GetGuid(nameof(DataRecordExtensions.GetGuid));
        var actualEnum = dataRecordMock.Object.GetEnum<DayOfWeek>(nameof(DataRecordExtensions.GetEnum));

        // assert
        Assert.Multiple(() => {
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetString)], actualString);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetBoolean)], actualBoolean);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetChar)], actualChar);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetSByte)], actualSByte);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetByte)], actualByte);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetInt16)], actualInt16);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetUInt16)], actualUInt16);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetInt32)], actualInt32);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetUInt32)], actualUInt32);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetInt64)], actualInt64);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetUInt64)], actualUInt64);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetSingle)], actualSingle);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetDouble)], actualDouble);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetDecimal)], actualDecimal);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetDateTime)], actualDateTime);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetDateTimeOffset)], actualDateTimeOffset);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetTimeSpan)], actualTimeSpan);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetGuid)], actualGuid);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetEnum)], actualEnum);
        });
    }

    [Fact]
    public void GetSomething_Should_Return_Value_From_DataRecord() {
        // arrange
        var culture = new CultureInfo("en-US");
        var guid = Guid.NewGuid();
        var now = DateTime.Now.Date;

        var happyValues = new Dictionary<string, object> {
            { nameof(DataRecordExtensions.GetString), "Value" },
            { nameof(DataRecordExtensions.GetBoolean), true },
            { nameof(DataRecordExtensions.GetChar), 'X' },
            { nameof(DataRecordExtensions.GetSByte), (sbyte)16 },
            { nameof(DataRecordExtensions.GetByte), (byte)32 },
            { nameof(DataRecordExtensions.GetInt16), (short)64 },
            { nameof(DataRecordExtensions.GetUInt16), (ushort)128 },
            { nameof(DataRecordExtensions.GetInt32), 256 },
            { nameof(DataRecordExtensions.GetUInt32), (uint)512 },
            { nameof(DataRecordExtensions.GetInt64), (long)1024 },
            { nameof(DataRecordExtensions.GetUInt64), (ulong)2048 },
            { nameof(DataRecordExtensions.GetSingle), 3.14f },
            { nameof(DataRecordExtensions.GetDouble), 9.10d },
            { nameof(DataRecordExtensions.GetDecimal), 100m },
            { nameof(DataRecordExtensions.GetDateTime), now },
            { nameof(DataRecordExtensions.GetDateTimeOffset), new DateTimeOffset(now) },
            { nameof(DataRecordExtensions.GetTimeSpan), TimeSpan.FromSeconds(15) },
            { nameof(DataRecordExtensions.GetGuid), guid },
            { nameof(DataRecordExtensions.GetEnum), DayOfWeek.Wednesday }
        };

        var extensions = new Dictionary<string, object> {
            { nameof(DataRecordExtensions.GetString), "Value" },
            { nameof(DataRecordExtensions.GetBoolean), "true" },
            { nameof(DataRecordExtensions.GetChar), "X" },
            { nameof(DataRecordExtensions.GetSByte), "16" },
            { nameof(DataRecordExtensions.GetByte), "32" },
            { nameof(DataRecordExtensions.GetInt16), "64" },
            { nameof(DataRecordExtensions.GetUInt16), "128" },
            { nameof(DataRecordExtensions.GetInt32), "256" },
            { nameof(DataRecordExtensions.GetUInt32), "512" },
            { nameof(DataRecordExtensions.GetInt64), "1024" },
            { nameof(DataRecordExtensions.GetUInt64), "2048" },
            { nameof(DataRecordExtensions.GetSingle), "3.14" },
            { nameof(DataRecordExtensions.GetDouble), "9.10" },
            { nameof(DataRecordExtensions.GetDecimal), "100" },
            { nameof(DataRecordExtensions.GetDateTime), now.ToString(culture) },
            { nameof(DataRecordExtensions.GetDateTimeOffset), new DateTimeOffset(now).ToString(culture) },
            { nameof(DataRecordExtensions.GetTimeSpan), TimeSpan.FromSeconds(15).ToString() },
            { nameof(DataRecordExtensions.GetGuid), guid.ToString() },
            { nameof(DataRecordExtensions.GetEnum) + "string", nameof(DayOfWeek.Wednesday) },
            { nameof(DataRecordExtensions.GetEnum) + "int", (int)DayOfWeek.Wednesday }
        };
        var dataRecordMock = new Mock<IDataRecord>();
        foreach (var extension in extensions) {
            dataRecordMock
               .Setup(mock => mock[extension.Key])
               .Returns(extension.Value);
        }

        // act
        var actualString = dataRecordMock.Object.GetString(nameof(DataRecordExtensions.GetString), culture);
        var actualBoolean =
            dataRecordMock.Object.GetBoolean(nameof(DataRecordExtensions.GetBoolean), formatProvider: culture);
        var actualChar = dataRecordMock.Object.GetChar(nameof(DataRecordExtensions.GetChar), formatProvider: culture);
        var actualSByte = dataRecordMock.Object.GetSByte(nameof(DataRecordExtensions.GetSByte), formatProvider: culture);
        var actualByte = dataRecordMock.Object.GetByte(nameof(DataRecordExtensions.GetByte), formatProvider: culture);
        var actualInt16 = dataRecordMock.Object.GetInt16(nameof(DataRecordExtensions.GetInt16), formatProvider: culture);
        var actualUInt16 =
            dataRecordMock.Object.GetUInt16(nameof(DataRecordExtensions.GetUInt16), formatProvider: culture);
        var actualInt32 = dataRecordMock.Object.GetInt32(nameof(DataRecordExtensions.GetInt32), formatProvider: culture);
        var actualUInt32 =
            dataRecordMock.Object.GetUInt32(nameof(DataRecordExtensions.GetUInt32), formatProvider: culture);
        var actualInt64 = dataRecordMock.Object.GetInt64(nameof(DataRecordExtensions.GetInt64), formatProvider: culture);
        var actualUInt64 =
            dataRecordMock.Object.GetUInt64(nameof(DataRecordExtensions.GetUInt64), formatProvider: culture);
        var actualSingle =
            dataRecordMock.Object.GetSingle(nameof(DataRecordExtensions.GetSingle), formatProvider: culture);
        var actualDouble =
            dataRecordMock.Object.GetDouble(nameof(DataRecordExtensions.GetDouble), formatProvider: culture);
        var actualDecimal =
            dataRecordMock.Object.GetDecimal(nameof(DataRecordExtensions.GetDecimal), formatProvider: culture);
        var actualDateTime =
            dataRecordMock.Object.GetDateTime(nameof(DataRecordExtensions.GetDateTime), formatProvider: culture);
        var actualDateTimeOffset =
            dataRecordMock.Object.GetDateTimeOffset(nameof(DataRecordExtensions.GetDateTimeOffset),
                formatProvider: culture);
        var actualTimeSpan =
            dataRecordMock.Object.GetTimeSpan(nameof(DataRecordExtensions.GetTimeSpan), formatProvider: culture);
        var actualGuid = dataRecordMock.Object.GetGuid(nameof(DataRecordExtensions.GetGuid));
        var actualEnumString = dataRecordMock.Object.GetEnum<DayOfWeek>(nameof(DataRecordExtensions.GetEnum) + "string");
        var actualEnumInt = dataRecordMock.Object.GetEnum<DayOfWeek>(nameof(DataRecordExtensions.GetEnum) + "int");

        // assert
        Assert.Multiple(() => {
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetString)], actualString);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetBoolean)], actualBoolean);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetChar)], actualChar);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetSByte)], actualSByte);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetByte)], actualByte);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetInt16)], actualInt16);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetUInt16)], actualUInt16);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetInt32)], actualInt32);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetUInt32)], actualUInt32);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetInt64)], actualInt64);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetUInt64)], actualUInt64);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetSingle)], actualSingle);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetDouble)], actualDouble);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetDecimal)], actualDecimal);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetDateTime)], actualDateTime);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetDateTimeOffset)], actualDateTimeOffset);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetTimeSpan)], actualTimeSpan);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetGuid)], actualGuid);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetEnum)], actualEnumString);
            Assert.Equal(expected: happyValues[nameof(DataRecordExtensions.GetEnum)], actualEnumInt);
        });
    }

    [Fact]
    public void GetSomething_Should_Return_Fallback_When_DataRecord_Provides_DBNull_Value() {
        // arrange
        var extensions = new Dictionary<string, object> {
            { nameof(DataRecordExtensions.GetString), "String" },
            { nameof(DataRecordExtensions.GetBoolean), true },
            { nameof(DataRecordExtensions.GetChar), 'X' },
            { nameof(DataRecordExtensions.GetSByte), (sbyte)16 },
            { nameof(DataRecordExtensions.GetByte), (byte)32 },
            { nameof(DataRecordExtensions.GetInt16), (short)64 },
            { nameof(DataRecordExtensions.GetUInt16), (ushort)128 },
            { nameof(DataRecordExtensions.GetInt32), 256 },
            { nameof(DataRecordExtensions.GetUInt32), (uint)512 },
            { nameof(DataRecordExtensions.GetInt64), (long)1024 },
            { nameof(DataRecordExtensions.GetUInt64), (ulong)2048 },
            { nameof(DataRecordExtensions.GetSingle), 3.14f },
            { nameof(DataRecordExtensions.GetDouble), 9.10d },
            { nameof(DataRecordExtensions.GetDecimal), 100m },
            { nameof(DataRecordExtensions.GetDateTime), DateTime.Now },
            { nameof(DataRecordExtensions.GetDateTimeOffset), DateTimeOffset.Now },
            { nameof(DataRecordExtensions.GetTimeSpan), TimeSpan.FromSeconds(15) },
            { nameof(DataRecordExtensions.GetGuid), Guid.NewGuid() }
        };
        var dataRecordMock = new Mock<IDataRecord>();
        dataRecordMock
           .Setup(mock => mock[It.IsAny<string>()])
           .Returns(DBNull.Value);

        // act
        var actualString = dataRecordMock.Object.GetString(nameof(DataRecordExtensions.GetString),
            (string)extensions[nameof(DataRecordExtensions.GetString)]);
        var actualBoolean = dataRecordMock.Object.GetBoolean(nameof(DataRecordExtensions.GetBoolean),
            (bool)extensions[nameof(DataRecordExtensions.GetBoolean)]);
        var actualChar = dataRecordMock.Object.GetChar(nameof(DataRecordExtensions.GetChar),
            (char)extensions[nameof(DataRecordExtensions.GetChar)]);
        var actualSByte = dataRecordMock.Object.GetSByte(nameof(DataRecordExtensions.GetSByte),
            (sbyte)extensions[nameof(DataRecordExtensions.GetSByte)]);
        var actualByte = dataRecordMock.Object.GetByte(nameof(DataRecordExtensions.GetByte),
            (byte)extensions[nameof(DataRecordExtensions.GetByte)]);
        var actualInt16 = dataRecordMock.Object.GetInt16(nameof(DataRecordExtensions.GetInt16),
            (short)extensions[nameof(DataRecordExtensions.GetInt16)]);
        var actualUInt16 = dataRecordMock.Object.GetUInt16(nameof(DataRecordExtensions.GetUInt16),
            (ushort)extensions[nameof(DataRecordExtensions.GetUInt16)]);
        var actualInt32 = dataRecordMock.Object.GetInt32(nameof(DataRecordExtensions.GetInt32),
            (int)extensions[nameof(DataRecordExtensions.GetInt32)]);
        var actualUInt32 = dataRecordMock.Object.GetUInt32(nameof(DataRecordExtensions.GetUInt32),
            (uint)extensions[nameof(DataRecordExtensions.GetUInt32)]);
        var actualInt64 = dataRecordMock.Object.GetInt64(nameof(DataRecordExtensions.GetInt64),
            (long)extensions[nameof(DataRecordExtensions.GetInt64)]);
        var actualUInt64 = dataRecordMock.Object.GetUInt64(nameof(DataRecordExtensions.GetUInt64),
            (ulong)extensions[nameof(DataRecordExtensions.GetUInt64)]);
        var actualSingle = dataRecordMock.Object.GetSingle(nameof(DataRecordExtensions.GetSingle),
            (float)extensions[nameof(DataRecordExtensions.GetSingle)]);
        var actualDouble = dataRecordMock.Object.GetDouble(nameof(DataRecordExtensions.GetDouble),
            (double)extensions[nameof(DataRecordExtensions.GetDouble)]);
        var actualDecimal = dataRecordMock.Object.GetDecimal(nameof(DataRecordExtensions.GetDecimal),
            (decimal)extensions[nameof(DataRecordExtensions.GetDecimal)]);
        var actualDateTime = dataRecordMock.Object.GetDateTime(nameof(DataRecordExtensions.GetDateTime),
            (DateTime)extensions[nameof(DataRecordExtensions.GetDateTime)]);
        var actualDateTimeOffset = dataRecordMock.Object.GetDateTimeOffset(
            nameof(DataRecordExtensions.GetDateTimeOffset),
            (DateTimeOffset)extensions[nameof(DataRecordExtensions.GetDateTimeOffset)]);
        var actualTimeSpan = dataRecordMock.Object.GetTimeSpan(nameof(DataRecordExtensions.GetTimeSpan),
            (TimeSpan)extensions[nameof(DataRecordExtensions.GetTimeSpan)]);
        var actualGuid = dataRecordMock.Object.GetGuid(nameof(DataRecordExtensions.GetGuid),
            (Guid)extensions[nameof(DataRecordExtensions.GetGuid)]);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetString)], actualString);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetBoolean)], actualBoolean);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetChar)], actualChar);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetSByte)], actualSByte);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetByte)], actualByte);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetInt16)], actualInt16);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetUInt16)], actualUInt16);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetInt32)], actualInt32);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetUInt32)], actualUInt32);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetInt64)], actualInt64);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetUInt64)], actualUInt64);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetSingle)], actualSingle);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetDouble)], actualDouble);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetDecimal)], actualDecimal);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetDateTime)], actualDateTime);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetDateTimeOffset)], actualDateTimeOffset);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetTimeSpan)], actualTimeSpan);
            Assert.Equal(expected: extensions[nameof(DataRecordExtensions.GetGuid)], actualGuid);
        });
    }

    [Theory]
    [InlineData(nameof(DayOfWeek.Monday))]
    [InlineData((int)DayOfWeek.Monday)]
    public void GetEnum_Should_Return_Enum_Value(object value) {
        // arrange
        var expected = DayOfWeek.Monday;
        var dataRecordMock = new Mock<IDataRecord>();
        dataRecordMock
           .Setup(mock => mock[It.IsAny<string>()])
           .Returns(value);

        // act
        var actual = dataRecordMock.Object
                                   .GetEnum<DayOfWeek>("Enum");

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetEnum_Should_Return_Fallback() {
        // arrange
        var expected = DayOfWeek.Wednesday;
        var dataRecordMock = new Mock<IDataRecord>();
        dataRecordMock
           .Setup(mock => mock[It.IsAny<string>()])
           .Returns(DBNull.Value);

        // act
        var actual = dataRecordMock.Object
                                   .GetEnum("Enum", DayOfWeek.Wednesday);

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetEnum_Throws_InvalidOperationException_If_Method_Argument_Not_Enum() {
        Assert.Throws<InvalidOperationException>(() => Mock.Of<IDataRecord>().GetEnum<int>("Enum"));
    }

    [Fact]
    public void GetBlob_Should_Return_Array_Of_Bytes() {
        // arrange
        var expected = Encoding.UTF8.GetBytes("This is a test");
        var dataRecordMock = new Mock<IDataRecord>();
        dataRecordMock
           .Setup(mock => mock[It.IsAny<string>()])
           .Returns(expected);

        // act
        var actual = dataRecordMock.Object
                                   .GetBlob("Blob");

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetBlob_Should_Return_Fallback_If_DataRecord_Is_DBNull() {
        // arrange
        var fallback = Encoding.UTF8.GetBytes("This is a test");
        var dataRecordMock = new Mock<IDataRecord>();
        dataRecordMock
           .Setup(mock => mock[It.IsAny<string>()])
           .Returns(DBNull.Value);

        // act
        var actual = dataRecordMock.Object
                                   .GetBlob("Blob", fallback);

        // assert
        Assert.Equal(fallback, actual);
    }
}