using Lucene.Net.Index;
using Nameless.Lucene.Empty;

namespace Nameless.Lucene;

/// <summary>
///     Default implementation of <see cref="ISearchHit" />.
/// </summary>
public sealed class SearchHit : ISearchHit {
    private readonly LuceneDocument _document;

    private readonly Lazy<string> _documentID;

    /// <inheritdoc />
    public string DocumentID => _documentID.Value;

    /// <inheritdoc />
    public float Score { get; }

    public SearchHit(LuceneDocument document, float score = 0F) {
        _document = Guard.Against.Null(document);
        _documentID = new Lazy<string>(GetDocumentID);

        Score = score;
    }

    /// <inheritdoc />
    public bool? GetBoolean(string fieldName) {
        // Boolean values are stored as integer (1 = true, 0 = false)
        return GetInteger(fieldName) is 1;
    }

    /// <inheritdoc />
    public string? GetString(string fieldName) {
        return GetField(fieldName).GetStringValue();
    }

    /// <inheritdoc />
    public byte? GetByte(string fieldName) {
        return GetField(fieldName).GetByteValue();
    }

    /// <inheritdoc />
    public short? GetShort(string fieldName) {
        return GetField(fieldName).GetInt16Value();
    }

    /// <inheritdoc />
    public int? GetInteger(string fieldName) {
        return GetField(fieldName).GetInt32Value();
    }

    /// <inheritdoc />
    public long? GetLong(string fieldName) {
        return GetField(fieldName).GetInt64Value();
    }

    /// <inheritdoc />
    public float? GetFloat(string fieldName) {
        return GetField(fieldName).GetSingleValue();
    }

    /// <inheritdoc />
    public double? GetDouble(string fieldName) {
        return GetField(fieldName).GetDoubleValue();
    }

    /// <inheritdoc />
    public DateTimeOffset? GetDateTimeOffset(string fieldName) {
        // DateTimeOffset values are stored as ticks
        var value = GetLong(fieldName);

        return value.HasValue
            ? new DateTimeOffset(value.Value, TimeSpan.Zero)
            : null;
    }

    /// <inheritdoc />
    public DateTime? GetDateTime(string fieldName) {
        // DateTime values are stored as ticks
        var value = GetLong(fieldName);

        return value.HasValue
            ? new DateTime(value.Value, DateTimeKind.Utc)
            : null;
    }

    /// <inheritdoc />
    public DateOnly? GetDateOnly(string fieldName) {
        // DateOnly values are stored as ticks
        var value = GetLong(fieldName);

        if (!value.HasValue) { return null; }

        var dateTime = new DateTime(value.Value, DateTimeKind.Utc);

        return DateOnly.FromDateTime(dateTime);
    }

    /// <inheritdoc />
    public TimeOnly? GetTimeOnly(string fieldName) {
        // TimeOnly values are stored as ticks
        var value = GetLong(fieldName);

        if (!value.HasValue) { return null; }

        var timeSpan = TimeSpan.FromTicks(value.Value);

        return TimeOnly.FromTimeSpan(timeSpan);
    }

    /// <inheritdoc />
    public TimeSpan? GetTimeSpan(string fieldName) {
        // TimeSpan values are stored as ticks
        var value = GetLong(fieldName);

        return value.HasValue
            ? TimeSpan.FromTicks(value.Value)
            : null;
    }

    /// <inheritdoc />
    public TEnum? GetEnum<TEnum>(string fieldName)
        where TEnum : struct, Enum {
        // Enum values are stored as strings
        var value = GetString(fieldName);

        return value is not null && Enum.TryParse<TEnum>(value, out var result)
            ? result
            : null;
    }

    private string GetDocumentID() {
        return GetString(Document.RESERVED_ID_NAME)
               ?? throw new InvalidOperationException("Missing document ID.");
    }

    private IIndexableField GetField(string name) {
        return _document.GetField(name) ?? EmptyIndexableField.Instance;
    }
}
