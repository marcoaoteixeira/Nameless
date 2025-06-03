namespace Nameless.Security;

/// <summary>
/// Options for configuring security features.
/// </summary>
public sealed record SecurityOptions {
    public RijndaelCryptoOptions RijndaelCryptoOptions { get; set; } = new();
}
