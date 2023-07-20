namespace Nameless.Localization.Microsoft.UnitTest.Fixtures {

    public class Service {

        private IMSStringLocalizer _localizer = default!;
        public IMSStringLocalizer Localizer {
            get { return _localizer ??= MSNullStringLocalizer.Instance; }
            set { _localizer = value; }
        }

        public string Get(string key, params object[] args) {
            return Localizer[key, arguments: args];
        }
    }
}
