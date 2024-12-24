namespace Nameless.Search;

public sealed class EmptySearchHit : ISearchHit {
    public string DocumentID
        => string.Empty;

    public float Score
        => 0F;

    public bool? GetBoolean(string fieldName)
        => null;

    public string? GetString(string fieldName)
        => null;

    public byte? GetByte(string fieldName)
        => null;

    public short? GetShort(string fieldName)
        => null;

    public int? GetInteger(string fieldName)
        => null;
    
    public long? GetLong(string fieldName)
        => null;

    public float? GetFloat(string fieldName)
        => null;

    public double? GetDouble(string fieldName)
        => null;

    public DateTimeOffset? GetDateTimeOffset(string fieldName)
        => null;

    public DateTime? GetDateTime(string fieldName)
        => null;
}