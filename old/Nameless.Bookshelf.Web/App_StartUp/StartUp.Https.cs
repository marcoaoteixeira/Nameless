using Microsoft.AspNetCore.Builder;

namespace Nameless.Bookshelf.Web {
    public partial class StartUp {
        #region Public Methods

        public void ConfigureHttps (IApplicationBuilder app) {
            app.UseHttpsRedirection ();
        }

        #endregion
    }
}