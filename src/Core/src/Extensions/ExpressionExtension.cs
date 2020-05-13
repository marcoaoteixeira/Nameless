using System;
using System.Linq.Expressions;

namespace Nameless {
    /// <summary>
    /// Extension methods for <see cref="Expression"/>.
    /// </summary>
    public static class ExpressionExtension {

        #region Public Static Methods

        /// <summary>
        /// Retrieves the member info.
        /// </summary>
        /// <param name="self">The method.</param>
        /// <returns>An instance of <see cref="MemberExpression"/> representing the <paramref name="self"/> expression.</returns>
        /// <remarks>Used to get property information</remarks>
        public static MemberExpression GetMemberInfo (this Expression self) {
            if (self == null) { return null; }

            if (!(self is LambdaExpression lambda)) {
                throw new ArgumentException ($"Not a lambda expression ({nameof(self)})");
            }

            MemberExpression member = null;
            switch (lambda.Body.NodeType) {
                case ExpressionType.Convert:
                    member = ((UnaryExpression) lambda.Body).Operand as MemberExpression;
                    break;

                case ExpressionType.MemberAccess:
                    member = lambda.Body as MemberExpression;
                    break;
            }

            if (member == null) {
                throw new InvalidOperationException ("Not a member expression");
            }

            return member;
        }

        /// <summary>
        /// Retrieves member name by a function expression.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="self">The self function.</param>
        /// <returns>The name of the member.</returns>
        public static string GetMemberName<T> (this Expression<Func<T, object>> self) {
            if (self == null) { return null; }

            return GetExpressionMemberName (self.Body);
        }

        /// <summary>
        /// Retrieves member name by a expression.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="self">The self expression.</param>
        /// <returns>The name of the member.</returns>
        public static string GetMemberName<T> (this Expression<Action<T>> self) {
            if (self == null) { return null; }

            return GetExpressionMemberName (self.Body);
        }

        #endregion Public Static Methods

        #region Private Static Methods

        private static string GetExpressionMemberName (Expression expression) {
            if (expression is MemberExpression memberExpression) {
                return memberExpression.Member.Name;
            }

            if (expression is MethodCallExpression methodCallExpression) {
                return methodCallExpression.Method.Name;
            }

            if (expression is UnaryExpression unaryExpression) {
                return GetUnaryExpressionMemberName (unaryExpression);
            }

            throw new ArgumentException ("Invalid expression.", nameof (expression));
        }

        private static string GetUnaryExpressionMemberName (UnaryExpression expression) => expression.Operand is MethodCallExpression methodCallExpression ?
            methodCallExpression.Method.Name :
            ((MemberExpression) expression.Operand).Member.Name;

        #endregion Private Static Methods
    }
}