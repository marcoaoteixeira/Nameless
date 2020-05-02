using System;

namespace Nameless.EventSourcing {
    // Define other methods and classes here
    /// <summary>
    /// Exception for events out of order.
    /// </summary>
    public class EventsOutOfOrderException : Exception {

        #region Public Properties

        /// <summary>
        /// Gets the event ID.
        /// </summary>
        public Guid EventID { get; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="EventsOutOfOrderException"/>
        /// </summary>
        /// <param name="eventID">Event ID.</param>
        public EventsOutOfOrderException (Guid eventID) {
            EventID = eventID;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EventsOutOfOrderException"/>
        /// </summary>
        public EventsOutOfOrderException () { }

        /// <summary>
        /// Initializes a new instance of <see cref="EventsOutOfOrderException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        public EventsOutOfOrderException (string message) : base (message) { }

        /// <summary>
        /// Initializes a new instance of <see cref="EventsOutOfOrderException"/>
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public EventsOutOfOrderException (string message, Exception innerException) : base (message, innerException) { }

        #endregion Public Constructors
    }
}