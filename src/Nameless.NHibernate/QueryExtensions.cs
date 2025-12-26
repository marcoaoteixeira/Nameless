using Nameless.NHibernate.Infrastructure;
using NHibernate;

namespace Nameless.NHibernate;

/// <summary>
/// Extension methods for <see cref="IQuery"/>
/// </summary>
public static class QueryExtensions {
    private static readonly DynamicResultTransformer DynamicTransformer = new();

    /// <param name="self">The source <see cref="global::NHibernate.IQuery" />.</param>
    extension(IQuery self) {
        /// <summary>
        ///     Converts an <see cref="global::NHibernate.IQuery" /> to a dynamic list.
        /// </summary>
        /// <returns>A collection of dynamics, representing the query result.</returns>
        public IList<dynamic> ToDynamicList() {
            return self.SetResultTransformer(DynamicTransformer)
                       .List<dynamic>();
        }
    }
}