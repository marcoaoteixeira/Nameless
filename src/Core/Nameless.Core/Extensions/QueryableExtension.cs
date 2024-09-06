using System.Linq.Expressions;

namespace Nameless;

/// <summary>
/// <see cref="IQueryable{T}"/> extension methods.
/// </summary>
public static class QueryableExtension {
    /// <summary>
    /// Orders the queryable result, ascending, by the specified property name that
    /// is present in the queryable type.
    /// </summary>
    /// <typeparam name="T">Type of the query.</typeparam>
    /// <param name="self">The queryable instance.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>The ordered queryable.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> or
    /// <paramref name="propertyName"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="propertyName"/> is empty or white spaces.
    /// </exception>
    /// <exception cref="MissingMemberException">
    /// if <paramref name="propertyName"/> is not present
    /// in the query type.
    /// </exception>
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> self, string propertyName)
        where T : class
        => InnerOrderBy(queryable: Prevent.Argument.Null(self, nameof(self)),
                        propertyName: propertyName,
                        ascending: true);

    /// <summary>
    /// Orders the queryable result, descending, by the specified property name that is present in the queryable type.
    /// </summary>
    /// <typeparam name="T">Type of the query.</typeparam>
    /// <param name="self">The queryable instance.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>The ordered queryable.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> or
    /// <paramref name="propertyName"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="propertyName"/> is empty or white spaces.
    /// </exception>
    /// <exception cref="MissingMemberException">
    /// if <paramref name="propertyName"/> is not present
    /// in the query type.
    /// </exception>
    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> self, string propertyName)
        where T : class
        => InnerOrderBy(queryable: Prevent.Argument.Null(self, nameof(self)),
                        propertyName: propertyName,
                        ascending: false);

    private static IOrderedQueryable<T> InnerOrderBy<T>(IQueryable<T> queryable, string propertyName, bool ascending = true)
        where T : class {
        var type = typeof(T);
        var property = type.GetProperty(propertyName)
                    ?? throw new MissingMemberException($"Property \"{propertyName}\" not found in type {typeof(T).FullName}.");
        var parameter = Expression.Parameter(type, "_");
        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        var propertyExpression = Expression.Lambda(propertyAccess, parameter);
        var queryableMethodName = ascending
            ? nameof(Queryable.OrderBy)
            : nameof(Queryable.OrderByDescending);

        var queryExpression = Expression.Call(type: typeof(Queryable),
                                              methodName: queryableMethodName,
                                              typeArguments: [type, property.PropertyType],
                                              arguments: [queryable.Expression, Expression.Quote(propertyExpression)]);

        var query = queryable.Provider.CreateQuery<T>(queryExpression);

        return (IOrderedQueryable<T>)query;
    }
}