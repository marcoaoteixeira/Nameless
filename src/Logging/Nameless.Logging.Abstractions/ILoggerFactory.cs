namespace Nameless.Logging {
    /// <summary>
    /// Interface to implement a logger factory.
    /// </summary>
    public interface ILoggerFactory {
        #region Methods

        /// <summary>
        /// Creates a new instance of the <see cref="ILogger"/>
        /// for the given source.
        /// </summary>
        /// <param name="source">The logger source.</param>
        /// <returns>
        /// Returns a new instance of <see cref="ILogger"/> for
        /// the specified source.
        /// </returns>
        ILogger CreateLogger(string source);

        #endregion
    }
}