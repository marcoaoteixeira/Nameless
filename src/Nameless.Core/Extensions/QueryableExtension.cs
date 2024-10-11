using System.Linq.Expressions;
using Nameless.Collections.Generic;

namespace Nameless;

/// <summary>
/// <see cref="IQueryable{T}"/> extension methods.
/// </summary>
public static class QueryableExtension {
    /// <summary>
    /// Returns the current <see cref="IQueryable{T}"/> as a <see cref="IPaginable{T}"/> object.
    /// </summary>
    /// <typeparam name="T">Type of the query items.</typeparam>
    /// <param name="self">The current <see cref="IQueryable{T}"/></param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>
    /// A <see cref="IPaginable{T}"/> object
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// if <paramref name="pageSize"/> is lower or equal to 0 (zero).
    /// </exception>
    public static IPaginable<T> AsPaginable<T>(this IQueryable<T> self, int pageSize) {
        Prevent.Argument.Null(self);
        Prevent.Argument.LowerOrEqual(pageSize, to: 0);
        
        return new Paginable<T>(self, pageSize);
    }

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
        => InnerOrderBy(queryable: Prevent.Argument.Null(self),
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
        => InnerOrderBy(queryable: Prevent.Argument.Null(self),
                        propertyName: propertyName,
                        ascending: false);

    private static IOrderedQueryable<T> InnerOrderBy<T>(IQueryable<T> queryable, string propertyName, bool ascending = true)
        where T : class {
        var type = typeof(T);
        var property = type.GetProperty(propertyName)
                    ?? throw new MissingMemberException($"Property \"{propertyName}\" not found in type {typeof(T).FullName}.");
        var parameter = Expression.Parameter(type, Constants.Separators.UNDERSCORE);
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