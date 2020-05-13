using Microsoft.AspNetCore.Builder;

namespace Nameless.Skeleton.Web {
    public partial class StartUp {
        #region Public Methods

        public void UsingRouting (IApplicationBuilder app) {
            app.UseRouting ();
        }

        #endregion
    }
}