using Microsoft.AspNetCore.Builder;

namespace Nameless.Skeleton.Web {
    public partial class StartUp {

        #region Public Methods

        public void UsingCookiePolicy (IApplicationBuilder app) {
            app.UseCookiePolicy ();
        }

        #endregion
    }
}