namespace Nameless.Localization.Json.UnitTests.Fixtures {

    public class Service {

        private IStringLocalizer _localizer = default!;
        public IStringLocalizer Localizer {
            get { return _localizer ??= NullStringLocalizer.Instance; }
            set { _localizer = value ?? NullStringLocalizer.Instance; }
        }

        public string Get(string key, params object[] args) {
            return Localizer[key, args: args].GetTranslation();
        }
    }
}
