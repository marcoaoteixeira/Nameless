using ElmahCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Nameless.WebApplication {
    public partial class StartUp {
        #region Private Methods

        private void ConfigureExceptionHandler (IApplicationBuilder app, IHostingEnvironment env) {
            if (!env.IsDevelopment ()) {
                app.UseExceptionHandler ("/Home/Error");
                app.UseElmah ();
            } else { app.UseDeveloperExceptionPage (); }
        }

        #endregion
    }
}
