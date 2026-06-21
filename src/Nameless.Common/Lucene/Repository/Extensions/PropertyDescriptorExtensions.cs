using System.Diagnostics.CodeAnalysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Nameless.Lucene.Repository.Mappings;

namespace Nameless.Lucene.Repository;

internal static class PropertyDescriptorExtensions {
    extension<TDocument>(PropertyDescriptor<TDocument> self)
        where TDocument : class {
        internal bool TryCreateField(TDocument instance, [NotNullWhen(returnValue: true)] out IIndexableField? output) {
            var value = self.Getter(instance);

            output = value switch {
                string stringValue => CreateStringField(self, stringValue),
                int intValue => new Int32Field(self.Name, intValue, GetStoreOptions(self)),
                bool boolValue => new Int32Field(self.Name, boolValue ? 1 : 0, GetStoreOptions(self)),
                Guid guidValue => CreateStringField(self, guidValue.ToString()),
                DateTime dateTimeValue => new Int64Field(self.Name, dateTimeValue.Ticks, GetStoreOptions(self)),
                Enum enumValue => new StringField(self.Name, enumValue.ToString(), GetStoreOptions(self)),
                double doubleValue => new DoubleField(self.Name, doubleValue, GetStoreOptions(self)),

                decimal decimalValue => new DoubleField(self.Name, Convert.ToDouble(decimalValue), GetStoreOptions(self)),
                char charValue => CreateStringField(self, new string([charValue])),
                sbyte sbyteValue => new Int32Field(self.Name, sbyteValue, GetStoreOptions(self)),
                byte byteValue => new Int32Field(self.Name, byteValue, GetStoreOptions(self)),
                ushort ushortValue => new Int32Field(self.Name, ushortValue, GetStoreOptions(self)),
                short shortValue => new Int32Field(self.Name, shortValue, GetStoreOptions(self)),
                uint uintValue => new Int64Field(self.Name, uintValue, GetStoreOptions(self)),
                ulong ulongValue => new DoubleField(self.Name, ulongValue, GetStoreOptions(self)),
                long longValue => new Int64Field(self.Name, longValue, GetStoreOptions(self)),
                float floatValue => new SingleField(self.Name, floatValue, GetStoreOptions(self)),
                DateTimeOffset dateTimeOffsetValue => new Int64Field(self.Name, dateTimeOffsetValue.Ticks, GetStoreOptions(self)),
                DateOnly dateOnlyValue => new Int64Field(self.Name, dateOnlyValue.ToDateTime(TimeOnly.MinValue).Ticks, GetStoreOptions(self)),
                TimeOnly timeOnlyValue => new Int64Field(self.Name, timeOnlyValue.Ticks, GetStoreOptions(self)),
                TimeSpan timeSpanValue => new Int64Field(self.Name, timeSpanValue.Ticks, GetStoreOptions(self)),
                _ => null
            };

            return output is not null;
        }
    }

    private static IIndexableField CreateStringField(PropertyDescriptor self, string value) {
        if (self.Options.HasFlag(PropertyOptions.Sanitize)) {
            value = value.RemoveHtmlTags();
        }

        return self.Options.HasFlag(PropertyOptions.Analyze)
            ? new TextField(self.Name, value, GetStoreOptions(self))
            : new StringField(self.Name, value, GetStoreOptions(self));
    }

    private static Field.Store GetStoreOptions(PropertyDescriptor self) {
        return self.Options.HasFlag(PropertyOptions.Store)
            ? Field.Store.YES
            : Field.Store.NO;
    }
}
