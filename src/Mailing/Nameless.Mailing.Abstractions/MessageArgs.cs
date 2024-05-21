using Nameless.Infrastructure;

namespace Nameless.Mailing {
    public sealed class MessageArgs : ArgCollection {
        #region Private Constants

        private const string USE_HTML_BODY = nameof(USE_HTML_BODY);
        private const string CARBON_COPY = nameof(CARBON_COPY);
        private const string BLIND_CARBON_COPY = nameof(BLIND_CARBON_COPY);

        #endregion

        #region Public Static Methods

        public bool GetUseHtmlBody() {
            var arg = Get(USE_HTML_BODY) ?? false;

            return (bool)arg;
        }

        public void SetUseHtmlBody(bool value)
            => Set(USE_HTML_BODY, value);

        public string GetCarbonCopy() {
            var arg = Get(CARBON_COPY) ?? string.Empty;

            return (string)arg;
        }

        public void SetCarbonCopy(string value)
            => Set(CARBON_COPY, value);

        public string GetBlindCarbonCopy() {
            var arg = Get(BLIND_CARBON_COPY) ?? string.Empty;

            return (string)arg;
        }

        public void SetBlindCarbonCopy(string value)
            => Set(BLIND_CARBON_COPY, value);

        #endregion
    }
}
