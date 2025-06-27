namespace Nameless.Identity.Web;

public readonly record struct OutputCachePolicy(string PolicyName, TimeSpan Expiration) {
    public static readonly OutputCachePolicy OpenApi = new(nameof(OpenApi), TimeSpan.FromDays(7));

    public static readonly OutputCachePolicy OneMinute = new(nameof(OneMinute), TimeSpan.FromMinutes(1));
}
