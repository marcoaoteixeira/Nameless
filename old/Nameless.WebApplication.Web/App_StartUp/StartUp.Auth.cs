using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Nameless.WebApplication.Web {
    public partial class StartUp {
        #region Private Methods

        private void ConfigureAuth (IApplicationBuilder app, IHostingEnvironment env) {
            app.UseAuthentication ();
        }

        #endregion
    }
}