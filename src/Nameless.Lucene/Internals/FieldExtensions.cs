using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace Nameless.Lucene.Internals;

/// <summary>
///     <see cref="Field"/> extension methods.
/// </summary>
internal static class FieldExtensions {
    /// <param name="self">
    ///     The current <see cref="Field"/>.
    /// </param>
    extension(Field self) {
        /// <summary>
        ///     Converts <see cref="Field"/> to <see cref="IIndexableField"/>.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IIndexableField"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     If the <see cref="Field.Type"/> is not supported.
        /// </exception>
        internal IIndexableField ToIndexableField() {
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
                _ => throw new InvalidOperationException(message: "Indexable type not available.")
            };
        }

        private Int32Field CreateBoolean() {
            // Boolean is stored as integer
            return new Int32Field(
                self.Name,
                self.Value is true ? 1 : 0,
                self.GetStoreOption()
            );
        }

        private LuceneField CreateString() {
            var value = (string)self.Value;

            if (self.Options.HasFlag(FieldOptions.Sanitize)) {
                value = value.RemoveHtmlTags();
            }

            return self.Options.HasFlag(FieldOptions.Analyze)
                ? new TextField(self.Name, value, self.GetStoreOption())
                : new StringField(self.Name, value, self.GetStoreOption());
        }

        private Int32Field CreateByte() {
            return new Int32Field(self.Name, (byte)self.Value, self.GetStoreOption());
        }

        private Int32Field CreateShort() {
            return new Int32Field(self.Name, (short)self.Value, self.GetStoreOption());
        }

        private Int32Field CreateInteger() {
            return new Int32Field(self.Name, (int)self.Value, self.GetStoreOption());
        }

        private Int64Field CreateLong() {
            return new Int64Field(self.Name, (long)self.Value, self.GetStoreOption());
        }

        private SingleField CreateFloat() {
            return new SingleField(self.Name, (float)self.Value, self.GetStoreOption());
        }

        private DoubleField CreateDouble() {
            return new DoubleField(self.Name, (double)self.Value, self.GetStoreOption());
        }

        private Int64Field CreateDateTimeOffset() {
            // DateTimeOffset is stored as ticks

            // Ensure the value is in UTC
            var value = ((DateTimeOffset)self.Value).ToUniversalTime();

            return new Int64Field(self.Name, value.Ticks, self.GetStoreOption());
        }

        private Int64Field CreateDateTime() {
            // DateTime is stored as ticks

            // Ensure the value is in UTC
            var value = ((DateTime)self.Value).ToUniversalTime();

            return new Int64Field(self.Name, value.Ticks, self.GetStoreOption());
        }

        private Int64Field CreateDateOnly() {
            // DateOnly is stored as ticks

            // Ensure the value is in UTC
            var value = ((DateOnly)self.Value).ToDateTime(TimeOnly.MinValue)
                                              .ToUniversalTime();

            return new Int64Field(self.Name, value.Ticks, self.GetStoreOption());
        }

        private Int64Field CreateTimeOnly() {
            // TimeOnly is stored as ticks
            var value = (TimeOnly)self.Value;

            return new Int64Field(self.Name, value.Ticks, self.GetStoreOption());
        }

        private Int64Field CreateTimeSpan() {
            // TimeSpan is stored as ticks
            var value = (TimeSpan)self.Value;

            return new Int64Field(self.Name, value.Ticks, self.GetStoreOption());
        }

        private StringField CreateEnum() {
            return new StringField(
                self.Name,
                self.Value.ToString(),
                self.GetStoreOption()
            );
        }

        private LuceneField.Store GetStoreOption() {
            return self.Options.HasFlag(FieldOptions.Store)
                ? LuceneField.Store.YES
                : LuceneField.Store.NO;
        }
    }
}