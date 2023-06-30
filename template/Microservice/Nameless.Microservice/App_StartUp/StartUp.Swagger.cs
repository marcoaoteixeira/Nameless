namespace Nameless.Microservice {
    public partial class StartUp {
        #region Private Static Methods

        private static void ConfigureSwagger(IServiceCollection services) {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        private static void UseSwagger(IApplicationBuilder applicationBuilder, IHostEnvironment hostEnvironment) {
            if (hostEnvironment.IsDevelopment()) {
                applicationBuilder.UseSwagger();
                applicationBuilder.UseSwaggerUI();
            }
        }

        #endregion
    }
}