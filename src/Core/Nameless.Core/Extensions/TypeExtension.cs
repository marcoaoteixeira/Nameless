using System.Reflection;

namespace Nameless {

    /// <summary>
    /// Extension methods for <see cref="Type"/>.
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

        #endregion Private Static Read-Only Fields

        #region Public Static Methods

        public static bool IsSingleton(this Type self) => self.GetCustomAttribute<SingletonAttribute>(inherit: false) != null;

        /// <summary>
        /// Verifies if the <see cref="Type"/> is an instance of <see cref="Nullable"/>.
        /// </summary>
        /// <param name="self">The self type.</param>
        /// <returns><c>true</c>, if is instance of <see cref="Nullable"/>, otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool IsNullable(this Type self) {
            Prevent.Null(self, nameof(self));

            return self.IsGenericType && self.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Can convert to <see cref="Nullable"/> type.
        /// </summary>
        /// <param name="self">The self type.</param>
        /// <returns><c>true</c>, if can convert, otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool AllowNull(this Type self) {
            Prevent.Null(self, nameof(self));

            return !self.IsValueType || self.IsNullable();
        }

        /// <summary>
        /// Retrieves the generic method associated to the self type.
        /// </summary>
        /// <param name="self">The self type.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="genericArgumentTypes">Method generic argument types, if any.</param>
        /// <param name="argumentTypes">Method argument types, if any.</param>
        /// <param name="returnType">Method return type.</param>
        /// <returns>Returns an instance of <see cref="MethodInfo"/> representing the generic method.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static MethodInfo? GetGenericMethod(this Type self, string name, Type[] genericArgumentTypes, Type[]? argumentTypes = default, Type? returnType = default) {
            Prevent.Null(self, nameof(self));
            Prevent.NullOrWhiteSpaces(name, nameof(name));
            Prevent.Null(genericArgumentTypes, nameof(genericArgumentTypes));

            var innerArgumentTypes = argumentTypes ?? Array.Empty<Type>();
            var innerReturnType = returnType ?? typeof(void);

            return self.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Where(method =>
                   method.Name == name &&
                   method.GetGenericArguments().Length == genericArgumentTypes.Length &&
                   method.GetParameters().Select(parameter => parameter.ParameterType).SequenceEqual(innerArgumentTypes) &&
                   (method.ReturnType.IsGenericType && !method.ReturnType.IsGenericTypeDefinition ? innerReturnType.GetGenericTypeDefinition() : method.ReturnType) == innerReturnType)
                .Single()
                .MakeGenericMethod(genericArgumentTypes);
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
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool IsAssignableTo(this Type self, Type type) {
            Prevent.Null(self, nameof(self));
            Prevent.Null(type, nameof(type));

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
        public static bool IsAssignableTo<T>(this Type self) => IsAssignableTo(self, typeof(T));

        /// <summary>
        /// Verifies if the <paramref name="self"/> is a simple type.
        /// </summary>
        /// <param name="self">The self type.</param>
        /// <returns><c>true</c> if is simple type; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool IsSimple(this Type self) {
            Prevent.Null(self, nameof(self));

            return self.IsPrimitive || WriteTypes.Contains(self);
        }

        /// <summary>
        /// Retrieves the first occurrence of the specified generic argument.
        /// </summary>
        /// <param name="self">The source type.</param>
        /// <param name="genericArgumentType">The generic argument type.</param>
        /// <returns>The generic argument type, if found.</returns>
        public static Type? GetFirstOccurrenceOfGenericArgument(this Type? self, Type genericArgumentType) {
            if (self == default) { return default; }

            Prevent.Null(genericArgumentType, nameof(genericArgumentType));

            var args = self.GetGenericArguments();
            var result = args.FirstOrDefault(genericArgumentType.IsAssignableFromGenericType);
            return result ?? GetFirstOccurrenceOfGenericArgument(self.BaseType, genericArgumentType);
        }

        public static bool IsAssignableFromGenericType(this Type self, Type type) {
            foreach (var item in type.GetInterfaces()) {
                if (item.IsGenericType && item.GetGenericTypeDefinition() == self) {
                    return true;
                }
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == self) {
                return true;
            }

            if (type.BaseType == default) {
                return false;
            }

            return IsAssignableFromGenericType(self, type.BaseType);
        }

        public static bool HasInterface<TInterface>(this Type self) where TInterface : class
            => HasInterface(self, typeof(TInterface));

        public static bool HasInterface(this Type self, Type @interface) {
            Prevent.Null(self, nameof(self));
            Prevent.Null(@interface, nameof(@interface));

            return self.GetInterfaces().Any(_ => @interface.IsAssignableFrom(_) || @interface.IsAssignableFromGenericType(_));
        }

        #endregion
    }
}