using System.Data;
using System.Globalization;
using System.Text;
using Moq;

namespace Nameless.Data;

public class DataRecordExtensionTests {
    [Test]
    public void GetSomething_Should_Return_Value_From_DataRecord_Happy_Path() {
        // arrange
        var extensions = new Dictionary<string, object> {
            { nameof(DataRecordExtension.GetString), "Value" },
            { nameof(DataRecordExtension.GetBoolean), true },
            { nameof(DataRecordExtension.GetChar), 'X' },
            { nameof(DataRecordExtension.GetSByte), (sbyte)16 },
            { nameof(DataRecordExtension.GetByte), (byte)32 },
            { nameof(DataRecordExtension.GetInt16), (short)64 },
            { nameof(DataRecordExtension.GetUInt16), (ushort)128 },
            { nameof(DataRecordExtension.GetInt32), 256 },
            { nameof(DataRecordExtension.GetUInt32), (uint)512 },
            { nameof(DataRecordExtension.GetInt64), (long)1024 },
            { nameof(DataRecordExtension.GetUInt64), (ulong)2048 },
            { nameof(DataRecordExtension.GetSingle), 3.14f },
            { nameof(DataRecordExtension.GetDouble), 9.10d },
            { nameof(DataRecordExtension.GetDecimal), 100m },
            { nameof(DataRecordExtension.GetDateTime), DateTime.Now },
            { nameof(DataRecordExtension.GetDateTimeOffset), DateTimeOffset.Now },
            { nameof(DataRecordExtension.GetTimeSpan), TimeSpan.FromSeconds(15) },
            { nameof(DataRecordExtension.GetGuid), Guid.NewGuid() },
            { nameof(DataRecordExtension.GetEnum), DayOfWeek.Wednesday },
        };
        var dataRecordMock = new Mock<IDataRecord>();
        foreach (var extension in extensions) {
            dataRecordMock
                .Setup(mock => mock[extension.Key])
                .Returns(extension.Value);
        }

        // act
        var actualString = DataRecordExtension.GetString(dataRecordMock.Object, nameof(DataRecordExtension.GetString));
        var actualBoolean = DataRecordExtension.GetBoolean(dataRecordMock.Object, nameof(DataRecordExtension.GetBoolean));
        var actualChar = DataRecordExtension.GetChar(dataRecordMock.Object, nameof(DataRecordExtension.GetChar));
        var actualSByte = DataRecordExtension.GetSByte(dataRecordMock.Object, nameof(DataRecordExtension.GetSByte));
        var actualByte = DataRecordExtension.GetByte(dataRecordMock.Object, nameof(DataRecordExtension.GetByte));
        var actualInt16 = DataRecordExtension.GetInt16(dataRecordMock.Object, nameof(DataRecordExtension.GetInt16));
        var actualUInt16 = DataRecordExtension.GetUInt16(dataRecordMock.Object, nameof(DataRecordExtension.GetUInt16));
        var actualInt32 = DataRecordExtension.GetInt32(dataRecordMock.Object, nameof(DataRecordExtension.GetInt32));
        var actualUInt32 = DataRecordExtension.GetUInt32(dataRecordMock.Object, nameof(DataRecordExtension.GetUInt32));
        var actualInt64 = DataRecordExtension.GetInt64(dataRecordMock.Object, nameof(DataRecordExtension.GetInt64));
        var actualUInt64 = DataRecordExtension.GetUInt64(dataRecordMock.Object, nameof(DataRecordExtension.GetUInt64));
        var actualSingle = DataRecordExtension.GetSingle(dataRecordMock.Object, nameof(DataRecordExtension.GetSingle));
        var actualDouble = DataRecordExtension.GetDouble(dataRecordMock.Object, nameof(DataRecordExtension.GetDouble));
        var actualDecimal = DataRecordExtension.GetDecimal(dataRecordMock.Object, nameof(DataRecordExtension.GetDecimal));
        var actualDateTime = DataRecordExtension.GetDateTime(dataRecordMock.Object, nameof(DataRecordExtension.GetDateTime));
        var actualDateTimeOffset = DataRecordExtension.GetDateTimeOffset(dataRecordMock.Object, nameof(DataRecordExtension.GetDateTimeOffset));
        var actualTimeSpan = DataRecordExtension.GetTimeSpan(dataRecordMock.Object, nameof(DataRecordExtension.GetTimeSpan));
        var actualGuid = DataRecordExtension.GetGuid(dataRecordMock.Object, nameof(DataRecordExtension.GetGuid));
        var actualEnum = DataRecordExtension.GetEnum<DayOfWeek>(dataRecordMock.Object, nameof(DataRecordExtension.GetEnum));

        // assert
        Assert.Multiple(() => {
            Assert.That(actualString, Is.EqualTo(extensions[nameof(DataRecordExtension.GetString)]));
            Assert.That(actualBoolean, Is.EqualTo(extensions[nameof(DataRecordExtension.GetBoolean)]));
            Assert.That(actualChar, Is.EqualTo(extensions[nameof(DataRecordExtension.GetChar)]));
            Assert.That(actualSByte, Is.EqualTo(extensions[nameof(DataRecordExtension.GetSByte)]));
            Assert.That(actualByte, Is.EqualTo(extensions[nameof(DataRecordExtension.GetByte)]));
            Assert.That(actualInt16, Is.EqualTo(extensions[nameof(DataRecordExtension.GetInt16)]));
            Assert.That(actualUInt16, Is.EqualTo(extensions[nameof(DataRecordExtension.GetUInt16)]));
            Assert.That(actualInt32, Is.EqualTo(extensions[nameof(DataRecordExtension.GetInt32)]));
            Assert.That(actualUInt32, Is.EqualTo(extensions[nameof(DataRecordExtension.GetUInt32)]));
            Assert.That(actualInt64, Is.EqualTo(extensions[nameof(DataRecordExtension.GetInt64)]));
            Assert.That(actualUInt64, Is.EqualTo(extensions[nameof(DataRecordExtension.GetUInt64)]));
            Assert.That(actualSingle, Is.EqualTo(extensions[nameof(DataRecordExtension.GetSingle)]));
            Assert.That(actualDouble, Is.EqualTo(extensions[nameof(DataRecordExtension.GetDouble)]));
            Assert.That(actualDecimal, Is.EqualTo(extensions[nameof(DataRecordExtension.GetDecimal)]));
            Assert.That(actualDateTime, Is.EqualTo(extensions[nameof(DataRecordExtension.GetDateTime)]));
            Assert.That(actualDateTimeOffset, Is.EqualTo(extensions[nameof(DataRecordExtension.GetDateTimeOffset)]));
            Assert.That(actualTimeSpan, Is.EqualTo(extensions[nameof(DataRecordExtension.GetTimeSpan)]));
            Assert.That(actualGuid, Is.EqualTo(extensions[nameof(DataRecordExtension.GetGuid)]));
            Assert.That(actualEnum, Is.EqualTo(extensions[nameof(DataRecordExtension.GetEnum)]));
        });
    }

