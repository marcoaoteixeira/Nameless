using System.Linq.Expressions;

namespace Nameless;

/// <summary>
///     <see cref="IQueryable{T}" /> extension methods.
/// </summary>
public static class QueryableExtensions {
    /// <param name="self">The queryable instance.</param>
    /// <typeparam name="T">Type of the query.</typeparam>
    extension<T>(IQueryable<T> self) where T : class {
        /// <summary>
        ///     Orders the queryable result, ascending, by the specified property name that
        ///     is present in the queryable type.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The ordered queryable.</returns>
        public IOrderedQueryable<T> OrderBy(string propertyName) {
            return self.InnerOrderBy(propertyName);
        }

        /// <summary>
        ///     Orders the queryable result, descending, by the specified property name that is present in the queryable type.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The ordered queryable.</returns>
        /// <exception cref="MissingMemberException">
        ///     if <paramref name="propertyName" /> is not present
        ///     in the query type.
        /// </exception>
        public IOrderedQueryable<T> OrderByDescending(string propertyName) {
            return self.InnerOrderBy(propertyName, ascending: false);
        }

        private IOrderedQueryable<T> InnerOrderBy(string propertyName, bool ascending = true) {
            var type = typeof(T);
            var property = type.GetProperty(propertyName)
                           ?? throw new MissingMemberException(
                               $"Property '{propertyName}' not found in type {typeof(T).Name}.");
            var parameter = Expression.Parameter(type, Separators.UNDERSCORE);
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var propertyExpression = Expression.Lambda(propertyAccess, parameter);
            var queryableMethodName = ascending
                ? nameof(Queryable.OrderBy)
                : nameof(Queryable.OrderByDescending);

            var queryExpression = Expression.Call(
                typeof(Queryable),
                queryableMethodName,
                [type, property.PropertyType],
                [self.Expression, Expression.Quote(propertyExpression)]
            );

            var query = self.Provider.CreateQuery<T>(queryExpression);

            return (IOrderedQueryable<T>)query;
        }
    }
}