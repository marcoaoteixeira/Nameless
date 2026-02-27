using System.Reflection;

namespace Nameless.Web.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PolicyNameAttribute : Attribute {
    /// <summary>
    ///     Gets the annotated type configuration section name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="PolicyNameAttribute"/> class.
    /// </summary>
    /// <param name="name">
    ///     The name of the configuration section.
    /// </param>
    public PolicyNameAttribute(string name) {
        Name = Throws.When.NullOrWhiteSpace(name);
    }

    /// <summary>
    ///     Retrieves the configuration section name for a
    ///     type.
    /// </summary>
    /// <remarks>
    ///     If the type <typeparamref name="TType"/> is not annotated with the
    ///     <see cref="PolicyNameAttribute"/>, it returns the name
    ///     of the type.
    /// </remarks>
    /// <typeparam name="TType">
    ///     The type.
    /// </typeparam>
    /// <returns>
    ///     The configuration section name.
    /// </returns>
    public static string GetName<TType>() {
        return GetName(typeof(TType));
    }

    /// <summary>
    ///     Retrieves the configuration section name for a
    ///     type.
    /// </summary>
    /// <remarks>
    ///     If the <paramref name="type "/> is not annotated with the
    ///     <see cref="PolicyNameAttribute"/>, it returns the name
    ///     of the type.
    /// </remarks>
    /// <param name="type">
    ///     The type.
    /// </param>
    /// <returns>
    ///     The configuration section name.
    /// </returns>
    public static string GetName(Type type) {
        var attr = type.GetCustomAttribute<PolicyNameAttribute>();

        return attr?.Name ?? type.Name;
    }
}
