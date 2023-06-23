using FluentValidation;

namespace Nameless.Microservices {
    public partial class StartUp {
        #region Private Static Methods

        private static void ConfigureFluentValidation(IServiceCollection serviceCollection) {
            serviceCollection.AddValidatorsFromAssemblies(
                assemblies: new[] {
            typeof(StartUp).Assembly
                }
            );
        }

        #endregion
    }
}