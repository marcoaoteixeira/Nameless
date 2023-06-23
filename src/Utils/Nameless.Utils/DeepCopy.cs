using System.Text.Json;

namespace Nameless.Utils {
    public static class DeepCopy {
        #region Public Static Methods

        /// <summary>
        /// Performs a deep copy of an object, using JSON as a serialization method. NOTE: Private members are not cloned using this method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="obj">The object to copy.</param>
        /// <returns>The copied object.</returns>
        /// <remarks>
        /// Reference article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
        /// </remarks>
        public static object? Clone(object obj) {
            // Don't serialize a null object, simply return the default for that object.
            if (obj is null) { return default; }

            var objType = obj.GetType();
            if (objType.IsAbstract || objType.IsInterface || objType.IsPointer) {
                throw new InvalidOperationException($"Cannot clone abstract classes, interfaces or pointers. Object type: {objType}");
            }

            var json = JsonSerializer.Serialize(obj);

            return JsonSerializer.Deserialize(json, objType);
        }

        public static T? Clone<T>(T obj) where T : class => (T?)Clone(obj as object);

        #endregion
    }
}