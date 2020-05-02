using Microsoft.AspNetCore.Builder;

namespace Nameless.WebApplication.Web {
    public partial class StartUp {
        #region Public Methods

        public void ConfigureMvc (IApplicationBuilder app) {
            app.UseMvc (routes => {
                routes.MapRoute (
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }

        #endregion
    }
}