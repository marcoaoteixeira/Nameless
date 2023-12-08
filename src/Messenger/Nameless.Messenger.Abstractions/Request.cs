using System.Text;

namespace Nameless.Messenger {
    public sealed record Request {
        #region Public Properties

        /// <summary>
        /// Gets or sets the message subject.
        /// </summary>
        public string Subject { get; init; } = null!;
        /// <summary>
        /// Gets or sets the message content.
        /// </summary>
        public string Message { get; init; } = null!;
        /// <summary>
        /// Gets or sets the message language.
        /// </summary>
        public string? Language { get; init; }
        /// <summary>
        /// Gets or sets the message encoding.
        /// </summary>
        public Encoding Encoding { get; init; } = Encoding.UTF8;
        /// <summary>
        /// Gets or sets an array of address from the
        /// person (or people) who sends the message
        /// </summary>
        public string[] From { get; init; } = [];
        /// <summary>
        /// Gets or sets an array of address to the
        /// person (or people) who receives the message
        /// </summary>
        public string[] To { get; init; } = [];
        /// <summary>
        /// A dictionary of properties that can be used
        /// by the messenger.
        /// </summary>
        public MessengerArgs Args { get; } = new();
        /// <summary>
        /// Gets or sets the message priority.
        /// </summary>
        public Priority Priority { get; init; } = Priority.Normal;

        #endregion
    }
}
