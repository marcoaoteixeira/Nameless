namespace Nameless.MongoDB;

public static class CollectionNamingStrategyExtension {
    public static string GetCollectionName<T>(this ICollectionNamingStrategy self)
        => Prevent.Argument
                  .Null(self)
                  .GetCollectionName(typeof(T));
}