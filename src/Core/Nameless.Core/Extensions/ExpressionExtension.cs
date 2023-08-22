using System.Linq.Expressions;

namespace Nameless {
    /// <summary>
    /// <see cref="Expression"/> extension methods.
    /// </summary>
    public static class ExpressionExtension {
        #region Public Static Methods

        /// <summary>
        /// Retrieves the member info.
        /// </summary>
        /// <param name="self">The method.</param>
        /// <returns>An instance of <see cref="MemberExpression"/> representing the <paramref name="self"/> expression.</returns>
        /// <remarks>Used to get property information</remarks>
        /// <exception cref="ArgumentException">if <paramref name="self"/> is not a <see cref="LambdaExpression"/>.</exception>
        public static MemberExpression? GetMemberInfo(this Expression self) {
            if (self is not LambdaExpression lambda) {
                throw new ArgumentException("Not a lambda expression", nameof(self));
            }

            MemberExpression? member = default;
            switch (lambda.Body.NodeType) {
                case ExpressionType.Convert:
                    member = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                    break;

                case ExpressionType.MemberAccess:
                    member = lambda.Body as MemberExpression;
                    break;
            }

            return member;
        }

        /// <summary>
        /// Retrieves member name by a function expression.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="self">The self function.</param>
        /// <returns>The name of the member.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string? GetMemberName<T>(this Expression<Func<T, object>> self)
            => GetExpressionMemberName(self.Body);

        /// <summary>
        /// Retrieves member name by a expression.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="self">The self expression.</param>
        /// <returns>The name of the member.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string? GetMemberName<T>(this Expression<Action<T>> self)
            => GetExpressionMemberName(self.Body);

        /// <summary>
        /// Creates an AND condition with another expression.
        /// </summary>
        /// <typeparam name="T">Type of the expression.</typeparam>
        /// <param name="self">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>An expression composition.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> or <paramref name="right"/> is <c>null</c>.</exception>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> self, Expression<Func<T, bool>> right) {
            Guard.Against.Null(right, nameof(right));

            var param = Expression.Parameter(typeof(T), "_");
            var body = Expression.And(
                left: Expression.Invoke(self, param),
                right: Expression.Invoke(right, param)
            );
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        /// <summary>
        /// Creates an OR condition with another expression.
        /// </summary>
        /// <typeparam name="T">Type of the expression.</typeparam>
        /// <param name="self">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>An expression composition.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> or <paramref name="right"/> is <c>null</c>.</exception>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> self, Expression<Func<T, bool>> right) {
            Guard.Against.Null(self, nameof(self));
            Guard.Against.Null(right, nameof(right));

            var param = Expression.Parameter(typeof(T), "_");
            var body = Expression.Or(
                left: Expression.Invoke(self, param),
                right: Expression.Invoke(right, param)
            );
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        #endregion

        #region Private Static Methods

        private static string GetExpressionMemberName(Expression expression)
            => expression switch {
                MemberExpression => ((MemberExpression)expression).Member.Name,
                MethodCallExpression => ((MethodCallExpression)expression).Method.Name,
                UnaryExpression => GetUnaryExpressionMemberName((UnaryExpression)expression),
                _ => throw new ArgumentException("Invalid expression.", nameof(expression)),
            };

        private static string GetUnaryExpressionMemberName(UnaryExpression expression)
            => expression.Operand switch {
                MethodCallExpression => ((MethodCallExpression)expression.Operand).Method.Name,
                _ => ((MemberExpression)expression.Operand).Member.Name,
            };

        #endregion
    }
}