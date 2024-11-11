namespace Nameless.Web.Options;

/// <summary>
///     Represents the Swagger page information that will be present at the top of the page.
/// </summary>
public sealed class SwaggerPageOptions {
    /// <summary>
    ///     Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = "Swagger Page";

    private ContactSettings? _swaggerContactSettings;
    /// <summary>
    ///     Gets or sets the contact information.
    /// </summary>
    public ContactSettings Contact {
        get => _swaggerContactSettings ?? new ContactSettings();
        set => _swaggerContactSettings = value;
    }

    private LicenseSettings? _swaggerLicenseSettings;
    /// <summary>
    ///     Gets or sets the license information.
    /// </summary>
    public LicenseSettings License {
        get => _swaggerLicenseSettings ??= new LicenseSettings();
        set => _swaggerLicenseSettings = value;
    }
}