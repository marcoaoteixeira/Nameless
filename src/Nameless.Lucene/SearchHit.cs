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
    public bool? GetBoolean(string name) {
        // Boolean values are stored as integer (1 = true, 0 = false)
        return GetInteger(name) is 1;
    }

    /// <inheritdoc />
    public string? GetString(string name) {
        return GetField(name).GetStringValue();
    }

    /// <inheritdoc />
    public byte? GetByte(string name) {
        return GetField(name).GetByteValue();
    }

    /// <inheritdoc />
    public short? GetShort(string name) {
        return GetField(name).GetInt16Value();
    }

    /// <inheritdoc />
    public int? GetInteger(string name) {
        return GetField(name).GetInt32Value();
    }

    /// <inheritdoc />
    public long? GetLong(string name) {
        return GetField(name).GetInt64Value();
    }

    /// <inheritdoc />
    public float? GetFloat(string name) {
        return GetField(name).GetSingleValue();
    }

    /// <inheritdoc />
    public double? GetDouble(string name) {
        return GetField(name).GetDoubleValue();
    }

    /// <inheritdoc />
    public DateTimeOffset? GetDateTimeOffset(string name) {
        // DateTimeOffset values are stored as ticks
        var value = GetLong(name);

        return value.HasValue
            ? new DateTimeOffset(value.Value, TimeSpan.Zero)
            : null;
    }

    /// <inheritdoc />
    public DateTime? GetDateTime(string name) {
        // DateTime values are stored as ticks
        var value = GetLong(name);

        return value.HasValue
            ? new DateTime(value.Value, DateTimeKind.Utc)
            : null;
    }

    /// <inheritdoc />
    public DateOnly? GetDateOnly(string name) {
        // DateOnly values are stored as ticks
        var value = GetLong(name);

        if (!value.HasValue) { return null; }

        var dateTime = new DateTime(value.Value, DateTimeKind.Utc);

        return DateOnly.FromDateTime(dateTime);
    }

    /// <inheritdoc />
    public TimeOnly? GetTimeOnly(string name) {
        // TimeOnly values are stored as ticks
        var value = GetLong(name);

        if (!value.HasValue) { return null; }

        var timeSpan = TimeSpan.FromTicks(value.Value);

        return TimeOnly.FromTimeSpan(timeSpan);
    }

    /// <inheritdoc />
    public TimeSpan? GetTimeSpan(string name) {
        // TimeSpan values are stored as ticks
        var value = GetLong(name);

        return value.HasValue
            ? TimeSpan.FromTicks(value.Value)
            : null;
    }

    /// <inheritdoc />
    public TEnum? GetEnum<TEnum>(string name)
        where TEnum : struct, Enum {
        // Enum values are stored as strings
        var value = GetString(name);

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
