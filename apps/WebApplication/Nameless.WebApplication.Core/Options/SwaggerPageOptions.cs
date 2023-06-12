namespace Nameless.WebApplication.Options {

    public sealed class SwaggerPageOptions {

        #region Public Static Read-Only Properties

        public static SwaggerPageOptions Default => new();

        #endregion

        #region Public Properties

        public string Description { get; set; } = "Web Application Swagger API";
        public Contact Contact { get; set; } = new();
        public License License { get; set; } = new();

        #endregion
    }

    public sealed class Contact {

        #region Public Properties

        public string Name { get; set; } = "Web Application Administration";
        public string Email { get; set; } = "administrator@webapplication.com";
        public string Url { get; set; } = "https://www.webapplication.com/";

        #endregion
    }

    public sealed class License {

        #region Public Properties

        public string Name { get; set; } = "MIT";
        public string Url { get; set; } = "https://opensource.org/licenses/MIT";

        #endregion
    }
}
