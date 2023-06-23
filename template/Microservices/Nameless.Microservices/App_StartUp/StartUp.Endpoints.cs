using Nameless.AspNetCore.Filters;

namespace Nameless.Microservices {
    public partial class StartUp {
        #region Private Static Methods

        private static void ConfigureEndpoints(IServiceCollection services) {
            services.AddControllers(config => {
                config.Filters.Add<ValidateModelStateActionFilter>();
            });
        }

        private static void UseEndpoints(IApplicationBuilder applicationBuilder) {
            applicationBuilder.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        #endregion
    }
}