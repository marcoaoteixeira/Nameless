using System.Reflection;

namespace Nameless {
    /// <summary>
    /// <see cref="Type"/> extension methods.
    /// </summary>
    public static class TypeExtension {
        #region Private Static Read-Only Fields

        private static readonly Type[] WriteTypes = new[] {
            typeof (string),
            typeof (DateTime),
            typeof (Enum),
            typeof (decimal),
            typeof (Guid)
        };

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Verifies if the <see cref="Type"/> is an instance of <see cref="Nullable"/>.
        /// </summary>
        /// <param name="self">The self type.</param>
        /// <returns><c>true</c>, if is instance of <see cref="Nullable"/>, otherwise, <c>false</c>.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool IsNullable(this Type self)
            => self.IsGenericType && self.GetGenericTypeDefinition() == typeof(Nullable<>);

        /// <summary>
        /// Can convert to <see cref="Nullable"/> type.
        /// </summary>
        /// <param name="self">The self type.</param>
        /// <returns><c>true</c>, if it can convert, otherwise, <c>false</c>.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool AllowNull(this Type self)
            => !self.IsValueType || self.IsNullable();

        /// <summary>
        /// Retrieves the generic method associated to the self type.
        /// </summary>
        /// <param name="self">The self type.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="genericArgumentTypes">Method generic argument types, if any.</param>
        /// <param name="argumentTypes">Method argument types, if any.</param>
        /// <param name="returnType">Method return type.</param>
        /// <returns>Returns an instance of <see cref="MethodInfo"/> representing the generic method.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static MethodInfo? GetGenericMethod(this Type self, string name, Type[] genericArgumentTypes, Type[]? argumentTypes = null, Type? returnType = null) {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(genericArgumentTypes, nameof(genericArgumentTypes));

            var innerArgumentTypes = argumentTypes ?? [];
            var innerReturnType = returnType ?? typeof(void);

            return self
                   .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                   .SingleOrDefault(method => method.Name == name &&
                                              method.GetGenericArguments().Length == genericArgumentTypes.Length &&
                                              method.GetParameters().Select(parameter => parameter.ParameterType).SequenceEqual(innerArgumentTypes) &&
                                              (method.ReturnType is { IsGenericType: true, IsGenericTypeDefinition: false } ? innerReturnType.GetGenericTypeDefinition() : method.ReturnType) == innerReturnType)
                   ?.MakeGenericMethod(genericArgumentTypes);
        }

        /// <summary>
        /// Returns a value that indicates whether the current type can be assigned to the
        /// specified type.
        /// </summary>
        /// <param name="self">The current type.</param>
        /// <param name="type">The specified type.</param>
        /// <returns>
        /// <c>true</c> if the current type can be assigned to the specified type;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAssignableTo(this Type self, Type type) {
            Guard.Against.Null(type, nameof(type));

            return type.IsAssignableFrom(self);
        }

        /// <summary>
        /// Returns a value that indicates whether the current type can be assigned to the
        /// specified type.
        /// </summary>
        /// <typeparam name="T">The specified type.</typeparam>
        /// <param name="self">The current type.</param>
        /// <returns>
        /// <c>true</c> if the current type can be assigned to the specified type;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAssignableTo<T>(this Type self)
            => IsAssignableTo(self, typeof(T));

        /// <summary>
        /// Verifies if the <paramref name="self"/> is a simple type.
        /// </summary>
        /// <param name="self">The self type.</param>
        /// <returns><c>true</c> if is simple type; otherwise, <c>false</c>.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool IsSimple(this Type self)
            => self.IsPrimitive || WriteTypes.Contains(self);

        /// <summary>
        /// Retrieves the first occurrence of the specified generic argument.
        /// </summary>
        /// <param name="self">The source type.</param>
        /// <param name="genericArgumentType">The generic argument type.</param>
        /// <returns>The generic argument type, if found.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="genericArgumentType"/> is <c>null</c>.</exception>
        public static Type? GetFirstOccurrenceOfGenericArgument(this Type? self, Type genericArgumentType) {
            while (true) {
                if (self is null) { return null; }

                Guard.Against.Null(genericArgumentType, nameof(genericArgumentType));

                var args = self.GetGenericArguments();
                var result = args.FirstOrDefault(genericArgumentType.IsAssignableFromGenericType);

                if (result is not null) { return result; }

                self = self.BaseType;
            }
        }

        /// <summary>
        /// Checks if the current type (<paramref name="self"/>) is assignable from the <paramref name="type"/>.
        /// </summary>
        /// <param name="self">The current type.</param>
        /// <param name="type">The assignable from type.</param>
        /// <returns><c>true</c> if assignable; otherwise <c>false</c>.</returns>
        public static bool IsAssignableFromGenericType(this Type self, Type? type) {
            bool Assignable(Type current) {
                return current.IsGenericType &&
                       current.GetGenericTypeDefinition() == self;
            }

            while (true) {
                if (type is null) { return false; }

                var assignable = type.GetInterfaces().Any(Assignable);
                if (assignable) { return true; }
                
                if (Assignable(type)) { return true; }

                type = type.BaseType;
            }
        }

        public static bool HasInterface<TInterface>(this Type self) where TInterface : class
            => HasInterface(self, typeof(TInterface));

        /// <summary>
        /// Checks if the current type (<paramref name="self"/>) implements the specified interface defined by <paramref name="interfaceType"/>.
        /// </summary>
        /// <param name="self">The current type.</param>
        /// <param name="interfaceType">The interface type.</param>
        /// <returns><c>true</c> if implements; otherwise <c>false</c>.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">if <paramref name="interfaceType"/> is <c>null</c>.</exception>
        public static bool HasInterface(this Type self, Type interfaceType) {
            Guard.Against.Null(interfaceType, nameof(interfaceType));

            return self.GetInterfaces()
                       .Any(type => interfaceType.IsAssignableFrom(type) ||
                                    interfaceType.IsAssignableFromGenericType(type));
        }

        public static bool HasAttribute<TAttribute>(this Type self, bool inherit = false)
            where TAttribute : Attribute
            => HasAttribute(self, typeof(TAttribute), inherit);

        public static bool HasAttribute(this Type self, Type attributeType, bool inherit = false) {
            Guard.Against.Null(attributeType, nameof(attributeType));

            return self.GetCustomAttribute(attributeType, inherit) is not null;
        }

        public static bool HasParameterlessConstructor(this Type self)
            => self.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                   .Any(constructor => constructor.GetParameters()
                                                  .Length == 0);

        #endregion
    }
}