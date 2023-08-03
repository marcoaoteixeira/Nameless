using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Nameless.Helpers;

namespace Nameless.Localization.Microsoft.Json.Objects {
    public sealed class Region : IEnumerable<Message> {
        #region Public Static Read-Only Properties

        public static Region Empty => new(string.Empty);

        #endregion

        #region Public Properties

        public string Name { get; }

        #endregion

        #region Internal Properties

        internal Dictionary<string, Message> Messages { get; } = new();

        #endregion

        #region Public Constructors

        public Region(string name) {
            Name = Prevent.Against.Null(name, nameof(name));
        }

        #endregion

        #region Public Methods

        public void Add(Message message) {
            Prevent.Against.Null(message, nameof(message));

            Messages.AddOrChange(message.ID, message);
        }

        public bool TryGetValue(string id, [NotNullWhen(true)] out Message? output)
            => Messages.TryGetValue(id, out output);

        public bool Equals(Region? other)
            => other != null
            && other.Name == Name;

        #endregion

        #region Public Override Methods

        public override bool Equals(object? obj)
            => Equals(obj as Region);

        public override int GetHashCode()
            => SimpleHash.Compute(Name);

        #endregion

        #region IEnumerable<Message> Members

        IEnumerator<Message> IEnumerable<Message>.GetEnumerator()
            => Messages.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Messages.Values.GetEnumerator();

        #endregion
    }
}
