using Microsoft.AspNetCore.Builder;

namespace Nameless.WebApplication {
    public partial class StartUp {
        #region Public Methods

        public void ConfigureCors (IApplicationBuilder app) {
            app.UseCors (policy => {
                policy
                    .AllowAnyOrigin ()
                    .AllowAnyMethod ()
                    .AllowAnyHeader ();
            });
        }

        #endregion
    }
}