    [Test]
    public void GetSomething_Should_Return_Value_From_DataRecord() {
        // arrange
        var culture = new CultureInfo("en-US");
        var guid = Guid.NewGuid();
        var now = DateTime.Now.Date;

        var happyValues = new Dictionary<string, object> {
            { nameof(DataRecordExtension.GetString), "Value" },
            { nameof(DataRecordExtension.GetBoolean), true },
            { nameof(DataRecordExtension.GetChar), 'X' },
            { nameof(DataRecordExtension.GetSByte), (sbyte)16 },
            { nameof(DataRecordExtension.GetByte), (byte)32 },
            { nameof(DataRecordExtension.GetInt16), (short)64 },
            { nameof(DataRecordExtension.GetUInt16), (ushort)128 },
            { nameof(DataRecordExtension.GetInt32), 256 },
            { nameof(DataRecordExtension.GetUInt32), (uint)512 },
            { nameof(DataRecordExtension.GetInt64), (long)1024 },
            { nameof(DataRecordExtension.GetUInt64), (ulong)2048 },
            { nameof(DataRecordExtension.GetSingle), 3.14f },
            { nameof(DataRecordExtension.GetDouble), 9.10d },
            { nameof(DataRecordExtension.GetDecimal), 100m },
            { nameof(DataRecordExtension.GetDateTime), now },
            { nameof(DataRecordExtension.GetDateTimeOffset), new DateTimeOffset(now) },
            { nameof(DataRecordExtension.GetTimeSpan), TimeSpan.FromSeconds(15) },
            { nameof(DataRecordExtension.GetGuid), guid },
            { nameof(DataRecordExtension.GetEnum), DayOfWeek.Wednesday },
        };

        var extensions = new Dictionary<string, object> {
            { nameof(DataRecordExtension.GetString), "Value" },
            { nameof(DataRecordExtension.GetBoolean), "true" },
            { nameof(DataRecordExtension.GetChar), "X" },
            { nameof(DataRecordExtension.GetSByte), "16" },
            { nameof(DataRecordExtension.GetByte), "32" },
            { nameof(DataRecordExtension.GetInt16), "64" },
            { nameof(DataRecordExtension.GetUInt16), "128" },
            { nameof(DataRecordExtension.GetInt32), "256" },
            { nameof(DataRecordExtension.GetUInt32), "512" },
            { nameof(DataRecordExtension.GetInt64), "1024" },
            { nameof(DataRecordExtension.GetUInt64), "2048" },
            { nameof(DataRecordExtension.GetSingle), "3.14" },
            { nameof(DataRecordExtension.GetDouble), "9.10" },
            { nameof(DataRecordExtension.GetDecimal), "100" },
            { nameof(DataRecordExtension.GetDateTime), now.ToString(culture) },
            { nameof(DataRecordExtension.GetDateTimeOffset), new DateTimeOffset(now).ToString(culture) },
            { nameof(DataRecordExtension.GetTimeSpan), TimeSpan.FromSeconds(15).ToString() },
            { nameof(DataRecordExtension.GetGuid), guid.ToString() },
            { nameof(DataRecordExtension.GetEnum) + "string", nameof(DayOfWeek.Wednesday) },
            { nameof(DataRecordExtension.GetEnum) + "int", (int)DayOfWeek.Wednesday },
        };
        var dataRecordMock = new Mock<IDataRecord>();
        foreach (var extension in extensions) {
            dataRecordMock
                .Setup(mock => mock[extension.Key])
                .Returns(extension.Value);
        }

        // act
        var actualString = DataRecordExtension.GetString(dataRecordMock.Object, nameof(DataRecordExtension.GetString), formatProvider: culture);
        var actualBoolean = DataRecordExtension.GetBoolean(dataRecordMock.Object, nameof(DataRecordExtension.GetBoolean), formatProvider: culture);
        var actualChar = DataRecordExtension.GetChar(dataRecordMock.Object, nameof(DataRecordExtension.GetChar), formatProvider: culture);
        var actualSByte = DataRecordExtension.GetSByte(dataRecordMock.Object, nameof(DataRecordExtension.GetSByte), formatProvider: culture);
        var actualByte = DataRecordExtension.GetByte(dataRecordMock.Object, nameof(DataRecordExtension.GetByte), formatProvider: culture);
        var actualInt16 = DataRecordExtension.GetInt16(dataRecordMock.Object, nameof(DataRecordExtension.GetInt16), formatProvider: culture);
        var actualUInt16 = DataRecordExtension.GetUInt16(dataRecordMock.Object, nameof(DataRecordExtension.GetUInt16), formatProvider: culture);
        var actualInt32 = DataRecordExtension.GetInt32(dataRecordMock.Object, nameof(DataRecordExtension.GetInt32), formatProvider: culture);
        var actualUInt32 = DataRecordExtension.GetUInt32(dataRecordMock.Object, nameof(DataRecordExtension.GetUInt32), formatProvider: culture);
        var actualInt64 = DataRecordExtension.GetInt64(dataRecordMock.Object, nameof(DataRecordExtension.GetInt64), formatProvider: culture);
        var actualUInt64 = DataRecordExtension.GetUInt64(dataRecordMock.Object, nameof(DataRecordExtension.GetUInt64), formatProvider: culture);
        var actualSingle = DataRecordExtension.GetSingle(dataRecordMock.Object, nameof(DataRecordExtension.GetSingle), formatProvider: culture);
        var actualDouble = DataRecordExtension.GetDouble(dataRecordMock.Object, nameof(DataRecordExtension.GetDouble), formatProvider: culture);
        var actualDecimal = DataRecordExtension.GetDecimal(dataRecordMock.Object, nameof(DataRecordExtension.GetDecimal), formatProvider: culture);
        var actualDateTime = DataRecordExtension.GetDateTime(dataRecordMock.Object, nameof(DataRecordExtension.GetDateTime), formatProvider: culture);
        var actualDateTimeOffset = DataRecordExtension.GetDateTimeOffset(dataRecordMock.Object, nameof(DataRecordExtension.GetDateTimeOffset), formatProvider: culture);
        var actualTimeSpan = DataRecordExtension.GetTimeSpan(dataRecordMock.Object, nameof(DataRecordExtension.GetTimeSpan), formatProvider: culture);
        var actualGuid = DataRecordExtension.GetGuid(dataRecordMock.Object, nameof(DataRecordExtension.GetGuid));
        var actualEnumString = DataRecordExtension.GetEnum<DayOfWeek>(dataRecordMock.Object, nameof(DataRecordExtension.GetEnum) + "string");
        var actualEnumInt = DataRecordExtension.GetEnum<DayOfWeek>(dataRecordMock.Object, nameof(DataRecordExtension.GetEnum) + "int");

        // assert
        Assert.Multiple(() => {
            Assert.That(actualString, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetString)]));
            Assert.That(actualBoolean, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetBoolean)]));
            Assert.That(actualChar, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetChar)]));
            Assert.That(actualSByte, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetSByte)]));
            Assert.That(actualByte, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetByte)]));
            Assert.That(actualInt16, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetInt16)]));
            Assert.That(actualUInt16, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetUInt16)]));
            Assert.That(actualInt32, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetInt32)]));
            Assert.That(actualUInt32, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetUInt32)]));
            Assert.That(actualInt64, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetInt64)]));
            Assert.That(actualUInt64, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetUInt64)]));
            Assert.That(actualSingle, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetSingle)]));
            Assert.That(actualDouble, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetDouble)]));
            Assert.That(actualDecimal, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetDecimal)]));
            Assert.That(actualDateTime, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetDateTime)]));
            Assert.That(actualDateTimeOffset, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetDateTimeOffset)]));
            Assert.That(actualTimeSpan, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetTimeSpan)]));
            Assert.That(actualGuid, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetGuid)]));
            Assert.That(actualEnumString, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetEnum)]));
            Assert.That(actualEnumInt, Is.EqualTo(happyValues[nameof(DataRecordExtension.GetEnum)]));
        });
    }

    [Test]
    public void GetSomething_Should_Return_Fallback_When_DataRecord_Provides_DBNull_Value() {
        // arrange
        var extensions = new Dictionary<string, object> {
            { nameof(DataRecordExtension.GetString), "String" },
            { nameof(DataRecordExtension.GetBoolean), true },
            { nameof(DataRecordExtension.GetChar), 'X' },
            { nameof(DataRecordExtension.GetSByte), (sbyte)16 },
            { nameof(DataRecordExtension.GetByte), (byte)32 },
            { nameof(DataRecordExtension.GetInt16), (short)64 },
            { nameof(DataRecordExtension.GetUInt16), (ushort)128 },
            { nameof(DataRecordExtension.GetInt32), 256 },
            { nameof(DataRecordExtension.GetUInt32), (uint)512 },
            { nameof(DataRecordExtension.GetInt64), (long)1024 },
            { nameof(DataRecordExtension.GetUInt64), (ulong)2048 },
            { nameof(DataRecordExtension.GetSingle), 3.14f },
            { nameof(DataRecordExtension.GetDouble), 9.10d },
            { nameof(DataRecordExtension.GetDecimal), 100m },
            { nameof(DataRecordExtension.GetDateTime), DateTime.Now },
            { nameof(DataRecordExtension.GetDateTimeOffset), DateTimeOffset.Now },
            { nameof(DataRecordExtension.GetTimeSpan), TimeSpan.FromSeconds(15) },
            { nameof(DataRecordExtension.GetGuid), Guid.NewGuid() },
        };
        var dataRecordMock = new Mock<IDataRecord>();
        dataRecordMock
            .Setup(mock => mock[It.IsAny<string>()])
            .Returns(DBNull.Value);

        // act
        var actualString = DataRecordExtension.GetString(dataRecordMock.Object, nameof(DataRecordExtension.GetString), (string)extensions[nameof(DataRecordExtension.GetString)]);
        var actualBoolean = DataRecordExtension.GetBoolean(dataRecordMock.Object, nameof(DataRecordExtension.GetBoolean), (bool)extensions[nameof(DataRecordExtension.GetBoolean)]);
        var actualChar = DataRecordExtension.GetChar(dataRecordMock.Object, nameof(DataRecordExtension.GetChar), (char)extensions[nameof(DataRecordExtension.GetChar)]);
        var actualSByte = DataRecordExtension.GetSByte(dataRecordMock.Object, nameof(DataRecordExtension.GetSByte), (sbyte)extensions[nameof(DataRecordExtension.GetSByte)]);
        var actualByte = DataRecordExtension.GetByte(dataRecordMock.Object, nameof(DataRecordExtension.GetByte), (byte)extensions[nameof(DataRecordExtension.GetByte)]);
        var actualInt16 = DataRecordExtension.GetInt16(dataRecordMock.Object, nameof(DataRecordExtension.GetInt16), (short)extensions[nameof(DataRecordExtension.GetInt16)]);
        var actualUInt16 = DataRecordExtension.GetUInt16(dataRecordMock.Object, nameof(DataRecordExtension.GetUInt16), (ushort)extensions[nameof(DataRecordExtension.GetUInt16)]);
        var actualInt32 = DataRecordExtension.GetInt32(dataRecordMock.Object, nameof(DataRecordExtension.GetInt32), (int)extensions[nameof(DataRecordExtension.GetInt32)]);
        var actualUInt32 = DataRecordExtension.GetUInt32(dataRecordMock.Object, nameof(DataRecordExtension.GetUInt32), (uint)extensions[nameof(DataRecordExtension.GetUInt32)]);
        var actualInt64 = DataRecordExtension.GetInt64(dataRecordMock.Object, nameof(DataRecordExtension.GetInt64), (long)extensions[nameof(DataRecordExtension.GetInt64)]);
        var actualUInt64 = DataRecordExtension.GetUInt64(dataRecordMock.Object, nameof(DataRecordExtension.GetUInt64), (ulong)extensions[nameof(DataRecordExtension.GetUInt64)]);
        var actualSingle = DataRecordExtension.GetSingle(dataRecordMock.Object, nameof(DataRecordExtension.GetSingle), (float)extensions[nameof(DataRecordExtension.GetSingle)]);
        var actualDouble = DataRecordExtension.GetDouble(dataRecordMock.Object, nameof(DataRecordExtension.GetDouble), (double)extensions[nameof(DataRecordExtension.GetDouble)]);
        var actualDecimal = DataRecordExtension.GetDecimal(dataRecordMock.Object, nameof(DataRecordExtension.GetDecimal), (decimal)extensions[nameof(DataRecordExtension.GetDecimal)]);
        var actualDateTime = DataRecordExtension.GetDateTime(dataRecordMock.Object, nameof(DataRecordExtension.GetDateTime), (DateTime)extensions[nameof(DataRecordExtension.GetDateTime)]);
        var actualDateTimeOffset = DataRecordExtension.GetDateTimeOffset(dataRecordMock.Object, nameof(DataRecordExtension.GetDateTimeOffset), (DateTimeOffset)extensions[nameof(DataRecordExtension.GetDateTimeOffset)]);
        var actualTimeSpan = DataRecordExtension.GetTimeSpan(dataRecordMock.Object, nameof(DataRecordExtension.GetTimeSpan), (TimeSpan)extensions[nameof(DataRecordExtension.GetTimeSpan)]);
        var actualGuid = DataRecordExtension.GetGuid(dataRecordMock.Object, nameof(DataRecordExtension.GetGuid), (Guid)extensions[nameof(DataRecordExtension.GetGuid)]);

        // assert
        Assert.Multiple(() => {
            Assert.That(actualString, Is.EqualTo(extensions[nameof(DataRecordExtension.GetString)]));
            Assert.That(actualBoolean, Is.EqualTo(extensions[nameof(DataRecordExtension.GetBoolean)]));
            Assert.That(actualChar, Is.EqualTo(extensions[nameof(DataRecordExtension.GetChar)]));
            Assert.That(actualSByte, Is.EqualTo(extensions[nameof(DataRecordExtension.GetSByte)]));
            Assert.That(actualByte, Is.EqualTo(extensions[nameof(DataRecordExtension.GetByte)]));
            Assert.That(actualInt16, Is.EqualTo(extensions[nameof(DataRecordExtension.GetInt16)]));
            Assert.That(actualUInt16, Is.EqualTo(extensions[nameof(DataRecordExtension.GetUInt16)]));
            Assert.That(actualInt32, Is.EqualTo(extensions[nameof(DataRecordExtension.GetInt32)]));
            Assert.That(actualUInt32, Is.EqualTo(extensions[nameof(DataRecordExtension.GetUInt32)]));
            Assert.That(actualInt64, Is.EqualTo(extensions[nameof(DataRecordExtension.GetInt64)]));
            Assert.That(actualUInt64, Is.EqualTo(extensions[nameof(DataRecordExtension.GetUInt64)]));
            Assert.That(actualSingle, Is.EqualTo(extensions[nameof(DataRecordExtension.GetSingle)]));
            Assert.That(actualDouble, Is.EqualTo(extensions[nameof(DataRecordExtension.GetDouble)]));
            Assert.That(actualDecimal, Is.EqualTo(extensions[nameof(DataRecordExtension.GetDecimal)]));
            Assert.That(actualDateTime, Is.EqualTo(extensions[nameof(DataRecordExtension.GetDateTime)]));
            Assert.That(actualDateTimeOffset, Is.EqualTo(extensions[nameof(DataRecordExtension.GetDateTimeOffset)]));
            Assert.That(actualTimeSpan, Is.EqualTo(extensions[nameof(DataRecordExtension.GetTimeSpan)]));
            Assert.That(actualGuid, Is.EqualTo(extensions[nameof(DataRecordExtension.GetGuid)]));
        });
    }

    [TestCase(nameof(DayOfWeek.Monday))]
    [TestCase((int)DayOfWeek.Monday)]
    public void GetEnum_Should_Return_Enum_Value(object value) {
        // arrange
        var expected = DayOfWeek.Monday;
        var dataRecordMock = new Mock<IDataRecord>();
        dataRecordMock
            .Setup(mock => mock[It.IsAny<string>()])
            .Returns(value);

        // act
        var actual = DataRecordExtension
            .GetEnum<DayOfWeek>(dataRecordMock.Object, "Enum");

        // assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetEnum_Should_Return_Fallback() {
        // arrange
        var expected = DayOfWeek.Wednesday;
        var dataRecordMock = new Mock<IDataRecord>();
        dataRecordMock
            .Setup(mock => mock[It.IsAny<string>()])
            .Returns(DBNull.Value);

        // act
        var actual = DataRecordExtension
            .GetEnum(dataRecordMock.Object, "Enum", DayOfWeek.Wednesday);

        // assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetEnum_Throws_InvalidOperationException_If_Method_Argument_Not_Enum()
        => Assert.Throws<InvalidOperationException>(
            code: () => DataRecordExtension.GetEnum<int>(
                self: Mock.Of<IDataRecord>(),
                columnName: "Enum"
            )
        );

    [Test]
    public void GetBlob_Should_Return_Array_Of_Bytes() {
        // arrange
        var expected = Encoding.UTF8.GetBytes("This is a test");
        var dataRecordMock = new Mock<IDataRecord>();
        dataRecordMock
            .Setup(mock => mock[It.IsAny<string>()])
            .Returns(expected);

        // act
        var actual = DataRecordExtension
            .GetBlob(dataRecordMock.Object, "Blob");

        // assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetBlob_Should_Return_Fallback_If_DataRecord_Is_DBNull() {
        // arrange
        var fallback = Encoding.UTF8.GetBytes("This is a test");
        var dataRecordMock = new Mock<IDataRecord>();
        dataRecordMock
            .Setup(mock => mock[It.IsAny<string>()])
            .Returns(DBNull.Value);

        // act
        var actual = DataRecordExtension
            .GetBlob(dataRecordMock.Object, "Blob", fallback);

        // assert
        Assert.That(actual, Is.EqualTo(fallback));
    }
}