namespace Nameless.Caching {

    public delegate void EvictionCallback(string key, object? value, string? reason);
}
