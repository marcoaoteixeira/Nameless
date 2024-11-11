namespace Nameless.Web.Options;

public sealed class LicenseSettings {
    /// <summary>
    ///     Gets or sets the license name.
    /// </summary>
    public string Name { get; set; } = "MIT";

    /// <summary>
    ///     Gets or sets the license URL.
    /// </summary>
    public string Url { get; set; } = "https://opensource.org/licenses/MIT";
}