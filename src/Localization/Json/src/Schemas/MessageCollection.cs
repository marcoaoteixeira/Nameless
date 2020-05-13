using System;
using System.Collections.Generic;
using System.Linq;

namespace Nameless.Localization.Json.Schemas {
    public sealed class MessageCollection {
        #region Private Properties

        private IDictionary<string, Message> Dictionary { get; }

        #endregion

        #region Public Properties

        public string Name { get; }
        public IEnumerable<Message> Messages => Dictionary.Values;

        #endregion

        #region Public Constructors

        public MessageCollection (string name, Message[] messages) {
            Prevent.ParameterNullOrWhiteSpace (name, nameof (name));
            Prevent.ParameterNullOrEmpty (messages, nameof (messages));

            Name = name;
            Dictionary = messages.ToDictionary (key => key.Text, value => value);
        }

        #endregion

        #region Public Methods

        public LocalizedString GetTranslation (string text, int count = -1, PluralizationRuleDelegate pluralizationRule = null, params object[] args) {
            return Dictionary.TryGetValue (text, out Message message) ?
                message.GetTranslation (text, count, pluralizationRule, args) :
                new LocalizedString (text, text, args);
        }

        public string[] GetTranslationOptions (string text) {
            return Dictionary.TryGetValue (text, out Message message) ?
                message.Translations :
                Array.Empty<string> ();
        }

        public bool Equals (MessageCollection obj) => obj != null && obj.Name == Name;

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) => Equals (obj as MessageCollection);

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += (Name ?? string.Empty).GetHashCode () * 7;
            }
            return hash;
        }

        public override string ToString () => Name;

        #endregion
    }
}