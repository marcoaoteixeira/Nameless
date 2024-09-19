namespace Nameless.Lucene;

[Singleton]
public sealed class EmptySearchHit : ISearchHit {
    public static ISearchHit Instance { get; } = new EmptySearchHit();

    public string DocumentID => string.Empty;

    public float Score => 0F;

    public bool GetBoolean(string fieldName) => false;

    public DateTimeOffset GetDateTimeOffset(string fieldName) => DateTimeOffset.MinValue;

    public double GetDouble(string fieldName) => double.NaN;

    public int GetInt(string fieldName) => -1;

    public string GetString(string fieldName) => string.Empty;

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static EmptySearchHit() { }

    private EmptySearchHit() { }
}