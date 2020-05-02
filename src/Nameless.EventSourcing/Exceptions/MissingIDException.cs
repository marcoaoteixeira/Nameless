using System;

namespace Nameless.EventSourcing {
    /// <summary>
    /// Exception for aggregate and event without ID.
    /// </summary>
    public class MissingIDException : Exception {

        #region Public Properties

        /// <summary>
        /// Gets the aggregate type.
        /// </summary>
        public Type AggregateType { get; }

        /// <summary>
        /// Gets the event type.
        /// </summary>
        public Type EventType { get; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="MissingIDException"/>
        /// </summary>
        public MissingIDException (Type aggregateType, Type eventType) {
            AggregateType = aggregateType;
            EventType = eventType;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MissingIDException"/>
        /// </summary>
        public MissingIDException () { }

        /// <summary>
        /// Initializes a new instance of <see cref="MissingIDException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        public MissingIDException (string message) : base (message) { }

        /// <summary>
        /// Initializes a new instance of <see cref="MissingIDException"/>
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public MissingIDException (string message, Exception innerException) : base (message, innerException) { }

        #endregion
    }
}