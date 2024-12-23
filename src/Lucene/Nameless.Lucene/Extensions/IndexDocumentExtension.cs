using Lucene.Net.Documents;
using Nameless.Lucene.Internals;
using Lucene_Field = Lucene.Net.Documents.Field;

namespace Nameless.Lucene;

public static class IndexDocumentExtension {
    private const string DATE_PATTERN = "yyyy-MM-ddTHH:mm:ssZ";

    private static DateTime MinDate => new(1980, 1, 1);

    public static Document ToDocument(this IIndexDocument self) {
        Prevent.Argument.Null(self);

        var result = new Document();
        foreach (var item in self) {
            if (item.Value is null) {
                continue;
            }

            var descriptor = FieldDescriptor.Create(item);
            var field = item.Type switch {
                IndexableType.Integer => CreateIntegerField(descriptor),
                IndexableType.Text => CreateTextField(descriptor),
                IndexableType.DateTime => CreateDateTimeField(descriptor),
                IndexableType.Boolean => CreateBooleanField(descriptor),
                IndexableType.Number => CreateNumberField(descriptor),
                _ => throw new InvalidOperationException("Undefined indexable type")
            };
            result.Add(field);
        }
        return result;
    }

    private static Int32Field CreateIntegerField(FieldDescriptor fieldDescriptor)
        => new(fieldDescriptor.Name, Convert.ToInt32(fieldDescriptor.Value), fieldDescriptor.Store);

    private static Lucene_Field CreateTextField(FieldDescriptor fieldDescriptor) {
        var value = (string)fieldDescriptor.Value;

        if (fieldDescriptor.Sanitize) {
            value = value.RemoveHtmlTags();
        }

        return fieldDescriptor.Analyze
            ? new TextField(fieldDescriptor.Name, value, fieldDescriptor.Store)
            : new StringField(fieldDescriptor.Name, value, fieldDescriptor.Store);
    }

    private static StringField CreateDateTimeField(FieldDescriptor fieldDescriptor) {
        var value = fieldDescriptor.Value switch {
            DateTimeOffset date => date.ToUniversalTime().ToString(DATE_PATTERN),
            DateTime date => date.ToUniversalTime().ToString(DATE_PATTERN),
            _ => MinDate.ToString(DATE_PATTERN),
        };

        return new StringField(fieldDescriptor.Name, value, fieldDescriptor.Store);
    }

    private static StringField CreateBooleanField(FieldDescriptor fieldDescriptor)
        => new(fieldDescriptor.Name, ToBooleanString(fieldDescriptor.Value), fieldDescriptor.Store);

    private static DoubleField CreateNumberField(FieldDescriptor fieldDescriptor)
        => new(fieldDescriptor.Name, Convert.ToDouble(fieldDescriptor.Value), fieldDescriptor.Store);

    private static string ToBooleanString(object obj) {
        var value = obj.ToString();

        // we'll consider null as false.
        if (value is null) {
            return bool.FalseString;
        }

        // any numeric value less than 0 is false.
        if (double.TryParse(value, out var result)) {
            return (result > 0D).ToString();
        }

        // self-explanatory.
        return string.Equals(a: value,
                             b: bool.TrueString,
                             comparisonType: StringComparison.OrdinalIgnoreCase)
                     .ToString();
    }
}