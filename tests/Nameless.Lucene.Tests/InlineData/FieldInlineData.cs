namespace Nameless.Lucene.InlineData;

public sealed class FieldInlineData : TheoryData<string, object, IndexableType, FieldOptions> {
    public FieldInlineData() {
        Add($"{nameof(IndexableType.Boolean)}_MIN", false, IndexableType.Boolean, FieldOptions.None);
        Add($"{nameof(IndexableType.String)}_MIN", string.Empty, IndexableType.String, FieldOptions.None);
        Add($"{nameof(IndexableType.Byte)}_MIN", byte.MinValue, IndexableType.Byte, FieldOptions.None);
        Add($"{nameof(IndexableType.Integer)}_MIN", int.MaxValue, IndexableType.Integer, FieldOptions.None);
        Add($"{nameof(IndexableType.Short)}_MIN", short.MinValue, IndexableType.Short, FieldOptions.None);
        Add($"{nameof(IndexableType.Long)}_MIN", long.MinValue, IndexableType.Long, FieldOptions.None);
        Add($"{nameof(IndexableType.Float)}_MIN", float.MinValue, IndexableType.Float, FieldOptions.None);
        Add($"{nameof(IndexableType.Double)}_MIN", double.MinValue, IndexableType.Double, FieldOptions.None);
        Add($"{nameof(IndexableType.DateTimeOffset)}_MIN", DateTimeOffset.MinValue.ToUniversalTime(), IndexableType.DateTimeOffset, FieldOptions.None);
        Add($"{nameof(IndexableType.DateTime)}_MIN", DateTime.MinValue.ToUniversalTime(), IndexableType.DateTime, FieldOptions.None);
        Add($"{nameof(IndexableType.DateOnly)}_MIN", DateOnly.MinValue, IndexableType.DateOnly, FieldOptions.None);
        Add($"{nameof(IndexableType.TimeOnly)}_MIN", TimeOnly.MinValue, IndexableType.TimeOnly, FieldOptions.None);
        Add($"{nameof(IndexableType.TimeSpan)}_MIN", TimeSpan.MinValue, IndexableType.TimeSpan, FieldOptions.None);
        Add($"{nameof(IndexableType.Enum)}_MIN", DayOfWeek.Monday, IndexableType.Enum, FieldOptions.None);

        Add($"{nameof(IndexableType.Boolean)}_MAX", true, IndexableType.Boolean, FieldOptions.None);
        Add($"{nameof(IndexableType.String)}_MAX", nameof(String), IndexableType.String, FieldOptions.None);
        Add($"{nameof(IndexableType.Byte)}_MAX", byte.MaxValue, IndexableType.Byte, FieldOptions.None);
        Add($"{nameof(IndexableType.Short)}_MAX", short.MaxValue, IndexableType.Short, FieldOptions.None);
        Add($"{nameof(IndexableType.Integer)}_MAX", int.MinValue, IndexableType.Integer, FieldOptions.None);
        Add($"{nameof(IndexableType.Long)}_MAX", long.MaxValue, IndexableType.Long, FieldOptions.None);
        Add($"{nameof(IndexableType.Float)}_MAX", float.MaxValue, IndexableType.Float, FieldOptions.None);
        Add($"{nameof(IndexableType.Double)}_MAX", double.MaxValue, IndexableType.Double, FieldOptions.None);
        Add($"{nameof(IndexableType.DateTimeOffset)}_MAX", DateTimeOffset.MaxValue.ToUniversalTime(), IndexableType.DateTimeOffset, FieldOptions.None);
        Add($"{nameof(IndexableType.DateTime)}_MAX", DateTime.MaxValue.ToUniversalTime(), IndexableType.DateTime, FieldOptions.None);
        Add($"{nameof(IndexableType.DateOnly)}_MAX", DateOnly.MaxValue, IndexableType.DateOnly, FieldOptions.None);
        Add($"{nameof(IndexableType.TimeOnly)}_MAX", TimeOnly.MaxValue, IndexableType.TimeOnly, FieldOptions.None);
        Add($"{nameof(IndexableType.TimeSpan)}_MAX", TimeSpan.MaxValue, IndexableType.TimeSpan, FieldOptions.None);
        Add($"{nameof(IndexableType.Enum)}_MAX", DayOfWeek.Sunday, IndexableType.Enum, FieldOptions.None);

        Add($"{nameof(IndexableType.Enum)}_FLAG", FieldOptions.Store | FieldOptions.Sanitize, IndexableType.Enum, FieldOptions.None);
    }
}
