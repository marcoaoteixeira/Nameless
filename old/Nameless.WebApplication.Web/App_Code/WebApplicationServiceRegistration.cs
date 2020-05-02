using System;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ElmahCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nameless.Data.SqlClient;
using Nameless.IoC.Autofac;
using Nameless.WebApplication.Auth;
using Nameless.WebApplication.Identity;

namespace Nameless.WebApplication.Web {
    public sealed class WebApplicationServiceRegistration : ServiceRegistrationBase {
        #region Private Read-Only Fields

        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;

        #endregion

        #region Public Constructors

        public WebApplicationServiceRegistration (IConfiguration configuration, IServiceCollection services) {
            Prevent.ParameterNull (configuration, nameof (configuration));
            Prevent.ParameterNull (services, nameof (services));

            _configuration = configuration;
            _services = services;
        }

        #endregion

        #region Protected Override Methods

        protected override void Load (ContainerBuilder builder) {
            RegisterServices (builder);

            ConfigureCookiePolicy (_services);

            ConfigureCors (_services);

            ConfigureExceptionHandler (_services);

            ConfigureMvc (_services);

            ConfigureApiVersioning (_services);

            ConfigureAuth (_configuration, _services);

            builder.Populate (_services);

            base.Load (builder);
        }

        #endregion

        #region Private Static Methods

        private static void RegisterServices (ContainerBuilder builder) {
            // Register identity stores
            builder
                .RegisterType<UserStore> ()
                .As<IUserStore<User>> ()
                .InstancePerDependency ();
            builder
                .RegisterType<RoleStore> ()
                .As<IRoleStore<Role>> ()
                .InstancePerDependency ();
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
                opts.CheckPermissionAction = ctx => ctx.User.Identity.IsAuthenticated && ctx.User.IsInRole ("SYS_ADMINISTRATOR");
                opts.LogPath = "~/elmah";
            });
        }

        private static void ConfigureMvc (IServiceCollection services) {
            // Add MVC services
            services
                .AddMvc (opts => { opts.Filters.Add (typeof (ValidateModelStateActionFilter)); })
                .AddControllersAsServices ()
                .AddMvcLocalization ()
                .SetCompatibilityVersion (CompatibilityVersion.Version_2_2);
        }

        private static void ConfigureAuth (IConfiguration config, IServiceCollection services) {
            // Register the ConfigurationBuilder instance of AuthSettings
            var authConfig = config.GetSection (nameof (AuthConfiguration)).Get<AuthConfiguration> ();
            var signingKey = new SymmetricSecurityKey (Encoding.ASCII.GetBytes (authConfig.SecretKey));

            // jwt wire up
            // Get options from app settings
            var jwtIssuerOptions = config.GetSection (nameof (JwtIssuerOptions)).Get<JwtIssuerOptions> ();
            jwtIssuerOptions.SigningCredentials = new SigningCredentials (signingKey, SecurityAlgorithms.HmacSha256);

            var tokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = jwtIssuerOptions.ValidateIssuer,
                ValidIssuer = jwtIssuerOptions.Issuer,

                ValidateAudience = jwtIssuerOptions.ValidateAudience,
                ValidAudience = jwtIssuerOptions.Audience,

                ValidateIssuerSigningKey = jwtIssuerOptions.ValidateIssuerSigningKey,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = jwtIssuerOptions.RequireExpirationTime,
                ValidateLifetime = jwtIssuerOptions.ValidateLifetime,
                ClockSkew = TimeSpan.FromMinutes (jwtIssuerOptions.ClockSkew)
            };

            services
                .AddAuthentication (opts => {
                    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer (opts => {
                    opts.ClaimsIssuer = jwtIssuerOptions.Issuer;
                    opts.TokenValidationParameters = tokenValidationParameters;
                    opts.SaveToken = true;

                    opts.Events = new JwtBearerEvents {
                        OnAuthenticationFailed = context => {
                            if (context.Exception.GetType () == typeof (SecurityTokenExpiredException)) {
                                context.Response.Headers.Add ("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            // api user claim policy
            services.AddAuthorization (opts => {
                opts.AddPolicy (Constants.Authorization_Policy_Name, policy => policy.RequireClaim (Constants.JwtClaimIdentifiers_Rol, Constants.JwtClaims_ApiAccess));
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
            identityBuilder
                .AddDefaultTokenProviders ();
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
    }
}