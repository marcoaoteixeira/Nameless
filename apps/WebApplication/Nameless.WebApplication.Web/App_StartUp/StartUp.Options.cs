using Nameless.Localization.Json;
using Nameless.Logging.log4net;
using Nameless.Lucene;
using Nameless.Security;
using Nameless.WebApplication.Options;

namespace Nameless.WebApplication.Web {

    public partial class StartUp {

        #region Private Static Methods

        private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration) {
            services.AddOptions();

            services
                .Configure<AdministratorOptions>(configuration.GetSection(GetSectionKey<AdministratorOptions>()))
                .Configure<JsonWebTokenOptions>(configuration.GetSection(GetSectionKey<JsonWebTokenOptions>()))
                .Configure<LocalizationOptions>(configuration.GetSection(GetSectionKey<LocalizationOptions>()))
                .Configure<Log4netOptions>(configuration.GetSection(GetSectionKey<Log4netOptions>()))
                .Configure<LuceneOptions>(configuration.GetSection(GetSectionKey<LuceneOptions>()))
                .Configure<PasswordGeneratorOptions>(configuration.GetSection(GetSectionKey<PasswordGeneratorOptions>()))
                .Configure<SwaggerPageOptions>(configuration.GetSection(GetSectionKey<SwaggerPageOptions>()));
        }

        #endregion

        #region Private Static Methods

        private static string GetSectionKey<TNode>() {
            return typeof(TNode).Name
                .Replace("Options", string.Empty)
                .Replace("Settings", string.Empty);
        }

        #endregion
    }
}
