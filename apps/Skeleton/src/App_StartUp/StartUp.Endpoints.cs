using Microsoft.AspNetCore.Builder;

namespace Nameless.Skeleton.Web {
    public partial class StartUp {
        #region Public Methods

        public void UsingEndpoints (IApplicationBuilder app) {
            app.UseEndpoints (endpoints => {
                endpoints.MapControllerRoute (
                    name: "WebApi_Area",
                    pattern: "{area}/api/v{version:apiVersion}/{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapControllerRoute (
                    name: "WebApi",
                    pattern: "api/v{version:apiVersion}/{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapControllerRoute (
                    name: "Mvc_Area",
                    pattern: "{area}/{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapControllerRoute (
                    name: "Mvc",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapRazorPages ();
            });
        }

        #endregion
    }
}