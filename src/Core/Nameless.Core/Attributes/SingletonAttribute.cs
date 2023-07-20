using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Nameless {

    /// <summary>
    /// Classes marked with <see cref="SingletonAttribute"/> must have a static
    /// property, defined in <see cref="AccessorName"/>, that will return the
    /// singleton instance object of the type that this attribute was applied.
    /// <see cref="https://en.wikipedia.org/wiki/Singleton_pattern"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class SingletonAttribute : Attribute {

        #region Public Constants

        public const string DEFAULT_ACCESSOR_NAME = "Instance";

        #endregion

        #region Public Properties

        private string? _accessorName;
        /// <summary>
        /// Gets or sets the name of the property that will be used to get
        /// the singleton instance of the annotated class.
        /// </summary>
        public string AccessorName {
            get { return _accessorName ??= DEFAULT_ACCESSOR_NAME; }
            set { _accessorName = value; }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Retrieves the singleton instance of the type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>A singleton instance of the type.</returns>
        public static T? GetInstance<T>() where T : class
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
        public static object? GetInstance(Type? type) {
            if (type == null) { return null; }

            if (!HasAttribute(type, out var attr)) { return null; }
            if (!HasAccessorProperty(type, attr.AccessorName, out var accessor)) { return null; }

            return accessor.GetValue(obj: null /* static instance */);
        }

        #endregion

        #region Private Static Methods

        private static bool HasAttribute(Type type, [NotNullWhen(true)] out SingletonAttribute? attr) {
            attr = type.GetCustomAttribute<SingletonAttribute>(inherit: false);
            return attr != null;
        }

        private static bool HasAccessorProperty(Type type, string accessorName, [NotNullWhen(true)] out PropertyInfo? accessor) {
            var currentAccessor = string.IsNullOrWhiteSpace(accessorName) ? DEFAULT_ACCESSOR_NAME : accessorName;
            accessor = type.GetProperty(currentAccessor, BindingFlags.Public | BindingFlags.Static);
            return accessor != null;
        }

        #endregion
    }
}
