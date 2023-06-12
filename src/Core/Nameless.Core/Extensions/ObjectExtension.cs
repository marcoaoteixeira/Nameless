using System.Reflection;
using System.Runtime.CompilerServices;

namespace Nameless {

    /// <summary>
    /// Extension methods for <see cref="object"/>.
    /// </summary>
    public static class ObjectExtension {

        #region Public Static Methods

        /// <summary>
        /// Verifies if the given object (or type) is an anonymous object (or type).
        /// </summary>
        /// <param name="self">The source object.</param>
        /// <returns><c>true</c> if anonymous object (or type), otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool IsAnonymous(this object self) {
            Prevent.Null(self, nameof(self));

            var type = self as Type ?? self.GetType();

            return
                type.GetCustomAttribute<CompilerGeneratedAttribute>(inherit: true) != default &&
                type.IsGenericType &&
                type.Name.Contains("AnonymousType") &&
                (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"));
        }

        #endregion
    }
}
