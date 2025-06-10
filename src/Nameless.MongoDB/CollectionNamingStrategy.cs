using System.Reflection;

namespace Nameless.MongoDB;

/// <summary>
/// Default implementation of <see cref="ICollectionNamingStrategy"/>.
/// </summary>
public sealed class CollectionNamingStrategy : ICollectionNamingStrategy {
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="type"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// This method retrieves the name of the MongoDB collection in two ways:
    /// The first is by checking for a <see cref="CollectionNameAttribute"/> on the type,
    /// this attribute will provide the name of the collection. If the attribute is not present,
    /// The second way is to generate a name by splitting the type name into words,
    /// converting them to lower case separated by underline, e.g.: UsersInRoles => users_in_roles,
    /// </remarks>
    public string GetCollectionName(Type type) {
        Prevent.Argument.Null(type);

        var attr = type.GetCustomAttribute<CollectionNameAttribute>(false);
        if (attr is not null) {
            return attr.Name;
        }

        return string.Join("_", type.Name.SplitUpperCase())
                     .ToLowerInvariant();
    }
}