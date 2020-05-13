using ElmahCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Nameless.Skeleton.Web {
    public partial class StartUp {
        #region Public Methods

        public void UsingErrorHandling (IApplicationBuilder app, IWebHostEnvironment env) {
            if (!env.IsDevelopment ()) {
                app.UseExceptionHandler ("/Home/Error");
                app.UseElmah ();
            } else { app.UseDeveloperExceptionPage (); }
        }

        #endregion
    }
}