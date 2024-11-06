using System.Collections;
using NHibernate.Transform;

namespace Nameless.NHibernate;

public sealed class DynamicResultTransformer : IResultTransformer {
    public IList TransformList(IList collection)
        => collection;

    public object TransformTuple(object[] tuple, string[] aliases) {
        Prevent.Argument.Null(tuple);
        Prevent.Argument.Null(aliases);

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