using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Nameless;

/// <summary>
/// Classes marked with <see cref="SingletonAttribute"/> were expected to
/// have a static property that will provide the current instance of the
/// class. See <a href="https://en.wikipedia.org/wiki/Singleton_pattern">Singleton Pattern on Wikipedia</a>
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SingletonAttribute : Attribute {
    /// <summary>
    /// Gets the default accessor name for the singleton class.
    /// </summary>
    public static readonly string DefaultAccessorName = "Instance";

    private string _accessorName = DefaultAccessorName;

    /// <summary>
    /// Gets or sets the name of the property that will be used to get
    /// the singleton instance of the annotated class.
    /// <br /><br />
    /// <strong>Note:</strong> if no accessor name is provided, that it will use
    /// the <see cref="DefaultAccessorName"/>.
    /// This rule applies to <c>null</c> or <see cref="string.Empty"/>.
    /// </summary>
    public string AccessorName {
        get => _accessorName;
        set => _accessorName = value.WithFallback(DefaultAccessorName);
    }

    /// <summary>
    /// Retrieves the singleton instance of the type.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    /// <returns>A singleton instance of the type.</returns>
    public static T? GetInstance<T>()
        where T : class
        => GetInstance(typeof(T)) as T;

    /// <summary>
    /// Retrieves the singlet instance of the type through its accessor property.
    /// </summary>
    /// <param name="type">The type</param>
    /// <returns>
    /// A singleton instance of the type, or <c>null</c> if <paramref name="type"/> is <c>null</c>
    /// or the type doesn't have the <see cref="SingletonAttribute"/> applied
    /// or doesn't have a defined single instance accessor property.
    /// .</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="type"/> is <c>null</c>.
    /// </exception>
    public static object? GetInstance(Type type) {
        Prevent.Argument.Null(type, nameof(type));

        if (!TryGetAttribute(type, out var attr)) { return null; }

        return TryGetAccessorProperty(type, attr.AccessorName, out var accessor)
            ? accessor.GetValue(obj: null /* static instance */)
            : null;
    }

    private static bool TryGetAttribute(MemberInfo type, [NotNullWhen(true)] out SingletonAttribute? attr) {
        attr = type.GetCustomAttribute<SingletonAttribute>(inherit: false);
        return attr is not null;
    }

    private static bool TryGetAccessorProperty(Type type, string accessorName, [NotNullWhen(true)] out PropertyInfo? accessor) {
        var currentAccessor = string.IsNullOrWhiteSpace(accessorName)
            ? DefaultAccessorName
            : accessorName;
        accessor = type.GetProperty(name: currentAccessor,
                                    bindingAttr: BindingFlags.Public | BindingFlags.Static);
        return accessor is not null;
    }
}