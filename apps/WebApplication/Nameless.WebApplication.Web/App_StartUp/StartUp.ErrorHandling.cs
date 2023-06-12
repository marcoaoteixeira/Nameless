using ElmahCore;
using ElmahCore.Mvc;

namespace Nameless.WebApplication.Web {

    public partial class StartUp {

        #region Private Static Methods

        private static void ConfigureErrorHandling(IServiceCollection services, IHostEnvironment hostEnvironment) {
            services.AddElmah<XmlFileErrorLog>(opts => {
                opts.OnPermissionCheck = ctx => (ctx.User.Identity != null && ctx.User.Identity.IsAuthenticated) || hostEnvironment.IsDevelopment();
                opts.Path = "application/logs";
                opts.LogPath = "/logs";
            });
        }

        private static void UseErrorHandling(IApplicationBuilder applicationBuilder, IHostEnvironment hostEnvironment) {
            if (hostEnvironment.IsDevelopment()) {
                applicationBuilder.UseDeveloperExceptionPage();
            }
            applicationBuilder.UseElmah();
        }

        #endregion
    }
}
