namespace Nameless.Patterns.Specifications {

    /// <summary>
    /// "OR" implementation of <see cref="CompositeSpecification{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the specification.</typeparam>
	public class OrSpecification<T> : CompositeSpecification<T>
        where T : class {

        #region Private Read-Only Fields

        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        #endregion Private Read-Only Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="OrSpecification{T}"/>.
        /// </summary>
        /// <param name="left">Left specification.</param>
        /// <param name="right">Right specification.</param>
        public OrSpecification(ISpecification<T> left, ISpecification<T> right) {
            Prevent.ParameterNull(left, nameof(left));
            Prevent.ParameterNull(right, nameof(right));

            _left = left;
            _right = right;
        }

        #endregion Public Constructors

        #region Public Override Methods

        /// <inheritdoc />
        public override bool IsSatisfiedBy(T candidate) => _left.IsSatisfiedBy(candidate) || _right.IsSatisfiedBy(candidate);

        #endregion Public Override Methods
    }
}