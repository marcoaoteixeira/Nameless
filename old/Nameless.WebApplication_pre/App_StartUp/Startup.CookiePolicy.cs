using Microsoft.AspNetCore.Builder;

namespace Nameless.WebApplication {
    public partial class StartUp {

        #region Public Methods

        public void ConfigureCookiePolicy (IApplicationBuilder app) {
            app.UseCookiePolicy ();
        }

        #endregion
    }
}
