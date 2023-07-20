namespace Nameless.Localization.Json.UnitTest.Fixtures {

    public class Service {

        private IStringLocalizer? _localizer;
        public IStringLocalizer Localizer {
            get { return _localizer ??= NullStringLocalizer.Instance; }
            set { _localizer = value; }
        }

        public string Get(string key, params object[] args) {
            return Localizer[key, args: args].GetTranslation();
        }
    }
}
