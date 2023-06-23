namespace Nameless.Microservices {
    public partial class StartUp {
        #region Private Static Methods

        private static void ConfigureRouting(IServiceCollection serviceCollection) {
            serviceCollection.AddRouting();
        }

        private static void UseRouting(IApplicationBuilder applicationBuilder) {
            applicationBuilder.UseRouting();
        }

        #endregion
    }
}