using System.Text;

namespace Nameless.Mailing;

/// <summary>
/// Defines a message to the mailing system.
/// </summary>
public sealed record Message {
    /// <summary>
    /// Gets or sets the message subject.
    /// </summary>
    public string Subject { get; }
    /// <summary>
    /// Gets or sets the message content.
    /// </summary>
    public string Content { get; }
    /// <summary>
    /// Gets or sets the message language.
    /// </summary>
    public string? Language { get; }

    /// <summary>
    /// Gets or sets the message encoding.
    /// </summary>
    public Encoding Encoding { get; }
    /// <summary>
    /// Gets or sets an array of address from the
    /// person (or people) who sends the message
    /// </summary>
    public string[] From { get; }
    /// <summary>
    /// Gets or sets an array of address to the
    /// person (or people) who receives the message
    /// </summary>
    public string[] To { get; }
    /// <summary>
    /// A dictionary of properties that can be used
    /// by the messenger.
    /// </summary>
    public MessageArgs Parameters { get; }
    /// <summary>
    /// Gets or sets the message priority.
    /// </summary>
    public Priority Priority { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Message"/>.
    /// </summary>
    /// <param name="subject">The message subject.</param>
    /// <param name="content">The message body content.</param>
    /// <param name="from">The message FROM list.</param>
    /// <param name="to">The message TO list.</param>
    /// <param name="language">The message language.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="parameters">The message extra parameters.</param>
    /// <param name="priority">The message priority.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="subject"/> or
    /// <paramref name="content"/> or
    /// <paramref name="from"/> or
    /// <paramref name="to"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="subject"/> or
    /// <paramref name="content"/> is empty or white spaces.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="from"/> or
    /// <paramref name="to"/> is empty array.
    /// </exception>
    public Message(string subject, string content, string[] from, string[] to, string? language = null, Encoding? encoding = null, MessageArgs? parameters = null, Priority priority = Priority.Normal) {
        Subject = Prevent.Argument.NullOrWhiteSpace(subject);
        Content = Prevent.Argument.NullOrWhiteSpace(content);
        Language = language;
        Encoding = encoding ?? Defaults.Encoding;
        From = Prevent.Argument.NullOrEmpty(from);
        To = Prevent.Argument.NullOrEmpty(to);
        Parameters = parameters ?? [];
        Priority = priority;
    }
}