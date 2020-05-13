using Microsoft.AspNetCore.Builder;

namespace Nameless.Skeleton.Web {
    public partial class StartUp {
        #region Public Methods

        public void UsingAuth (IApplicationBuilder app) {
            app.UseAuthentication ();
            app.UseAuthorization ();
        }

        #endregion
    }
}