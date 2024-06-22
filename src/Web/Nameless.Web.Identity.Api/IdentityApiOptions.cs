namespace Nameless.Web.Identity.Api {
    public sealed class IdentityApiOptions {
        #region Public Properties

        /// <summary>
        /// Gets or sets the Identity API base URL. Default value is: api/identity/v{version:apiVersion}
        /// </summary>
        public string BaseUrl { get; set; } = "api/identity/v{version:apiVersion}";

        #endregion
    }
}
