using FluentValidation;

namespace Nameless.WebApplication.Web {
    
    public partial class StartUp {

        #region Private Static Methods

        private static void ConfigureFluentValidation(IServiceCollection services) {
            services
                .AddValidatorsFromAssemblies(
                    assemblies: new[] {
                        typeof(StartUp).Assembly,
                        typeof(WebApplicationModule).Assembly
                    }
                );
        }

        #endregion
    }
}
