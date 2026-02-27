using System.Collections;
using NHibernate.Transform;

namespace Nameless.NHibernate.Infrastructure;

/// <summary>
/// Dynamic result transformer.
/// </summary>
public class DynamicResultTransformer : IResultTransformer {
    /// <summary>
    /// Transforms a <see cref="IList"/> object.
    /// </summary>
    /// <param name="collection">The <see cref="IList"/> object.</param>
    /// <returns>
    /// A transformed version of the <paramref name="collection"/>.
    /// </returns>
    public IList TransformList(IList collection) {
        return collection;
    }

    /// <summary>
    /// Transforms a tuple object into an unique value.
    /// </summary>
    /// <param name="tuple">The tuple.</param>
    /// <param name="aliases">The alias.</param>
    /// <returns>
    /// The transformed <see cref="object"/>.
    /// </returns>
    public object TransformTuple(object[] tuple, string[] aliases) {
        Throws.When.Null(tuple);
        Throws.When.Null(aliases);

        var result = new Dictionary<string, object>();
        tuple.Each((current, idx) => {
            if (!aliases.TryGetElementAt(idx, out var alias)) {
                return;
            }

            if (alias is not null) {
                result[alias] = current;
            }
        });
        return result;
    }
}