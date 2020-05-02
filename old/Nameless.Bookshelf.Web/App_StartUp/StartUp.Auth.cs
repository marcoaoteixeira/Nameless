using Microsoft.AspNetCore.Builder;

namespace Nameless.Bookshelf.Web {
    public partial class StartUp {
        #region Private Methods

        private void ConfigureAuth (IApplicationBuilder app) {
            app.UseAuthorization ();
        }

        #endregion
    }
}