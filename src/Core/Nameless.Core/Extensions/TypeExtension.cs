using System.Reflection;

namespace Nameless;

/// <summary>
/// <see cref="Type"/> extension methods.
/// </summary>
public static class TypeExtension {
    private static readonly Type[] WriteTypes = new[] {
        typeof(string),
        typeof(DateTime),
        typeof(Enum),
        typeof(decimal),
        typeof(Guid)
    };

    /// <summary>
    /// Verifies if the <see cref="Type"/> is an instance of <see cref="Nullable"/>.
    /// </summary>
    /// <param name="self">The self type.</param>
    /// <returns><c>true</c>, if is instance of <see cref="Nullable"/>, otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static bool IsNullable(this Type self) {
        Prevent.Argument.Null(self);

        return self.IsGenericType && self.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    /// Can convert to <see cref="Nullable"/> type.
    /// </summary>
    /// <param name="self">The self type.</param>
    /// <returns><c>true</c>, if it can convert, otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static bool AllowNull(this Type self) {
        Prevent.Argument.Null(self);

        return !self.IsValueType || self.IsNullable();
    }

    /// <summary>
    /// Verifies if the <paramref name="self"/> is a simple type.
    /// </summary>
    /// <param name="self">The self type.</param>
    /// <returns><c>true</c> if is simple type; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static bool IsSimple(this Type self) {
        Prevent.Argument.Null(self);

        return self.IsPrimitive || WriteTypes.Contains(self);
    }

    /// <summary>
    /// Checks if the current open generic type is assignable from the <paramref name="type"/>.
    /// </summary>
    /// <param name="self">The current open generic type.</param>
    /// <param name="type">The assignable from type.</param>
    /// <returns><c>true</c> if assignable; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static bool IsAssignableFromOpenGenericType(this Type self, Type? type) {
        Prevent.Argument.Null(self);

        while (true) {
            if (type is null) { return false; }

            // Check if any interface has the same open generic definition.
            var assignable = type.GetInterfaces()
                                 .Any(Assignable);
            if (assignable) { return true; }

            // Check if any class has the same open generic definition.
            if (Assignable(type)) { return true; }

            type = type.BaseType;
        }

        bool Assignable(Type current) {
            return current.IsGenericType &&
                   current.GetGenericTypeDefinition() == self;
        }
    }

    /// <summary>
    /// Checks if the current type implements the specified interface.
    /// </summary>
    /// <typeparam name="TInterface">Type of the interface.</typeparam>
    /// <param name="self">The current type.</param>
    /// <returns><c>true</c> if implements; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static bool HasInterface<TInterface>(this Type self)
        where TInterface : class
        => HasInterface(self, typeof(TInterface));

    /// <summary>
    /// Checks if the current type implements the specified interface defined by <paramref name="interfaceType"/>.
    /// </summary>
    /// <param name="self">The current type.</param>
    /// <param name="interfaceType">The interface type.</param>
    /// <returns><c>true</c> if implements; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> or
    /// <paramref name="interfaceType"/> is <c>null</c>.
    /// </exception>
    public static bool HasInterface(this Type self, Type interfaceType) {
        Prevent.Argument.Null(self);
        Prevent.Argument.Null(interfaceType);

        return self.GetInterfaces()
                   .Any(type => interfaceType.IsAssignableFrom(type) ||
                                interfaceType.IsAssignableFromOpenGenericType(type));
    }

    /// <summary>
    /// Checks if type has the specified attribute.
    /// </summary>
    /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
    /// <param name="self">The current type instance.</param>
    /// <param name="inherit">Whether the attribute is inherited.</param>
    /// <returns><c>true</c> if the type has the attribute; otherwise <c>false</c>.</returns>
    public static bool HasAttribute<TAttribute>(this Type self, bool inherit = false)
        where TAttribute : Attribute
        => HasAttribute(self, typeof(TAttribute), inherit);

    /// <summary>
    /// Checks if type has the specified attribute.
    /// </summary>
    /// <param name="self">The current type instance.</param>
    /// <param name="attributeType">Type of the attribute.</param>
    /// <param name="inherit">Whether the attribute is inherited.</param>
    /// <returns><c>true</c> if the type has the attribute; otherwise <c>false</c>.</returns>
    public static bool HasAttribute(this Type self, Type attributeType, bool inherit = false) {
        Prevent.Argument.Null(self);
        Prevent.Argument.Null(attributeType);

        return self.GetCustomAttribute(attributeType, inherit) is not null;
    }

    /// <summary>
    /// Checks if the type has a parameterless constructor.
    /// </summary>
    /// <param name="self">The current type instance.</param>
    /// <returns><c>true</c> if the type has a parameterless constructor; otherwise <c>false</c>.</returns>
    public static bool HasParameterlessConstructor(this Type self)
        => Prevent.Argument
                  .Null(self)
                  .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                  .Any(constructor => constructor.GetParameters().Length == 0);
}