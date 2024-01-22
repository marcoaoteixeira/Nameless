using System.Collections;
using System.Dynamic;
using NHibernate.Transform;

namespace Nameless.NHibernate {
    /// <summary>
    /// Singleton Pattern implementation for <see cref="DynamicResultTransformer" />. (see: https://en.wikipedia.org/wiki/Singleton_pattern)
    /// </summary>
    [Singleton]
    public sealed class DynamicResultTransformer : IResultTransformer {
        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="DynamicResultTransformer" />.
        /// </summary>
        public static IResultTransformer Instance { get; } = new DynamicResultTransformer();

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static DynamicResultTransformer() { }

        #endregion

        #region Private Constructors

        private DynamicResultTransformer() { }

        #endregion

        #region IResultTransformer Members

        public IList TransformList(IList collection)
            => collection;

        public object TransformTuple(object[] tuple, string[] aliases) {
            Guard.Against.Null(tuple, nameof(tuple));
            Guard.Against.Null(aliases, nameof(aliases));

            var result = new ExpandoObject() as IDictionary<string, object>;
            tuple.Each((current, idx) => {
                if (aliases.TryElementAt(idx, out var alias)) {
                    if (alias is not null) {
                        result[alias] = current;
                    }
                }
            });
            return result;
        }

        #endregion
    }
}
