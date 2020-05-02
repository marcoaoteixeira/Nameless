using System;

namespace Nameless.CircuitBreaker {
    public interface IExceptionFilter {
        #region Methods

        /// <summary>
        /// Given an <see cref="Exception"/>, executes the filter and returns if should (or not) rise it.
        /// </summary>
        /// <typeparam name="TException">Type of the <see cref="Exception"/>.</typeparam>
        /// <param name="ex">The instance of the <see cref="Exception"/>.</param>
        /// <returns><c>true</c>, if should rise it; otherwise, <c>false</c>.</returns>
        bool Ignore<TException> (TException ex) where TException : Exception;

        #endregion
    }
}
