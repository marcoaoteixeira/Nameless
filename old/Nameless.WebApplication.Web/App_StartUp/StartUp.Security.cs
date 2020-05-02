using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Nameless.WebApplication.Web {
    public partial class StartUp {
        #region Private Methods

        private void ConfigureSecurity (IApplicationBuilder app, IHostingEnvironment env) {
            if (!env.IsDevelopment ()) {
                // The default HSTS value is 30 days.
                // You may want to change this for production scenarios,
                // see https://aka.ms/aspnetcore-hsts.
                app.UseHsts ();
            }
        }

        #endregion
    }
}