namespace Nameless.Messenger.Email {
    public static class MessengerArgsExtension {
        #region Private Constants

        private const string USE_HTML_BODY = nameof(USE_HTML_BODY);
        private const string CARBON_COPY = nameof(CARBON_COPY);
        private const string BLIND_CARBON_COPY = nameof(BLIND_CARBON_COPY);

        #endregion

        #region Public Static Methods

        public static bool GetUseHtmlBody(this MessageArgs self) {
            var arg = self.Get(USE_HTML_BODY) ?? false;

            return (bool)arg;
        }

        public static void SetUseHtmlBody(this MessageArgs self, bool value)
            => self.Set(USE_HTML_BODY, value);

        public static string GetCarbonCopy(this MessageArgs self) {
            var arg = self.Get(CARBON_COPY) ?? string.Empty;

            return (string)arg;
        }

        public static void SetCarbonCopy(this MessageArgs self, string value)
            => self.Set(CARBON_COPY, value);

        public static string GetBlindCarbonCopy(this MessageArgs self) {
            var arg = self.Get(BLIND_CARBON_COPY) ?? string.Empty;

            return (string)arg;
        }

        public static void SetBlindCarbonCopy(this MessageArgs self, string value)
            => self.Set(BLIND_CARBON_COPY, value);

        #endregion
    }
}
