using ElmahCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.AspNetCore.Identity;
using Nameless.AspNetCore.Localization;
using Nameless.FileStorage.FileSystem;
using Nameless.Localization.Json;
using Nameless.Logging.Log4net;
using Nameless.Persistence.NHibernate;

namespace Nameless.Skeleton.Web {
    public partial class StartUp {
        #region Public Methods

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            ConfigureSettings (services, Configuration);
            ConfigureIdentity (services);
            ConfigureMvc (services);
            ConfigureCookiePolicy (services);
            ConfigureCors (services);
            ConfigureErrorHandling (services);
            ConfigureApiVersioning (services);
            ConfigureLocalization (services);
        }

        #endregion

        #region Private Static Methods

        private static void ConfigureSettings (IServiceCollection services, IConfiguration configuration) {
            var fileProviderSettings = configuration
                .GetSection (nameof (FileSystemStorageSettings))
                .Get<FileSystemStorageSettings> () ?? new FileSystemStorageSettings {
                    Root = typeof (StartUp).Assembly.GetDirectoryPath ()
                };
            services.ConfigureSettings (configuration, () => fileProviderSettings);

            var localizationSettings = configuration
                .GetSection (nameof (LocalizationSettings))
                .Get<LocalizationSettings> () ?? new LocalizationSettings ();
            services.ConfigureSettings (configuration, () => localizationSettings);

            var loggingSettings = configuration
                .GetSection (nameof (LoggingSettings))
                .Get<LoggingSettings> () ?? new LoggingSettings ();
            services.ConfigureSettings (configuration, () => loggingSettings);

            var nHibernateSettings = configuration
                .GetSection (nameof (NHibernateSettings))
                .Get<NHibernateSettings> () ?? new NHibernateSettings ();
            services.ConfigureSettings (configuration, () => nHibernateSettings);
        }

        private static void ConfigureIdentity (IServiceCollection services) {
            services.AddNamelessIdentity ();
        }

        private static void ConfigureMvc (IServiceCollection services) {
            // Add MVC services
            services
                .AddControllersWithViews (config => config.Filters.Add (typeof (ValidateModelStateActionFilter)))
                .AddControllersAsServices ()
                .SetCompatibilityVersion (CompatibilityVersion.Latest);
            services
                .AddRazorPages ()
                .AddDataAnnotationsLocalization ();
        }

        private static void ConfigureCookiePolicy (IServiceCollection services) {
            // This lambda determines whether user consent for
            // non-essential cookies is needed for a given request.
            services.Configure<CookiePolicyOptions> (options => {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
        }

        private static void ConfigureCors (IServiceCollection services) {
            // CORS defines a way in which a browser and server can
            // interact to determine whether or not it is safe to
            // allow the cross-origin request.
            services.AddCors ();
        }

        private static void ConfigureErrorHandling (IServiceCollection services) {
            services.AddElmah (opts => {
                opts.CheckPermissionAction = ctx => ctx.User.Identity.IsAuthenticated && ctx.User.IsInRole ("SYS_ADMINISTRATOR");
                opts.LogPath = "~/elmah";
            });
        }

        private static void ConfigureApiVersioning (IServiceCollection services) {
            services
                .AddApiVersioning (opts => {
                    opts.AssumeDefaultVersionWhenUnspecified = true;
                    opts.ApiVersionReader = new MediaTypeApiVersionReader ();
                    opts.ApiVersionSelector = new CurrentImplementationApiVersionSelector (opts);
                    opts.DefaultApiVersion = new ApiVersion (majorVersion: 1, minorVersion: 0);
                });
        }

        private static void ConfigureLocalization (IServiceCollection services) {
            services.AddNamelessLocalization ();
        }

        #endregion
    }
}