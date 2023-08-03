using Microsoft.Extensions.Localization;

namespace Nameless.Localization.Microsoft.Json.UnitTest.Fixtures {

    public class Service {

        private IStringLocalizer _localizer = default!;
        public IStringLocalizer Localizer {
            get => _localizer ??= NullStringLocalizer.Instance;
            set => _localizer = value;
        }

        public string Get(string key, params object[] args) => Localizer[key, arguments: args];
    }
}
