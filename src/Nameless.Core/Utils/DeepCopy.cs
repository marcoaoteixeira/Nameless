using System.Text.Json;

namespace Nameless.Utils;

public static class DeepCopy {
    /// <summary>
    ///     Performs a deep copy of an object, using JSON as a serialization
    ///     method. NOTE: Private members are not cloned using this method.
    /// </summary>
    /// <param name="value">
    ///     The object to copy.
    /// </param>
    /// <returns>
    ///     The copied object.
    /// </returns>
    /// <remarks>
    ///     Reference article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="value" /> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Can't clone abstract, interface or pointer.
    /// </exception>
    public static object Clone(object value) {
        Guard.Against.Null(value);

        var type = value.GetType();

        if (value is Type) {
            throw new InvalidOperationException($"Can't clone non-class object. Object type: {type}.");
        }

        if (!type.IsConcrete()) {
            throw new InvalidOperationException($"Can't clone non-concrete object. Object type: {type}.");
        }

        var json = JsonSerializer.Serialize(value);
        var result = JsonSerializer.Deserialize(json, type);

        return result ?? throw new InvalidOperationException("Unable to clone object.");
    }

    /// <summary>
    ///     Performs a deep copy of an object, using JSON as a serialization
    ///     method. NOTE: Private members are not cloned using this method.
    /// </summary>
    /// <typeparam name="T">
    ///     Type of the value.
    /// </typeparam>
    /// <param name="value">
    ///     The object to copy.
    /// </param>
    /// <returns>
    ///     The copied object.
    /// </returns>
    /// <remarks>
    ///     Reference article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="value" /> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Can't clone abstract, interface or pointer.
    /// </exception>
    public static T Clone<T>(T value)
        where T : class {
        return (T)Clone(value as object);
    }
}