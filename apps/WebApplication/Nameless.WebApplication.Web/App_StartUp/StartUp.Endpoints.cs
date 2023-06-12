using Nameless.AspNetCore.Filters;

namespace Nameless.WebApplication.Web {

    public partial class StartUp {

        #region Private Static Methods

        private static void ConfigureEndpoints(IServiceCollection services) {
            services
                .AddControllers(config => config.Filters.Add<ValidateModelStateActionFilter>())
                .AddNewtonsoftJson();
        }

        private static void UseEndpoints(IApplicationBuilder applicationBuilder) {
            applicationBuilder.UseEndpoints(_ => _.MapControllers());
        }

        #endregion
    }
}
