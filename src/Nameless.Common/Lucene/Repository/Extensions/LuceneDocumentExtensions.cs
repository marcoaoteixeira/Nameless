using System.Diagnostics.CodeAnalysis;
using Lucene.Net.Documents.Extensions;
using Lucene.Net.Index;
using Nameless.Lucene.Repository.Mappings;

namespace Nameless.Lucene.Repository;

internal static class LuceneDocumentExtensions {
    extension(IIndexableField self) {
        internal bool TryGetValue(PropertyDescriptor property, [NotNullWhen(returnValue: true)] out object? output) {
            output = Type.GetTypeCode(property.Type) switch {
                TypeCode.Boolean => self.GetInt32ValueOrDefault() > 0,
                TypeCode.Char => self.GetStringValue().First(),
                TypeCode.SByte or TypeCode.Byte => self.GetByteValueOrDefault(),
                TypeCode.Int16 or TypeCode.UInt16 => self.GetInt16ValueOrDefault(),
                TypeCode.Int32 => self.GetInt32ValueOrDefault(),
                TypeCode.UInt32 or TypeCode.Int64 => self.GetInt64ValueOrDefault(),
                TypeCode.UInt64 => Convert.ToUInt64(self.GetDoubleValueOrDefault()),
                TypeCode.Single => self.GetSingleValueOrDefault(),
                TypeCode.Double => self.GetDoubleValueOrDefault(),
                TypeCode.Decimal => Convert.ToDecimal(self.GetDoubleValueOrDefault()),
                TypeCode.DateTime => new DateTime(self.GetInt64ValueOrDefault()),
                TypeCode.String => self.GetStringValue(),
                _ => GetUnknown(property, self)
            };

            return output is not null;
        }
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
