namespace Nameless.Microservice {
    public partial class StartUp {
        #region Private Static Methods

        private static void ConfigureCors(IServiceCollection serviceCollection) {
            // CORS defines a way in which a browser and server can
            // interact to determine whether or not it is safe to
            // allow the cross-origin request.
            serviceCollection.AddCors();
        }

        private static void UseCors(IApplicationBuilder applicationBuilder) {
            applicationBuilder.UseCors(policy => {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        }

        #endregion
    }
}