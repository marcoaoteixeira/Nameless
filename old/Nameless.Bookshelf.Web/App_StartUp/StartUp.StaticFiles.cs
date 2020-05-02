using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace Nameless.Bookshelf.Web {
    public partial class StartUp {
        #region Public Methods

        public void ConfigureStaticFiles (IApplicationBuilder app) {
            var staticFolder = Path.Combine (Directory.GetCurrentDirectory (), "wwwroot");

            // Changed the wwwroot folder to match the Webpack destination build folder, when in development
            // if (env.IsDevelopment ()) { staticFolder = Path.Combine (staticFolder, "dist"); }

            app.UseFileServer (new FileServerOptions {
                FileProvider = new PhysicalFileProvider (staticFolder),
                EnableDirectoryBrowsing = false
            });
        }

        #endregion
    }
}