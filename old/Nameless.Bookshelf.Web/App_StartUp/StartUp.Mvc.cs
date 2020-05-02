using Microsoft.AspNetCore.Builder;

namespace Nameless.Bookshelf.Web {
    public partial class StartUp {
        #region Public Methods

        public void ConfigureMvc (IApplicationBuilder app) {
            app.UseEndpoints (endpoints => {
                endpoints.MapControllerRoute (
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        #endregion
    }
}