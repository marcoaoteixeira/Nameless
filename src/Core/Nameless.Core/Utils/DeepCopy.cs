using System.Text.Json;

namespace Nameless.Utils {
    public static class DeepCopy {
        #region Public Static Methods

        /// <summary>
        /// Performs a deep copy of an object, using JSON as a serialization method. NOTE: Private members are not cloned using this method.
        /// </summary>
        /// <param name="value">The object to copy.</param>
        /// <returns>The copied object.</returns>
        /// <remarks>
        /// Reference article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
        /// </remarks>
        /// <exception cref="ArgumentNullException">if <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Can't clone abstract, interface or pointer.</exception>
        public static object Clone(object value) {
            Guard.Against.Null(value, nameof(value));

            var type = value.GetType();
            if (type.IsAbstract || type.IsInterface || type.IsPointer) {
                throw new InvalidOperationException($"Cannot clone abstract classes, interfaces or pointers. Object type: {type}");
            }

            var json = JsonSerializer.Serialize(value);
            var result = JsonSerializer.Deserialize(json, type);

            return result!;
        }

        /// <summary>
        /// Performs a deep copy of an object, using JSON as a serialization method. NOTE: Private members are not cloned using this method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="value">The object to copy.</param>
        /// <returns>The copied object.</returns>
        /// <remarks>
        /// Reference article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
        /// </remarks>
        /// <exception cref="ArgumentNullException">if <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Can't clone abstract, interface or pointer.</exception>
        public static T Clone<T>(T value) where T : class
            => (T)Clone(value as object);

        #endregion
    }
}
