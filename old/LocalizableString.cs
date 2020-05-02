using Newtonsoft.Json;

namespace Nameless.Localization.Json {
    public class LocalizableString {
        #region Public Properties

        [JsonProperty ("name")]
        public string Name { get; set; }
        [JsonProperty ("value")]
        public string Value { get; set; }
        [JsonIgnore]
        public string Culture { get; set; }

        #endregion

        #region Public Methods

        public bool Equals (LocalizableString obj) {
            return obj != null &&
                obj.Name == Name &&
                obj.Value == Value &&
                obj.Culture == Culture;
        }

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) {
            return Equals (obj as LocalizableString);
        }

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += (Name ?? string.Empty).GetHashCode () * 7;
                hash += (Value ?? string.Empty).GetHashCode () * 7;
                hash += (Culture ?? string.Empty).GetHashCode () * 7;
            }
            return hash;
        }

        public override string ToString () {
            return Value ?? Name;
        }

        #endregion

        #region Public Implicit Operators

        public static implicit operator string (LocalizableString obj) {
            return obj.Value ?? obj.Name;
        }

        #endregion
    }
}