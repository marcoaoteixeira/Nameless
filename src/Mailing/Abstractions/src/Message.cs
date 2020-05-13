using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Nameless.Mailing {

    /// <summary>
    /// The e-mail message.
    /// </summary>
    public class Message {

        #region Public Properties

        /// <summary>
        /// Gets or sets the encoding type. Default value is
        /// <see cref="Encoding.UTF8" />.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

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
        /// Gets the list of blind carbon copy (BCC) recipients for this
        /// message.
        /// </summary>
        public IList<string> Bcc { get; } = new List<string> ();

        /// <summary>
        /// Gets or sets the subject of the message.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the message body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets whether the message body is HTML or not. Default
        /// value is <c>false</c>
        /// </summary>
        public bool IsBodyHtml { get; set; } = false;
        
        /// <summary>
        /// Gets or sets the message priority. Default value is
        /// <see cref="MessagePriority.Medium" />
        /// </summary>
        public MessagePriority Priority { get; set; } = MessagePriority.Medium;

        /// <summary>
        /// Gets the message headers.
        /// </summary>
        public NameValueCollection Headers { get; } = new NameValueCollection ();

        #endregion Public Properties
    }
}