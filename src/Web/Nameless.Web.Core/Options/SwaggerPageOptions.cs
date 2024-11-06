namespace Nameless.Web.Options;

/// <summary>
///     Represents the Swagger page information that will be present at the top of the page.
/// </summary>
public sealed class SwaggerPageOptions {
    /// <summary>
    ///     Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = "Swagger Page";

    private SwaggerContactSettings? _swaggerContactSettings;
    /// <summary>
    ///     Gets or sets the contact information.
    /// </summary>
    public SwaggerContactSettings Contact {
        get => _swaggerContactSettings ?? new SwaggerContactSettings();
        set => _swaggerContactSettings = value;
    }

    private SwaggerLicenseSettings? _swaggerLicenseSettings;
    /// <summary>
    ///     Gets or sets the license information.
    /// </summary>
    public SwaggerLicenseSettings License {
        get => _swaggerLicenseSettings ??= new SwaggerLicenseSettings();
        set => _swaggerLicenseSettings = value;
    }
}