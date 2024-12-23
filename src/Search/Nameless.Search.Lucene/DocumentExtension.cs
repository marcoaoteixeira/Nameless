using Lucene.Net.Documents;
using LuceneDocument = Lucene.Net.Documents.Document;
using LuceneField = Lucene.Net.Documents.Field;

namespace Nameless.Search.Lucene;

public static class DocumentExtension {
    private const string DATE_PATTERN = "yyyy-MM-ddTHH:mm:ssZ";

    private static DateTime MinDate => new(1980, 1, 1);

    public static LuceneDocument ToDocument(this IDocument self) {
        Prevent.Argument.Null(self);

        var result = new LuceneDocument();
        
        foreach (var item in self) {
            var field = item.Type switch {
                IndexableType.Text => CreateTextField(item),
                IndexableType.Integer => CreateIntegerField(item),
                IndexableType.DateTime => CreateDateTimeField(item),
                IndexableType.Boolean => CreateBooleanField(item),
                IndexableType.Number => CreateNumberField(item),
                _ => throw new InvalidOperationException("Undefined indexable type")
            };
            result.Add(field);
        }

        return result;
    }

    private static LuceneField CreateTextField(Field field) {
        var value = (string)field.Value;

        if (field.Options.HasFlag(FieldOptions.Sanitize)) {
            value = value.RemoveHtmlTags();
        }

        return field.Options.HasFlag(FieldOptions.Analyze)
            ? new TextField(field.Name, value, field.GetStoreOption())
            : new StringField(field.Name, value, field.GetStoreOption());
    }

    private static Int32Field CreateIntegerField(Field field)
        => new(field.Name, (int)field.Value, field.GetStoreOption());

    private static StringField CreateDateTimeField(Field field) {
        var value = field.Value switch {
            DateTimeOffset date => date.ToUniversalTime().ToString(DATE_PATTERN),
            DateTime date => date.ToUniversalTime().ToString(DATE_PATTERN),
            _ => MinDate.ToString(DATE_PATTERN),
        };

        return new StringField(field.Name, value, field.GetStoreOption());
    }

    private static StringField CreateBooleanField(Field field)
        => new(field.Name, field.Value.ToString(), field.GetStoreOption());

    private static DoubleField CreateNumberField(Field field)
        => new(field.Name, (double)field.Value, field.GetStoreOption());

    private static LuceneField.Store GetStoreOption(this Field self)
        => self.Options.HasFlag(FieldOptions.Store)
            ? LuceneField.Store.YES
            : LuceneField.Store.NO;
}