using System.Diagnostics.CodeAnalysis;
using Lucene.Net.Documents;
using Lucene.Net.Documents.Extensions;
using Lucene.Net.Index;

namespace Nameless.Lucene.Mapping;

internal static class IndexableFieldHelper {
    internal static bool TryCreate(PropertyDescriptor descriptor, object? value, [NotNullWhen(returnValue: true)] out IIndexableField? output) {
        output = value switch {
            string stringValue => CreateStringField(descriptor, stringValue),
            int intValue => new Int32Field(descriptor.Name, intValue, GetStoreOptions(descriptor)),
            bool boolValue => new Int32Field(descriptor.Name, boolValue ? 1 : 0, GetStoreOptions(descriptor)),
            Guid guidValue => CreateStringField(descriptor, guidValue.ToString()),
            DateTime dateTimeValue => new Int64Field(descriptor.Name, dateTimeValue.Ticks, GetStoreOptions(descriptor)),
            Enum enumValue => new StringField(descriptor.Name, enumValue.ToString(), GetStoreOptions(descriptor)),
            double doubleValue => new DoubleField(descriptor.Name, doubleValue, GetStoreOptions(descriptor)),

            decimal decimalValue => new DoubleField(descriptor.Name, Convert.ToDouble(decimalValue), GetStoreOptions(descriptor)),
            char charValue => CreateStringField(descriptor, new string([charValue])),
            sbyte sbyteValue => new Int32Field(descriptor.Name, sbyteValue, GetStoreOptions(descriptor)),
            byte byteValue => new Int32Field(descriptor.Name, byteValue, GetStoreOptions(descriptor)),
            ushort ushortValue => new Int32Field(descriptor.Name, ushortValue, GetStoreOptions(descriptor)),
            short shortValue => new Int32Field(descriptor.Name, shortValue, GetStoreOptions(descriptor)),
            uint uintValue => new Int64Field(descriptor.Name, uintValue, GetStoreOptions(descriptor)),
            ulong ulongValue => new DoubleField(descriptor.Name, ulongValue, GetStoreOptions(descriptor)),
            long longValue => new Int64Field(descriptor.Name, longValue, GetStoreOptions(descriptor)),
            float floatValue => new SingleField(descriptor.Name, floatValue, GetStoreOptions(descriptor)),
            DateTimeOffset dateTimeOffsetValue => new Int64Field(descriptor.Name, dateTimeOffsetValue.Ticks, GetStoreOptions(descriptor)),
            DateOnly dateOnlyValue => new Int64Field(descriptor.Name, dateOnlyValue.ToDateTime(TimeOnly.MinValue).Ticks, GetStoreOptions(descriptor)),
            TimeOnly timeOnlyValue => new Int64Field(descriptor.Name, timeOnlyValue.Ticks, GetStoreOptions(descriptor)),
            TimeSpan timeSpanValue => new Int64Field(descriptor.Name, timeSpanValue.Ticks, GetStoreOptions(descriptor)),
            _ => null
        };

        return output is not null;
    }

    internal static bool TryGetValue(PropertyDescriptor descriptor, IIndexableField field, [NotNullWhen(returnValue: true)] out object? output) {
        output = Type.GetTypeCode(descriptor.Type) switch {
            TypeCode.Boolean => field.GetInt32ValueOrDefault() > 0,
            TypeCode.Char => field.GetStringValue().First(),
            TypeCode.SByte or TypeCode.Byte => field.GetByteValueOrDefault(),
            TypeCode.Int16 or TypeCode.UInt16 => field.GetInt16ValueOrDefault(),
            TypeCode.Int32 => field.GetInt32ValueOrDefault(),
            TypeCode.UInt32 or TypeCode.Int64 => field.GetInt64ValueOrDefault(),
            TypeCode.UInt64 => Convert.ToUInt64(field.GetDoubleValueOrDefault()),
            TypeCode.Single => field.GetSingleValueOrDefault(),
            TypeCode.Double => field.GetDoubleValueOrDefault(),
            TypeCode.Decimal => Convert.ToDecimal(field.GetDoubleValueOrDefault()),
            TypeCode.DateTime => new DateTime(field.GetInt64ValueOrDefault()),
            TypeCode.String => field.GetStringValue(),
            _ => GetUnknown(descriptor, field)
        };

        return output is not null;
    }

    private static IIndexableField CreateStringField(PropertyDescriptor descriptor, string value) {
        if (descriptor.Options.HasFlag(PropertyOptions.Sanitize)) {
            value = value.RemoveHtmlTags();
        }

        return descriptor.Options.HasFlag(PropertyOptions.Analyze)
            ? new TextField(descriptor.Name, value, GetStoreOptions(descriptor))
            : new StringField(descriptor.Name, value, GetStoreOptions(descriptor));
    }

    private static Field.Store GetStoreOptions(PropertyDescriptor descriptor) {
        return descriptor.Options.HasFlag(PropertyOptions.Store)
            ? Field.Store.YES
            : Field.Store.NO;
    }

    private static object? GetUnknown(PropertyDescriptor descriptor, IIndexableField field) {
        if (descriptor.Type == typeof(Guid)) {
            return Guid.TryParse(field.GetStringValue(), out var guidValue)
                ? guidValue
                : null;
        }

        if (descriptor.Type == typeof(DateOnly)) {
            return DateOnly.FromDateTime(new DateTime(field.GetInt64ValueOrDefault()));
        }

        if (descriptor.Type == typeof(TimeOnly)) {
            return TimeOnly.FromDateTime(new DateTime(field.GetInt64ValueOrDefault()));
        }

        if (descriptor.Type == typeof(TimeSpan)) {
            return TimeSpan.FromTicks(field.GetInt64ValueOrDefault());
        }

        if (descriptor.Type == typeof(DateTimeOffset)) {
            return new DateTimeOffset(field.GetInt64ValueOrDefault(), TimeSpan.Zero);
        }

        if (typeof(Enum).IsAssignableFrom(descriptor.Type)) {
            return Enum.TryParse(descriptor.Type, field.GetStringValue(), out var enumValue)
                ? enumValue
                : null;
        }

        return null;
    }
}