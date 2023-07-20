namespace Nameless.Microservice {
    public partial class StartUp {
        #region Private Static Methods

        private static void ConfigureAuth(IServiceCollection services, IConfiguration config) {
        }

        private static void UseAuth(IApplicationBuilder app) {
            app.UseAuthentication();
        }

        #endregion
    }
}