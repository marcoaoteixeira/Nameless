namespace Nameless.Web.Options;

public sealed class ContactSettings {
    /// <summary>
    ///     Gets or sets the contact name.
    /// </summary>
    public string Name { get; set; } = "Swagger";

    /// <summary>
    ///     Gets or sets the contact e-mail.
    /// </summary>
    public string Email { get; set; } = "swagger@swagger.com";

    /// <summary>
    ///     Gets or sets the contact URL.
    /// </summary>
    public string Url { get; set; } = "http://localhost/swagger/";
}