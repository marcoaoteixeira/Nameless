namespace Nameless.Microservice {
    public partial class StartUp {
        #region Private Static Methods

        private static void ConfigureAutoMapper(IServiceCollection services) {
            services.AddAutoMapper(
                assemblies: new[] { typeof(StartUp).Assembly }
            );
        }

        #endregion
    }
}