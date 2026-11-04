using Nameless.Windows.Localization;

namespace Nameless.WinApp.Infrastructure;

internal static class TranslatorExtensions {
    extension(ITranslator self) {
        public string Get(string key, params object[] parameters) {
            var entry = CallerInfo.GetKey(key, skipFrame: 1);

            return self[entry, parameters];
        }
    }
}
