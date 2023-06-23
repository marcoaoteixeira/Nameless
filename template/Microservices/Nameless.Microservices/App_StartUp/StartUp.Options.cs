using Nameless.AspNetCore.Options;

namespace Nameless.Microservices {
    public partial class StartUp {
        #region Private Static Methods

        private static void ConfigureOptions(IServiceCollection serviceCollection, IConfiguration configuration) {
            serviceCollection.AddOptions();

            serviceCollection
                 .Configure<SwaggerPageOptions>(configuration.GetSection(GetSectionKey<SwaggerPageOptions>()));
        }

        private static string GetSectionKey<TNode>() {
            return typeof(TNode).Name
                .Replace("Options", string.Empty)
                .Replace("Settings", string.Empty);
        }

        #endregion
    }
}