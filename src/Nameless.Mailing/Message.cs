using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Nameless.Mailing {

    /// <summary>
    /// The e-mail message.
    /// </summary>
    public class Message {

        #region Public Static Read-Only Fields

        /// <summary>
        /// Gets the default encoding (UTF-8).
        /// </summary>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        #endregion Public Static Read-Only Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the message sender's address.
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Gets the message from addresses.
        /// </summary>
        public IList<string> From { get; } = new List<string> ();

        /// <summary>
        /// Gets the list of recipients for this message.
        /// </summary>
        public IList<string> To { get; } = new List<string> ();

        /// <summary>
        /// Gets the list of carbon copy (CC) recipients for this message.
        /// </summary>
        public IList<string> Cc { get; } = new List<string> ();

        /// <summary>
        /// Gets the list of blind carbon copy (BCC) recipients for this message.
        /// </summary>
        public IList<string> Bcc { get; } = new List<string> ();

        /// <summary>
        /// Gets or sets the subject of the message.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the subject encoding.
        /// </summary>
        public Encoding SubjectEncoding { get; set; } = DefaultEncoding;

        /// <summary>
        /// Gets or sets the message body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the message body encoding.
        /// </summary>
        public Encoding BodyEncoding { get; set; } = DefaultEncoding;

        /// <summary>
        /// Gets or sets if the message body is HTML.
        /// </summary>
        public bool IsBodyHtml { get; set; }

        /// <summary>
        /// Gets or sets the message priority.
        /// </summary>
        public MessagePriority Priority { get; set; } = MessagePriority.Medium;

        /// <summary>
        /// Gets or sets the message headers.
        /// </summary>
        public NameValueCollection Headers { get; set; }

        /// <summary>
        /// Gets or sets the encoding for the message header.
        /// </summary>
        public Encoding HeadersEncoding { get; set; } = DefaultEncoding;

        #endregion Public Properties
    }
}