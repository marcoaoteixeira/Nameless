using System;

namespace Nameless.Patterns.Specifications {

    /// <summary>
    /// "Expression" implementation of <see cref="CompositeSpecification{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the expression.</typeparam>
    public class ExpressionSpecification<T> : CompositeSpecification<T>
        where T : class {

        #region Private Read-Only Fields

        private readonly Func<T, bool> _expression;

        #endregion Private Read-Only Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of<see cref="ExpressionSpecification{T}"/>.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public ExpressionSpecification(Func<T, bool> expression) {
            Prevent.ParameterNull(expression, nameof(expression));

            _expression = expression;
        }

        #endregion Public Constructors

        #region Public Override Methods

        /// <inheritdoc />
        public override bool IsSatisfiedBy(T candidate) => _expression(candidate);

        #endregion Public Override Methods
    }
}