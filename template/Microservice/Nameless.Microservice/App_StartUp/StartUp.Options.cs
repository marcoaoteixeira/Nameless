using Nameless.AspNetCore;
using Nameless.AspNetCore.Options;

namespace Nameless.Microservice {
    public partial class StartUp {
        #region Private Static Methods

        private static void ConfigureOptions(IServiceCollection services, IConfiguration config) {
            services.AddOptions();

            services.PushOptions<SwaggerPageOptions>(config);
        }

        #endregion
    }
}