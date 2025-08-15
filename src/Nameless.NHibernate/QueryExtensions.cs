using Nameless.NHibernate.Infrastructure;
using NHibernate;

namespace Nameless.NHibernate;

/// <summary>
/// Extension methods for <see cref="IQuery"/>
/// </summary>
public static class QueryExtensions {
    private static readonly DynamicResultTransformer DynamicTransformer = new();

    /// <summary>
    ///     Converts an <see cref="T:NHibernate.IQuery" /> to a dynamic list.
    /// </summary>
    /// <param name="self">The source <see cref="T:NHibernate.IQuery" />.</param>
    /// <returns>A collection of dynamics, representing the query result.</returns>
    public static IList<dynamic> ToDynamicList(this IQuery self) {
        return self.SetResultTransformer(DynamicTransformer)
                   .List<dynamic>();
    }
}