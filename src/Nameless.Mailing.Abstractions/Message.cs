using System.Text;

namespace Nameless.Mailing;

/// <summary>
///     Defines a message to the mailing system.
/// </summary>
public sealed record Message {
    /// <summary>
    ///     Gets the message subject.
    /// </summary>
    public string Subject { get; }

    /// <summary>
    ///     Gets an array of address to be used as senders.
    /// </summary>
    public string[] From { get; }

    /// <summary>
    ///     Gets an array of address to be used as recipients.
    /// </summary>
    public string[] To { get; }

    /// <summary>
    ///     Gets an array of address to be used as carbon copy recipients.
    /// </summary>
    public string[] Cc { get; }

    /// <summary>
    ///     Gets an array of address to be used as blank carbon copy recipients.
    /// </summary>
    public string[] Bcc { get; }

    /// <summary>
    ///     Gets the message content.
    /// </summary>
    public string Content { get; }

    /// <summary>
    ///     Gets the message language.
    /// </summary>
    public string? Language { get; }

    /// <summary>
    ///     Gets the message encoding.
    /// </summary>
    public Encoding Encoding { get; }

    /// <summary>
    ///     Whether the message body should be HTML.
    /// </summary>
    public bool IsBodyHtml { get; }

    /// <summary>
    ///     Gets the message priority.
    /// </summary>
    public Priority Priority { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="Message" />.
    /// </summary>
    /// <param name="subject">The subject.</param>
    /// <param name="from">The senders address.</param>
    /// <param name="to">The recipients address.</param>
    /// <param name="content">The content.</param>
    /// <param name="cc">The carbon copy recipients address.</param>
    /// <param name="bcc">The blank carbon copy recipients address.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="language">The language.</param>
    /// <param name="isBodyHtml">Whether the body is HTML.</param>
    /// <param name="priority">The priority.</param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="subject" /> or
    ///     <paramref name="content" /> or
    ///     <paramref name="from" /> or
    ///     <paramref name="to" /> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="subject" /> or
    ///     <paramref name="content" /> is empty or white spaces.
    ///     Thrown also when <paramref name="from" /> or
    ///     <paramref name="to" /> is empty array.
    /// </exception>
    public Message(string subject,
                   string[] from,
                   string[] to,
                   string content,
                   string[]? cc = null,
                   string[]? bcc = null,
                   Encoding? encoding = null,
                   string? language = null,
                   bool isBodyHtml = false,
                   Priority priority = Priority.Normal) {
        Subject = Guard.Against.NullOrWhiteSpace(subject);
        From = Guard.Against.NullOrEmpty(from);
        To = Guard.Against.NullOrEmpty(to);
        Content = Guard.Against.NullOrWhiteSpace(content);
        Cc = cc ?? [];
        Bcc = bcc ?? [];
        Encoding = encoding ?? Defaults.Encoding;
        Language = language;
        IsBodyHtml = isBodyHtml;
        Priority = priority;
    }
}