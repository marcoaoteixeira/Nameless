using System;
using ElmahCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Bookshelf.Identity;
using Nameless.Data.SqlClient;

namespace Nameless.Bookshelf.Web {
    public partial class StartUp {
        #region Public Methods

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            ConfigureAuthorization (services);

            ConfigureCookiePolicy (services);

            ConfigureCors (services);

            ConfigureExceptionHandler (services);

            ConfigureMvc (services);

            ConfigureApiVersioning (services);

            ConfigureSettings (services);
        }

        #endregion

        #region Private Static Methods

        private static void ConfigureAuthorization (IServiceCollection services) {
            services.AddAuthorization ();
            services.AddAuthentication ()
                .AddCookie (opts => {
                    opts.LoginPath = "/Account/SignIn";
                    opts.LogoutPath = "/Account/SignOut";
                });

            // add identity
            var identityBuilder = services.AddIdentity<User, Role> (opts => {
                    // Password settings.
                    opts.Password.RequireDigit = true;
                    opts.Password.RequireLowercase = true;
                    opts.Password.RequireNonAlphanumeric = true;
                    opts.Password.RequireUppercase = true;
                    opts.Password.RequiredLength = 6;
                    opts.Password.RequiredUniqueChars = 1;

                    // Lockout settings.
                    opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (5);
                    opts.Lockout.MaxFailedAccessAttempts = 5;
                    opts.Lockout.AllowedForNewUsers = true;

                    // User settings.
                    opts.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    opts.User.RequireUniqueEmail = true;
                });

            identityBuilder = new IdentityBuilder (typeof (User), typeof (Role), identityBuilder.Services);
            identityBuilder.AddDefaultTokenProviders ();
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

        private static void ConfigureExceptionHandler (IServiceCollection services) {
            services.AddElmah (opts => {
                //opts.CheckPermissionAction = ctx => ctx.User.Identity.IsAuthenticated && ctx.User.IsInRole ("SYS_ADMINISTRATOR");
                opts.LogPath = "~/elmah";
            });
        }

        private static void ConfigureMvc (IServiceCollection services) {
            // Add MVC services
            services
                .AddControllersWithViews (config => config.Filters.Add (typeof (ValidateModelStateActionFilter)))
                .AddMvcLocalization ()
                .SetCompatibilityVersion (CompatibilityVersion.Latest);
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

        #endregion

        #region Private Methods

        private void ConfigureSettings (IServiceCollection services) {
            var databaseSettings = _configuration.GetSection (nameof (DatabaseSettings)).Get<DatabaseSettings> ();
            services.ConfigureSettings (_configuration, () => databaseSettings);
        }

        #endregion
    }
}