using System.Text.Json;
using Nameless.Infrastructure;

namespace Nameless;

public static class ArgCollectionExtension {
    /// <summary>
    /// Serializes the current <see cref="ArgCollection"/> to JSON.
    /// </summary>
    /// <param name="self">The current <see cref="ArgCollection"/>.</param>
    /// <returns>
    /// A JSON string representation of the current <see cref="ArgCollection"/>.
    /// </returns>
    public static string ToJson(this ArgCollection self)
        => JsonSerializer.Serialize(self);
}