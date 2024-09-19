namespace Nameless.MongoDB;

public static class CollectionNamingStrategyExtension {
    public static string GetCollectionName<T>(this ICollectionNamingStrategy self)
        => Prevent.Argument
                  .Null(self, nameof(self))
                  .GetCollectionName(typeof(T));
}