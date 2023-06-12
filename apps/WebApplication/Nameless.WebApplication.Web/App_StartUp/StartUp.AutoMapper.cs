namespace Nameless.WebApplication.Web {

    public partial class StartUp {

        #region Private Static Methods

        private static void ConfigureAutoMapper(IServiceCollection services) {
            services.AddAutoMapper(
                typeof(StartUp).Assembly,
                typeof(WebApplicationModule).Assembly
            );
        }

        #endregion
    }
}
