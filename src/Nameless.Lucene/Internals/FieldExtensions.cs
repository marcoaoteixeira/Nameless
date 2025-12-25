using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace Nameless.Lucene.Internals;

/// <summary>
///     <see cref="Field"/> extension methods.
/// </summary>
internal static class FieldExtensions {
    /// <summary>
    ///     Converts <see cref="Field"/> to <see cref="IIndexableField"/>.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="Field"/>.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="IIndexableField"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Field.Type"/> is not supported.
    /// </exception>
    internal static IIndexableField ToIndexableField(this Field self) {
        return self.Type switch {
            IndexableType.Boolean => self.CreateBoolean(),
            IndexableType.String => self.CreateString(),
            IndexableType.Byte => self.CreateByte(),
            IndexableType.Short => self.CreateShort(),
            IndexableType.Integer => self.CreateInteger(),
            IndexableType.Long => self.CreateLong(),
            IndexableType.Float => self.CreateFloat(),
            IndexableType.Double => self.CreateDouble(),
            IndexableType.DateTimeOffset => self.CreateDateTimeOffset(),
            IndexableType.DateTime => self.CreateDateTime(),
            IndexableType.DateOnly => self.CreateDateOnly(),
            IndexableType.TimeOnly => self.CreateTimeOnly(),
            IndexableType.TimeSpan => self.CreateTimeSpan(),
            IndexableType.Enum => self.CreateEnum(),
            _ => throw new InvalidOperationException("Indexable type not available.")
        };
    }

    private static Int32Field CreateBoolean(this Field self) {
        // Boolean is stored as integer
        return new Int32Field(
            name: self.Name,
            value: self.Value is true ? 1 : 0,
            stored: self.GetStoreOption()
        );
    }

    private static LuceneField CreateString(this Field self) {
        var value = (string)self.Value;

        if (self.Options.HasFlag(FieldOptions.Sanitize)) {
            value = value.RemoveHtmlTags();
        }

        return self.Options.HasFlag(FieldOptions.Analyze)
            ? new TextField(self.Name, value, self.GetStoreOption())
            : new StringField(self.Name, value, self.GetStoreOption());
    }

    private static Int32Field CreateByte(this Field self) {
        return new Int32Field(self.Name, (byte)self.Value, self.GetStoreOption());
    }

    private static Int32Field CreateShort(this Field self) {
        return new Int32Field(self.Name, (short)self.Value, self.GetStoreOption());
    }

    private static Int32Field CreateInteger(this Field self) {
        return new Int32Field(self.Name, (int)self.Value, self.GetStoreOption());
    }

    private static Int64Field CreateLong(this Field self) {
        return new Int64Field(self.Name, (long)self.Value, self.GetStoreOption());
    }

    private static SingleField CreateFloat(this Field self) {
        return new SingleField(self.Name, (float)self.Value, self.GetStoreOption());
    }

    private static DoubleField CreateDouble(this Field self) {
        return new DoubleField(self.Name, (double)self.Value, self.GetStoreOption());
    }

    private static Int64Field CreateDateTimeOffset(this Field self) {
        // DateTimeOffset is stored as ticks

        // Ensure the value is in UTC
        var value = ((DateTimeOffset)self.Value).ToUniversalTime();

        return new Int64Field(self.Name, value.Ticks, self.GetStoreOption());
    }

    private static Int64Field CreateDateTime(this Field self) {
        // DateTime is stored as ticks

        // Ensure the value is in UTC
        var value = ((DateTime)self.Value).ToUniversalTime();

        return new Int64Field(self.Name, value.Ticks, self.GetStoreOption());
    }

    private static Int64Field CreateDateOnly(this Field self) {
        // DateOnly is stored as ticks

        // Ensure the value is in UTC
        var value = ((DateOnly)self.Value).ToDateTime(TimeOnly.MinValue)
                                          .ToUniversalTime();

        return new Int64Field(self.Name, value.Ticks, self.GetStoreOption());
    }

    private static Int64Field CreateTimeOnly(this Field self) {
        // TimeOnly is stored as ticks
        var value = (TimeOnly)self.Value;

        return new Int64Field(self.Name, value.Ticks, self.GetStoreOption());
    }

    private static Int64Field CreateTimeSpan(this Field self) {
        // TimeSpan is stored as ticks
        var value = (TimeSpan)self.Value;

        return new Int64Field(self.Name, value.Ticks, self.GetStoreOption());
    }

    private static StringField CreateEnum(this Field self) {
        return new StringField(
            self.Name,
            self.Value.ToString(),
            self.GetStoreOption()
        );
    }

    private static LuceneField.Store GetStoreOption(this Field self) {
        return self.Options.HasFlag(FieldOptions.Store)
            ? LuceneField.Store.YES
            : LuceneField.Store.NO;
    }
}
