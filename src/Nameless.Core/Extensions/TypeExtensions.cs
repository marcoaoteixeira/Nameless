using System.Reflection;
using System.Text;

namespace Nameless;

/// <summary>
///     <see cref="Type" /> extension methods.
/// </summary>
public static class TypeExtensions {
    /// <summary>
    ///     Checks if the <see cref="Type" /> is a concrete class.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="Type" /> instance.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the type is a concrete class;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsConcrete(this Type self) {
        return self is {
            IsClass: true,
            IsAbstract: false,
            IsInterface: false,
            IsGenericTypeDefinition: false
        };
    }

    /// <summary>
    ///     Verifies if the <see cref="Type" /> is an instance of <see cref="Nullable" />.
    /// </summary>
    /// <param name="self">The self type.</param>
    /// <returns><see langword="true"/>, if is instance of <see cref="Nullable" />, otherwise, <see langword="false"/>.</returns>
    public static bool IsNullable(this Type self) {
        return self.IsGenericType && self.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    ///     Can convert to <see cref="Nullable" /> type.
    /// </summary>
    /// <param name="self">The self type.</param>
    /// <returns><see langword="true"/>, if it can convert, otherwise, <see langword="false"/>.</returns>
    public static bool AllowNull(this Type self) {
        return !self.IsValueType || self.IsNullable();
    }

    /// <summary>
    ///     Checks if the current open generic type is assignable from the <paramref name="type" />.
    /// </summary>
    /// <param name="self">The current generic type.</param>
    /// <param name="type">The assignable from type.</param>
    /// <returns><see langword="true"/> if assignable; otherwise <see langword="false"/>.</returns>
    public static bool IsAssignableFromGenericType(this Type self, Type? type) {
        if (!self.IsGenericType) { return false; }

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
                   current.GetGenericTypeDefinition() == self.GetGenericTypeDefinition();
        }
    }

    /// <summary>
    ///     Checks if the current type implements the specified interface.
    /// </summary>
    /// <typeparam name="TInterface">Type of the interface.</typeparam>
    /// <param name="self">The current type.</param>
    /// <returns><see langword="true"/> if implements; otherwise <see langword="false"/>.</returns>
    public static bool HasInterface<TInterface>(this Type self)
        where TInterface : class {
        return self.HasInterface(typeof(TInterface));
    }

    /// <summary>
    ///     Checks if the current type implements the specified interface defined by <paramref name="interfaceType" />.
    /// </summary>
    /// <param name="self">The current type.</param>
    /// <param name="interfaceType">The interface type.</param>
    /// <returns><see langword="true"/> if implements; otherwise <see langword="false"/>.</returns>
    public static bool HasInterface(this Type self, Type interfaceType) {
        return self.GetInterfaces()
                   .Any(type => interfaceType.IsAssignableFrom(type) || interfaceType.IsAssignableFromGenericType(type));
    }

    /// <summary>
    ///     Checks if type has the specified attribute.
    /// </summary>
    /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
    /// <param name="self">The current type instance.</param>
    /// <param name="inherit">Whether the attribute is inherited.</param>
    /// <returns><see langword="true"/> if the type has the attribute; otherwise <see langword="false"/>.</returns>
    public static bool HasAttribute<TAttribute>(this Type self, bool inherit = false)
        where TAttribute : Attribute {
        return self.HasAttribute(typeof(TAttribute), inherit);
    }

    /// <summary>
    ///     Checks if type has the specified attribute.
    /// </summary>
    /// <param name="self">The current type instance.</param>
    /// <param name="attributeType">Type of the attribute.</param>
    /// <param name="inherit">Whether the attribute is inherited.</param>
    /// <returns><see langword="true"/> if the type has the attribute; otherwise <see langword="false"/>.</returns>
    public static bool HasAttribute(this Type self, Type attributeType, bool inherit = false) {
        return self.GetCustomAttribute(attributeType, inherit) is not null;
    }

    /// <summary>
    ///     Checks if the type has a parameterless constructor.
    /// </summary>
    /// <param name="self">The current type instance.</param>
    /// <returns><see langword="true"/> if the type has a parameterless constructor; otherwise <see langword="false"/>.</returns>
    public static bool HasParameterlessConstructor(this Type self) {
        return self.GetConstructor(Type.EmptyTypes) is not null || (self.IsValueType && !self.IsNullable());
    }

    /// <summary>
    ///     Gets the pretty name of the type.
    /// </summary>
    /// <param name="self">The current <see cref="Type" /> instance.</param>
    /// <returns>
    ///     The pretty name of the type.
    /// </returns>
    /// <remarks>
    ///     This method is used to get the pretty name of the type, which is
    ///     useful for logging and debugging purposes. When you call the
    ///     property Name of a type and the type is generic, it will return
    ///     something like List`1 instead of List&lt;int&gt;. This method will
    ///     return List&lt;int&gt; instead. Or if the type is just the template
    ///     it will return List&lt;&gt;.
    /// </remarks>
    public static string GetPrettyName(this Type self) {
        if (!self.IsGenericType) {
            return self.Name;
        }

        var arguments = self.GetGenericArguments();
        var baseName = self.Name[..self.Name.IndexOf('`', StringComparison.Ordinal)];

        var sb = new StringBuilder();
        sb.Append(baseName);
        sb.Append('<');
        for (var index = 0; index < arguments.Length; index++) {
            if (index > 0) { sb.Append(','); }

            // gets the next argument, which can be a generic type
            sb.Append(GetPrettyName(arguments[index]));
        }

        sb.Append('>');

        return sb.ToString();
    }

    /// <summary>
    /// Determines whether the specified <see cref="Type"/> can be instantiated.
    /// </summary>
    /// <remarks>
    /// This method checks several conditions to determine if a type can be
    /// instantiated: it must not be an open generic type, a pointer type,
    /// or a by-ref type; it must not be abstract; it must be public; and it
    /// must have a parameterless constructor.
    /// </remarks>
    /// <param name="self">The <see cref="Type"/> to evaluate.</param>
    /// <returns>
    /// <see langword="true"/> if the type is instantiable; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public static bool CanInstantiate(this Type self) {
        return self is {
            // Exclude open generic types
            IsGenericTypeDefinition: false,

            // Exclude pointer types
            IsPointer: false,

            // Exclude by-ref types
            IsByRef: false,

            // Exclude abstract types (can't be instantiated)
            IsAbstract: false,

            // Only public types
            IsPublic: true
        } && self.HasParameterlessConstructor();
    }

    /// <summary>
    ///     Retrieves all interfaces/classes that "closes" the
    ///     current <see cref="Type"/> given the generic definition.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="Type"/>.
    /// </param>
    /// <param name="genericDefinition">
    ///     The generic definition type.
    /// </param>
    /// <returns>
    ///     A collection of <see cref="Type"/> that closes
    ///     the current <see cref="Type"/>.
    /// </returns>
    public static IEnumerable<Type> GetInterfacesThatCloses(this Type self, Type genericDefinition) {
        return genericDefinition.IsGenericTypeDefinition
            ? GetInterfacesThatClosesCore(self, genericDefinition)
            : [];
    }

    private static IEnumerable<Type> GetInterfacesThatClosesCore(Type? current, Type genericDefinition) {
        if (current is null || current.IsAbstract || current.IsInterface) { yield break; }

        if (genericDefinition.IsInterface) {
            var interfaces = current.GetInterfaces()
                                    .Where(type => type.IsGenericType &&
                                                   type.GetGenericTypeDefinition() == genericDefinition);
            foreach (var @interface in interfaces) {
                yield return @interface;
            }
        }
        else if (current.BaseType is { IsGenericType: true } && current.BaseType.GetGenericTypeDefinition() == genericDefinition) {
            yield return current.BaseType;
        }

        if (current.BaseType == typeof(object)) {
            yield break;
        }

        foreach (var @interface in GetInterfacesThatClosesCore(current.BaseType, genericDefinition)) {
            yield return @interface;
        }
    }

    /// <summary>
    ///     Whether the type is an open generic, meaning that it is
    ///     a generic type definition or it has generic parameters.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="Type"/>.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the type is an open generic;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsOpenGeneric(this Type self) {
        return self.IsGenericTypeDefinition || self.ContainsGenericParameters;
    }
}