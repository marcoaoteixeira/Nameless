using System.Reflection;

namespace Nameless.MongoDB.Impl;

[Singleton]
public sealed class CollectionNamingStrategy : ICollectionNamingStrategy {
    /// <summary>
    /// Gets the unique instance of <see cref="CollectionNamingStrategy"/>.
    /// </summary>
    public static ICollectionNamingStrategy Instance { get; } = new CollectionNamingStrategy();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static CollectionNamingStrategy() { }

    // Prevents the class from being constructed.
    private CollectionNamingStrategy() { }

    public string GetCollectionName(Type type) {
        Prevent.Argument.Null(type);

        var attr = type.GetCustomAttribute<CollectionNameAttribute>(inherit: false);
        if (attr is not null) {
            return attr.Name;
        }

        return string.Join(separator: Constants.Separators.UNDERSCORE,
                           type.Name.SplitUpperCase())
                     .ToLower();
    }
}