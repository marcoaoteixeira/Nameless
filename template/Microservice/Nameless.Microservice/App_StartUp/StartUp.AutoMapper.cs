namespace Nameless.Microservice {
    public partial class StartUp {
        #region Private Static Methods

        private static void ConfigureAutoMapper(IServiceCollection serviceCollection) {
            serviceCollection.AddAutoMapper(
                typeof(StartUp).Assembly
            );
        }

        #endregion
    }
}