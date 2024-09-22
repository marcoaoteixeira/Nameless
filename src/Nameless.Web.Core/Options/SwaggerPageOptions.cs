namespace Nameless.Web.Options;

/// <summary>
/// Represents the Swagger page information that will be present at the top of the page.
/// </summary>
public sealed class SwaggerPageOptions {
    /// <summary>
    /// Gets the default instance of <see cref="SwaggerPageOptions"/>.
    /// </summary>
    public static SwaggerPageOptions Default => new();

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = "Swagger Page";
    /// <summary>
    /// Gets or sets the contact information.
    /// </summary>
    public SwaggerContactOptions Contact { get; set; } = new();
    /// <summary>
    /// Gets or sets the license information.
    /// </summary>
    public SwaggerLicenseOptions License { get; set; } = new();
}

public sealed class SwaggerContactOptions {
    /// <summary>
    /// Gets or sets the contact name.
    /// </summary>
    public string Name { get; set; } = "Swagger";
    /// <summary>
    /// Gets or sets the contact e-mail.
    /// </summary>
    public string Email { get; set; } = "swagger@swagger.com";
    /// <summary>
    /// Gets or sets the contact URL.
    /// </summary>
    public string Url { get; set; } = "http://localhost/swagger/";
}

public sealed class SwaggerLicenseOptions {
    /// <summary>
    /// Gets or sets the license name.
    /// </summary>
    public string Name { get; set; } = "MIT";
    /// <summary>
    /// Gets or sets the license URL.
    /// </summary>
    public string Url { get; set; } = "https://opensource.org/licenses/MIT";
}