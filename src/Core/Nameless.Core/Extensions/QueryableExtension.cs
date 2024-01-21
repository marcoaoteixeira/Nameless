using System.Linq.Expressions;

namespace Nameless {
    /// <summary>
    /// <see cref="IQueryable{T}"/> extension methods.
    /// </summary>
    public static class QueryableExtension {
        #region Public Static Methods

        /// <summary>
        /// Orders the queryable result, ascending, by the specified property name that is present in the queryable type.
        /// </summary>
        /// <typeparam name="T">Type of the query.</typeparam>
        /// <param name="self">The queryable instance.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The ordered queryable.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> self, string propertyName) where T : class
            => InnerOrderBy(self, propertyName, ascending: true);

        /// <summary>
        /// Orders the queryable result, descending, by the specified property name that is present in the queryable type.
        /// </summary>
        /// <typeparam name="T">Type of the query.</typeparam>
        /// <param name="self">The queryable instance.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The ordered queryable.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> self, string propertyName) where T : class
            => InnerOrderBy(self, propertyName, ascending: false);

        #endregion

        #region Private Static Methods

        private static IOrderedQueryable<T> InnerOrderBy<T>(IQueryable<T> self, string propertyName, bool ascending = true) where T : class {
            var type = typeof(T);
            var property = type.GetProperty(propertyName)
                ?? throw new MissingMemberException($"Property \"{propertyName}\" not found in type {typeof(T).FullName}.");
            var parameter = Expression.Parameter(type, "_");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var propertyExpression = Expression.Lambda(propertyAccess, parameter);
            var queryableMethodName = ascending
                ? nameof(Queryable.OrderBy)
                : nameof(Queryable.OrderByDescending);

            var queryExpression = Expression.Call(
                type: typeof(Queryable),
                methodName: queryableMethodName,
                typeArguments: [type, property.PropertyType],
                arguments: [self.Expression, Expression.Quote(propertyExpression)]
            );

            var query = self.Provider.CreateQuery<T>(queryExpression);

            return (IOrderedQueryable<T>)query;
        }

        #endregion
    }
}
