using System.Reflection;

namespace Nameless.MongoDB;

public sealed class CollectionNamingStrategy : ICollectionNamingStrategy {
    public string GetCollectionName(Type type) {
        Prevent.Argument.Null(type);

        var attr = type.GetCustomAttribute<CollectionNameAttribute>(inherit: false);
        if (attr is not null) {
            return attr.Name;
        }

        return string.Join("_", type.Name.SplitUpperCase())
                     .ToLowerInvariant();
    }
}