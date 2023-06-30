﻿namespace Nameless.Text {

    /// <summary>
    /// Implementation of <see cref="TextExpression"/> that uses the <see
    /// cref="string.Format(IFormatProvider, string, object)"/> functionality.
    /// </summary>
    public sealed class FormatTextExpression : TextExpression {

        #region	Private Read-Only Fields

        private readonly IDataBinder _dataBinder;
        private readonly string _expression;
        private readonly string? _format;

        #endregion

        #region	Public Properties

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public string Expression => _expression;

        /// <summary>
        /// Gets the format.
        /// </summary>
        public string? Format => _format;

        #endregion

        #region	Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="FormatTextExpression"/>.
        /// </summary>
        /// <param name="dataBinder">An instance of <see cref="IDataBinder"/>.</param>
        /// <param name="expression">The text expression.</param>
        public FormatTextExpression(IDataBinder dataBinder, string expression) {
            Garda.Prevent.Null(dataBinder, nameof(dataBinder));
            Garda.Prevent.NullOrWhiteSpace(expression, nameof(expression));

            _dataBinder = dataBinder;

            if (!expression.StartsWith("{") || !expression.EndsWith("}")) { throw new FormatException("Invalid expression"); }

            var expressionWithoutBraces = expression[1..^1];
            var colonIndex = expressionWithoutBraces.IndexOf(':');

            if (colonIndex > 0) {
                _expression = expressionWithoutBraces[..colonIndex];
                _format = expressionWithoutBraces[(colonIndex + 1)..];
            } else { _expression = expressionWithoutBraces; }
        }

        #endregion

        #region Public Override Methods

        /// <inheritdoc/>
        public override string? Eval(object? obj) {
            Garda.Prevent.Null(obj, nameof(obj));

            return (!string.IsNullOrWhiteSpace(Format)
                    ? _dataBinder.Eval(obj, Expression, string.Concat("{0:", Format, "}"))
                    : _dataBinder.Eval(obj, Expression)).ToString();
        }

        #endregion
    }
}
