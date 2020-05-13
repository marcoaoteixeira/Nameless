namespace Nameless.Patterns.Specifications {

    /// <summary>
    /// Abstract implementation of <see cref="ISpecification{T}"/>
    /// </summary>
    /// <typeparam name="T">Type of the specification.</typeparam>
	public abstract class CompositeSpecification<T> : ISpecification<T>
        where T : class {

        #region ISpecification<T> Members

        /// <inheritdoc />
        public abstract bool IsSatisfiedBy(T candidate);

        /// <inheritdoc />
        public ISpecification<T> And(ISpecification<T> other) => new AndSpecification<T>(this, other);

        /// <inheritdoc />
        public ISpecification<T> Or(ISpecification<T> other) => new OrSpecification<T>(this, other);

        /// <inheritdoc />
        public ISpecification<T> Not() => new NotSpecification<T>(this);

        #endregion ISpecification<T> Members
    }
}