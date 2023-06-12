namespace Nameless.WebApplication.Web {

    public partial class StartUp {

        #region Private Static Methods

        private static void ConfigureRouting(IServiceCollection services) {
            services.AddRouting();
        }

        private static void UseRouting(IApplicationBuilder applicationBuilder) {
            applicationBuilder.UseRouting();
        }

        #endregion
    }
}
