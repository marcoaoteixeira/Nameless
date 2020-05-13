namespace Nameless.Patterns.Specifications {

    /// <summary>
    /// "NOT" implementation of <see cref="CompositeSpecification{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the specification.</typeparam>
	public class NotSpecification<T> : CompositeSpecification<T>
        where T : class {

        #region Private Read-Only Fields

        private readonly ISpecification<T> _other;

        #endregion Private Read-Only Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="NotSpecification{T}"/>.
        /// </summary>
        /// <param name="other">The specification to negate.</param>
        public NotSpecification(ISpecification<T> other) {
            Prevent.ParameterNull(other, nameof(other));

            _other = other;
        }

        #endregion Public Constructors

        #region Public Override Methods

        /// <inheritdoc />
        public override bool IsSatisfiedBy(T candidate) => !_other.IsSatisfiedBy(candidate);

        #endregion Public Override Methods
    }
}