namespace Nameless.Patterns.Specifications {

    /// <summary>
    /// Defines methods for specifications (Specification Design Pattern).
    /// </summary>
    /// <typeparam name="T">Type of the specification.</typeparam>
	public interface ISpecification<T>
        where T : class {

        #region Methods

        /// <summary>
        /// Checks if the candidate is valid for specification.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns><c>true</c> if candidate is valid; otherwise, <c>false</c>.</returns>
        bool IsSatisfiedBy(T candidate);

        /// <summary>
        /// Adds a new "AND" specification operator.
        /// </summary>
        /// <param name="other">The right hand <see cref="ISpecification{T}"/>.</param>
        /// <returns>The current <see cref="ISpecification{T}"/> instance.</returns>
		ISpecification<T> And(ISpecification<T> other);

        /// <summary>
        /// Adds a new "OR" specification operator.
        /// </summary>
        /// <param name="other">The right hand <see cref="ISpecification{T}"/>.</param>
        /// <returns>The current <see cref="ISpecification{T}"/> instance.</returns>
        ISpecification<T> Or(ISpecification<T> other);

        /// <summary>
        /// Adds a "NOT" specification operator.
        /// </summary>
        /// <returns>The current <see cref="ISpecification{T}"/> instance.</returns>
		ISpecification<T> Not();

        #endregion Methods
    }
}