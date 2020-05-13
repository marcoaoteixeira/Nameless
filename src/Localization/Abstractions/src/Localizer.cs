namespace Nameless.Localization {
    public delegate LocalizedString Localizer (string name, int count = -1, params object[] args);
}