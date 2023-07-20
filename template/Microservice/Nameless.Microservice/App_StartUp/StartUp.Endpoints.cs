using Nameless.AspNetCore.Filters;

namespace Nameless.Microservice {
    public partial class StartUp {
        #region Private Static Methods

        private static void ConfigureEndpoints(IServiceCollection services) {
            services.AddControllers(config => {
                config.Filters.Add<ValidateModelStateActionFilter>();
            });
        }

        private static void UseEndpoints(IApplicationBuilder app) {
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        #endregion
    }
}