namespace Nameless.Search;

public sealed class EmptySearchHit : ISearchHit {
    public string DocumentID
        => string.Empty;

    public float Score
        => 0F;

    public bool? GetBoolean(string fieldName) {
        return null;
    }

    public string? GetString(string fieldName) {
        return null;
    }

    public byte? GetByte(string fieldName) {
        return null;
    }

    public short? GetShort(string fieldName) {
        return null;
    }

    public int? GetInteger(string fieldName) {
        return null;
    }

    public long? GetLong(string fieldName) {
        return null;
    }

    public float? GetFloat(string fieldName) {
        return null;
    }

    public double? GetDouble(string fieldName) {
        return null;
    }

    public DateTimeOffset? GetDateTimeOffset(string fieldName) {
        return null;
    }

    public DateTime? GetDateTime(string fieldName) {
        return null;
    }
}