namespace Nameless.Microservice {
    public partial class StartUp {
        #region Private Static Methods

        private static void ConfigureRouting(IServiceCollection services) {
            services.AddRouting();
        }

        private static void UseRouting(IApplicationBuilder app) {
            app.UseRouting();
        }

        #endregion
    }
}