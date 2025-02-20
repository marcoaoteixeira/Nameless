using System.Text.Json;

namespace Nameless.Utils;

public static class DeepCopy {
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
        Prevent.Argument.Null(value);

        if (value is Type) {
            throw new InvalidOperationException($"Cannot clone non-class object. Object type: {value.GetType()}");
        }

        var json = JsonSerializer.Serialize(value);
        var result = JsonSerializer.Deserialize(json, value.GetType());

        return result ?? throw new InvalidOperationException("Unable to clone object.");
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
    public static T Clone<T>(T value)
        where T : class
        => (T)Clone(value as object);
}