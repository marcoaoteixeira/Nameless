using Lucene.Net.Documents;
using Nameless.Lucene.Internals;
using Lucene_Field = Lucene.Net.Documents.Field;

namespace Nameless.Lucene;

public static class DocumentExtension {
    private const string DATE_PATTERN = "yyyy-MM-ddTHH:mm:ssZ";

    private static DateTime MinDate => new(1980, 1, 1);

    public static Document ToLuceneDocument(this IDocument self) {
        Prevent.Argument.Null(self);

        var result = new Document();
        foreach (var item in self) {
            var descriptor = Descriptor.Create(item);
            var field = item.Type switch {
                IndexableType.Integer => CreateIntegerField(descriptor),
                IndexableType.Text => CreateTextField(descriptor),
                IndexableType.DateTime => CreateDateTimeField(descriptor),
                IndexableType.Boolean => CreateBooleanField(descriptor),
                IndexableType.Number => CreateNumberField(descriptor),
                _ => throw new InvalidOperationException("Undefined indexable type")
            };
            result.Fields.Add(field);
        }
        return result;
    }

    private static Int32Field CreateIntegerField(Descriptor descriptor)
        => new(descriptor.Name, Convert.ToInt32(descriptor.Value), descriptor.Store);

    private static Lucene_Field CreateTextField(Descriptor descriptor) {
        var value = (string)descriptor.Value;

        if (descriptor.Sanitize) {
            value = value.RemoveHtmlTags();
        }

        return descriptor.Analyze
            ? new TextField(descriptor.Name, value, descriptor.Store)
            : new StringField(descriptor.Name, value, descriptor.Store);
    }

    private static StringField CreateDateTimeField(Descriptor descriptor) {
        var value = descriptor.Value switch {
            DateTimeOffset date => date.ToUniversalTime().ToString(DATE_PATTERN),
            DateTime date => date.ToUniversalTime().ToString(DATE_PATTERN),
            _ => MinDate.ToString(DATE_PATTERN),
        };

        return new StringField(descriptor.Name, value, descriptor.Store);
    }

    private static StringField CreateBooleanField(Descriptor descriptor)
        => new(descriptor.Name, ToBooleanString(descriptor.Value), descriptor.Store);

    private static DoubleField CreateNumberField(Descriptor descriptor)
        => new(descriptor.Name, Convert.ToDouble(descriptor.Value), descriptor.Store);

    private static string ToBooleanString(object obj) {
        var value = obj.ToString();

        // we'll consider null as false.
        if (value is null) {
            return bool.FalseString;
        }

        // any numeric value less than 0 is false.
        if (double.TryParse(value, out var result)) {
            return (result > 0d).ToString();
        }

        // self-explanatory.
        return string.Equals(a: value,
                             b: bool.TrueString,
                             comparisonType: StringComparison.OrdinalIgnoreCase)
                     .ToString();
    }
}