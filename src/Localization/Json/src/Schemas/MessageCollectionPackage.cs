using System.Collections.Generic;
using System.Linq;

namespace Nameless.Localization.Json.Schemas {
    /// <summary>
    /// The message collection aggregation class.
    /// </summary>
    /// <example>
    /// <code>
    /// {
    ///     "[AssemblyName] TypeFullName": {
    ///         "Message": [ "Option 1", "Option 2", ..., "Option N" ]
    ///     }
    /// }
    /// </code>
    /// </example>
    public sealed class MessageCollectionPackage {
        #region Private Properties

        private IDictionary<string, MessageCollection> MessageCollections { get; }

        #endregion

        #region Public Properties

        public string CultureName { get; }

        #endregion

        #region Public Constructors

        public MessageCollectionPackage (string cultureName, MessageCollection[] messageCollections) {
            Prevent.ParameterNullOrWhiteSpace (cultureName, nameof (cultureName));
            Prevent.ParameterNullOrEmpty (messageCollections, nameof (messageCollections));

            CultureName = cultureName;
            MessageCollections = messageCollections.ToDictionary (key => key.Name, value => value);
        }

        #endregion

        #region Public Methods

        public bool TryGetMessageCollection (string messageCollectionName, out MessageCollection messageCollection) => MessageCollections.TryGetValue (messageCollectionName, out messageCollection);

        public bool Equals (MessageCollectionPackage obj) => obj != null && obj.CultureName == CultureName;

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) => Equals (obj as MessageCollectionPackage);

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += CultureName.GetHashCode () * 7;
            }
            return hash;
        }

        public override string ToString () => CultureName;

        #endregion
    }
}