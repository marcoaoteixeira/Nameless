using System.Linq.Expressions;

namespace Nameless {

    /// <summary>
    /// Extension methods for <see cref="IQueryable{T}"/>
    /// </summary>
    public static class QueryableExtension {

        #region Public Static Methods

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> self, string propertyName) where T : class
            => InnerOrderBy(self, propertyName, ascending: true);

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> self, string propertyName) where T : class
            => InnerOrderBy(self, propertyName, ascending: false);

        #endregion

        #region Private Static Methods

        private static IQueryable<T> InnerOrderBy<T>(IQueryable<T> self, string propertyName, bool ascending = true) where T : class {
            Prevent.Null(self, nameof(self));

            var type = typeof(T);
            var property = type.GetProperty(propertyName);
            if (property == default) {
                throw new MissingMemberException($"Property \"{propertyName}\" not found in type {typeof(T).FullName}.");
            }

            var parameter = Expression.Parameter(type, "_");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var propertyExpression = Expression.Lambda(propertyAccess, parameter);
            var queryableMethodName = ascending
                ? nameof(Queryable.OrderBy)
                : nameof(Queryable.OrderByDescending);

            var queryExpression = Expression.Call(
                type: typeof(Queryable),
                methodName: queryableMethodName,
                typeArguments: new[] { type, property.PropertyType },
                arguments: new[] { self.Expression, Expression.Quote(propertyExpression) }
            );

            return self.Provider.CreateQuery<T>(queryExpression);
        }

        #endregion
    }
}
