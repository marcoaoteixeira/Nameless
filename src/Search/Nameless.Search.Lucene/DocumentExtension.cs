using Lucene.Net.Documents;
using LuceneDocument = Lucene.Net.Documents.Document;
using LuceneField = Lucene.Net.Documents.Field;

namespace Nameless.Search.Lucene;

/// <summary>
/// <see cref="IDocument"/> extension methods.
/// </summary>
public static class DocumentExtension {
    // *** NOTE ***
    // When creating a field be sure that the corresponding "get"
    // in <see cref="ISearchBit"/> implementation respects the same
    // logic when retrieving it.

    public static LuceneDocument ToDocument(this IDocument self) {
        Prevent.Argument.Null(self);

        var result = new LuceneDocument();
        
        foreach (var item in self) {
            var field = item.Type switch {
                IndexableType.Boolean => CreateBooleanField(item),
                IndexableType.String => CreateStringField(item),
                IndexableType.Byte => CreateByteField(item),
                IndexableType.Short => CreateShortField(item),
                IndexableType.Integer => CreateIntegerField(item),
                IndexableType.Long => CreateLongField(item),
                IndexableType.Float => CreateFloatField(item),
                IndexableType.Double => CreateDoubleField(item),
                IndexableType.DateTimeOffset => CreateDateTimeOffsetField(item),
                IndexableType.DateTime => CreateDateTimeField(item),
                _ => throw new InvalidOperationException("Undefined indexable type")
            };
            result.Add(field);
        }

        return result;
    }

    private static StringField CreateBooleanField(Field field)
        => new(field.Name, (field.Value is true).ToString(), field.GetStoreOption());

    private static LuceneField CreateStringField(Field field) {
        var value = (string)field.Value;

        if (field.Options.HasFlag(FieldOptions.Sanitize)) {
            value = value.RemoveHtmlTags();
        }

        return field.Options.HasFlag(FieldOptions.Analyze)
            ? new TextField(field.Name, value, field.GetStoreOption())
            : new StringField(field.Name, value, field.GetStoreOption());
    }

    private static Int32Field CreateByteField(Field field)
        => new(field.Name, (byte)field.Value, field.GetStoreOption());

    private static Int32Field CreateShortField(Field field)
        => new(field.Name, (short)field.Value, field.GetStoreOption());

    private static Int32Field CreateIntegerField(Field field)
        => new(field.Name, (int)field.Value, field.GetStoreOption());

    private static Int64Field CreateLongField(Field field)
        => new(field.Name, (long)field.Value, field.GetStoreOption());

    private static DoubleField CreateFloatField(Field field)
        => new(field.Name, (float)field.Value, field.GetStoreOption());

    private static DoubleField CreateDoubleField(Field field)
        => new(field.Name, (double)field.Value, field.GetStoreOption());

    private static Int64Field CreateDateTimeOffsetField(Field field)
        => new(field.Name, ((DateTimeOffset)field.Value).ToUnixTimeMilliseconds(), field.GetStoreOption());

    private static Int64Field CreateDateTimeField(Field field)
        => new(field.Name, ((DateTime)field.Value).ToBinary(), field.GetStoreOption());

    private static LuceneField.Store GetStoreOption(this Field self)
        => self.Options.HasFlag(FieldOptions.Store)
            ? LuceneField.Store.YES
            : LuceneField.Store.NO;
}