using Nameless.Infrastructure;

namespace Nameless.Mailing;

public sealed class MessageArgs : ArgCollection {
    private const string USE_HTML_BODY = nameof(USE_HTML_BODY);
    private const string CARBON_COPY = nameof(CARBON_COPY);
    private const string BLIND_CARBON_COPY = nameof(BLIND_CARBON_COPY);

    public bool GetUseHtmlBody() {
        var arg = this[USE_HTML_BODY] ?? false;

        return (bool)arg;
    }

    public void SetUseHtmlBody(bool value)
        => this[USE_HTML_BODY] = value;

    public string GetCarbonCopy() {
        var arg = this[CARBON_COPY] ?? string.Empty;

        return (string)arg;
    }

    public void SetCarbonCopy(string value)
        => this[CARBON_COPY] = value;

    public string GetBlindCarbonCopy() {
        var arg = this[BLIND_CARBON_COPY] ?? string.Empty;

        return (string)arg;
    }

    public void SetBlindCarbonCopy(string value)
        => this[BLIND_CARBON_COPY] = value;
}