namespace Nameless.Lucene;

public sealed class EmptySearchHit : ISearchHit {
    public string DocumentID => string.Empty;

    public float Score => 0F;

    public bool GetBoolean(string fieldName) => false;

    public DateTimeOffset GetDateTimeOffset(string fieldName) => DateTimeOffset.MinValue;

    public double GetDouble(string fieldName) => double.NaN;

    public int GetInt(string fieldName) => -1;

    public string GetString(string fieldName) => string.Empty;
}