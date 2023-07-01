using MS_IStringLocalizer = Microsoft.Extensions.Localization.IStringLocalizer;

namespace Nameless.Localization.Microsoft.UnitTests.Fixtures {

    public class Service {

        private MS_IStringLocalizer _localizer = default!;
        public MS_IStringLocalizer Localizer {
            get { return _localizer ??= NullStringLocalizer.Instance; }
            set { _localizer = value ?? NullStringLocalizer.Instance; }
        }

        public string Get(string key, params object[] args) {
            return Localizer[key, arguments: args];
        }
    }
}
