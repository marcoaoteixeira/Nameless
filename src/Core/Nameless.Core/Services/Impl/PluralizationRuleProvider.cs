using System.Globalization;

namespace Nameless.Services.Impl {
    public sealed class PluralizationRuleProvider : IPluralizationRuleProvider {
        #region Public Read-Only Fields

        public readonly PluralizationRuleDelegate DefaultRule = (count) => count >= 1 ? 1 : 0;

        #endregion

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="PluralizationRuleProvider" />.
        /// </summary>
        public static IPluralizationRuleProvider Instance { get; } = new PluralizationRuleProvider();

        #endregion

        #region Private Read-Only Fields

        private readonly Dictionary<string, PluralizationRuleDelegate> _cache = [];

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static PluralizationRuleProvider() { }

        #endregion

        #region Private Constructors

        private PluralizationRuleProvider() {
            AddRule(["ay", "bo", "cgg", "dz", "fa", "id", "ja", "jbo", "ka", "kk", "km", "ko", "ky", "lo", "ms", "my", "sah", "su", "th", "tt", "ug", "vi", "wo", "zh"], n => 0);
            AddRule(["ach", "ak", "am", "arn", "br", "fil", "fr", "gun", "ln", "mfe", "mg", "mi", "oc", "pt-BR", "tg", "ti", "tr", "uz", "wa"], n => n > 1 ? 1 : 0);
            AddRule(["af", "an", "anp", "as", "ast", "az", "bg", "bn", "brx", "ca", "da", "de", "doi", "el", "en", "eo", "es", "es-AR", "et", "eu", "ff", "fi", "fo", "fur", "fy", "gl", "gu", "ha", "he", "hi", "hne", "hu", "hy", "ia", "it", "kl", "kn", "ku", "lb", "mai", "ml", "mn", "mni", "mr", "nah", "nap", "nb", "ne", "nl", "nn", "no", "nso", "or", "pa", "pap", "pms", "ps", "pt", "rm", "rw", "sat", "sco", "sd", "se", "si", "so", "son", "sq", "sv", "sw", "ta", "te", "tk", "ur", "yo"], n => n != 1 ? 1 : 0);
            AddRule(["is"], n => n % 10 != 1 || n % 100 == 11 ? 1 : 0);
            AddRule(["jv"], n => n != 0 ? 1 : 0);
            AddRule(["mk"], n => n == 1 || n % 10 == 1 ? 0 : 1);
            AddRule(["be", "bs", "hr", "lt"], n => n % 10 == 1 && n % 100 != 11 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2);
            AddRule(["cs"], n => (n == 1) ? 0 : (n is>=2 and <=4) ? 1 : 2);
            AddRule(["csb", "pl"], n => (n == 1) ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2);
            AddRule(["lv"], n => n % 10 == 1 && n % 100 != 11 ? 0 : n != 0 ? 1 : 2);
            AddRule(["mnk"], n => n == 0 ? 0 : n == 1 ? 1 : 2);
            AddRule(["ro"], n => n == 1 ? 0 : (n == 0 || (n % 100 > 0 && n % 100 < 20)) ? 1 : 2);
            AddRule(["cy"], n => (n == 1) ? 0 : (n == 2) ? 1 : (n is not 8 and not 11) ? 2 : 3);
            AddRule(["gd"], n => (n is 1 or 11) ? 0 : (n is 2 or 12) ? 1 : (n is>2 and <20) ? 2 : 3);
            AddRule(["kw"], n => (n == 1) ? 0 : (n == 2) ? 1 : (n == 3) ? 2 : 3);
            AddRule(["mt"], n => n == 1 ? 0 : n == 0 || (n % 100 > 1 && n % 100 < 11) ? 1 : (n % 100 is>10 and <20) ? 2 : 3);
            AddRule(["sl"], n => n % 100 == 1 ? 1 : n % 100 == 2 ? 2 : n % 100 is 3 or 4 ? 3 : 0);
            AddRule(["ru", "sr", "uk"], n => n % 10 == 1 && n % 100 != 11 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2);
            AddRule(["sk"], n => (n == 1) ? 0 : (n is>=2 and <=4) ? 1 : 2);
            AddRule(["ga"], n => n == 1 ? 0 : n == 2 ? 1 : (n is>2 and <7) ? 2 : (n is>6 and <11) ? 3 : 4);
            AddRule(["ar"], n => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 is>=3 and <=10 ? 3 : n % 100 >= 11 ? 4 : 5);
        }

        #endregion

        #region Private Methods

        private void AddRule(string[] cultures, PluralizationRuleDelegate rule) {
            foreach (var culture in cultures) {
                _cache.Add(culture, rule);
            }
        }

        #endregion

        #region IPluralizationRuleProvider Members

        /// <inheritdoc />
        public bool TryGet(CultureInfo culture, out PluralizationRuleDelegate? rule) {
            Guard.Against.Null(culture, nameof(culture));

            rule = null;

            if (string.IsNullOrWhiteSpace(culture.Name)) {
                return false;
            }

            return _cache.TryGetValue(culture.Name, out rule)
                || TryGet(culture.Parent, out rule);
        }

        #endregion
    }
}
