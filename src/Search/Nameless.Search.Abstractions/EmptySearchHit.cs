namespace Nameless.Search;

public sealed class EmptySearchHit : ISearchHit {
    public string DocumentID
        => string.Empty;

    public float Score
        => 0F;

    public bool? GetBoolean(string fieldName)
        => null;

    public DateTimeOffset? GetDateTimeOffset(string fieldName)
        => null;

    public double? GetDouble(string fieldName)
        => null;

    public int? GetInt(string fieldName)
        => null;

    public string? GetString(string fieldName)
        => null;
}