using System.Text;

namespace Nameless.Mailing {
    public sealed record Message(
        string[] From,
        string[] To,
        string Subject,
        string Content,
        string? Language = null,
        Encoding? Encoding = null,
        Priority Priority = Priority.Normal) {

        #region Public Properties

        /// <summary>
        /// Gets or sets the message subject.
        /// </summary>
        public string Subject { get; } = Subject;
        /// <summary>
        /// Gets or sets the message content.
        /// </summary>
        public string Content { get; } = Content;
        /// <summary>
        /// Gets or sets the message language.
        /// </summary>
        public string? Language { get; } = Language;

        /// <summary>
        /// Gets or sets the message encoding.
        /// </summary>
        public Encoding Encoding { get; } = Encoding ?? Root.Defaults.Encoding;
        /// <summary>
        /// Gets or sets an array of address from the
        /// person (or people) who sends the message
        /// </summary>
        public string[] From { get; } = From;
        /// <summary>
        /// Gets or sets an array of address to the
        /// person (or people) who receives the message
        /// </summary>
        public string[] To { get; } = To;
        /// <summary>
        /// A dictionary of properties that can be used
        /// by the messenger.
        /// </summary>
        public MessageArgs Parameters { get; } = [];
        /// <summary>
        /// Gets or sets the message priority.
        /// </summary>
        public Priority Priority { get; } = Priority;

        #endregion
    }
}
