using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nameless.Localization.Json.Objects;

/// <summary>
/// Represents a resource containing localized messages for a specific culture.
/// </summary>
[DebuggerDisplay(value: "{DebuggerDisplayValue,nq}")]
public record Resource {
    /// <summary>
    /// Gets an empty resource with no path, culture, and messages.
    /// </summary>
    public static Resource Empty => new(string.Empty, string.Empty, [], isAvailable: false);

    /// <summary>
    /// Gets the path of the resource.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the culture associated with the resource.
    /// </summary>
    public string Culture { get; }

    /// <summary>
    /// Gets the messages contained in the resource.
    /// </summary>
    public Message[] Messages { get; }

    /// <summary>
    /// Whether the resource is available or not.
    /// </summary>
    public bool IsAvailable { get; }

    private string DebuggerDisplayValue
        => $"{nameof(Path)}: {Path} | {nameof(Culture)}: {Culture} | {nameof(IsAvailable)}: {IsAvailable}";

    /// <summary>
    /// Initializes a new instance of the <see cref="Resource"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="culture">The culture.</param>
    /// <param name="messages">The messages</param>
    /// <param name="isAvailable">Whether it is available or not.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="path"/> or
    ///     <paramref name="culture"/> or
    ///     <paramref name="messages"/> is <see langword="null"/>.
    /// </exception>
    public Resource(string path, string culture, IEnumerable<Message> messages, bool isAvailable) {
        Path = path;
        Culture = culture;
        Messages = [.. messages];
        IsAvailable = isAvailable;
    }

    /// <summary>
    /// Tries to get a message by its identifier.
    /// </summary>
    /// <param name="id">The ID.</param>
    /// <param name="output">The message; if available.</param>
    /// <returns>
    /// <see langword="true"/> if the message was found; otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryGetMessage(string id, [NotNullWhen(returnValue: true)] out Message? output) {
        var current = Messages.SingleOrDefault(item => string.Equals(id, item.Id, StringComparison.Ordinal));

        output = current;

        return current is not null;
    }
}