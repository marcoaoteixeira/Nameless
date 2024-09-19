using System.Collections;
using System.Dynamic;
using NHibernate.Transform;

namespace Nameless.NHibernate;

/// <summary>
/// Implementation for a dynamic <see cref="IResultTransformer"/>.
/// It uses singleton pattern. See <a href="https://en.wikipedia.org/wiki/Singleton_pattern">Singleton Pattern on Wikipedia</a>
/// </summary>
[Singleton]
public sealed class DynamicResultTransformer : IResultTransformer {
    /// <summary>
    /// Gets the unique instance of <see cref="DynamicResultTransformer" />.
    /// </summary>
    public static IResultTransformer Instance { get; } = new DynamicResultTransformer();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static DynamicResultTransformer() { }

    private DynamicResultTransformer() { }

    public IList TransformList(IList collection)
        => collection;

    public object TransformTuple(object[] tuple, string[] aliases) {
        Prevent.Argument.Null(tuple);
        Prevent.Argument.Null(aliases);

        var result = new Dictionary<string, object>();
        tuple.Each((current, idx) => {
            if (aliases.TryGetElementAt(idx, out var alias)) {
                if (alias is not null) {
                    result[alias] = current;
                }
            }
        });
        return result;
    }
}