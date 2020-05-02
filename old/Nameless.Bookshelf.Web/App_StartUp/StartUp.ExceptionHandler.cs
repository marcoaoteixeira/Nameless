using ElmahCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Nameless.Bookshelf.Web {
    public partial class StartUp {
        #region Private Methods

        private void ConfigureExceptionHandler (IApplicationBuilder app, IWebHostEnvironment env) {
            if (!env.IsDevelopment ()) {
                app.UseExceptionHandler ("/Home/Error");
                app.UseElmah ();
            } else { app.UseDeveloperExceptionPage (); }
        }

        #endregion
    }
}