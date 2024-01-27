using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nameless.Infrastructure;
using Nameless.Infrastructure.Impl;
using Nameless.Web.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using CoreRoot = Nameless.Root;

namespace Nameless.Web {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        /// <summary>
        /// Registers an object to act like an option that will get its values
        /// from <see cref="IConfiguration"/> (using the Bind method).
        /// </summary>
        /// <typeparam name="TOptions">The type of the object.</typeparam>
        /// <param name="self">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="optionsProvider">The option provider.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection RegisterOptions<TOptions>(this IServiceCollection self, IConfiguration configuration, Func<TOptions> optionsProvider) where TOptions : class {
            Guard.Against.Null(configuration, nameof(configuration));
            Guard.Against.Null(optionsProvider, nameof(optionsProvider));

            var opts = optionsProvider();
            var key = typeof(TOptions)
                .Name
                .RemoveTail(CoreRoot.Defaults.OptionsSettingsTails);

            configuration.Bind(key, opts);
            self.AddSingleton(opts);

            return self;
        }

        /// <summary>
        /// Registers an object to act like an option that will get its values
        /// from <see cref="IConfiguration"/> (using the Bind method).
        /// </summary>
        /// <typeparam name="TOptions">The type of the object.</typeparam>
        /// <param name="self">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection RegisterOptions<TOptions>(this IServiceCollection self, IConfiguration configuration) where TOptions : class, new()
            => RegisterOptions(self, configuration, () => new TOptions());

        public static IServiceCollection RegisterApplicationContext(this IServiceCollection self, Version? appVersion = null)
            => self
                .AddSingleton<IApplicationContext>(provider => {
                    var hostEnvironment = provider.GetRequiredService<IHostEnvironment>();

                    return new ApplicationContext(
                        hostEnvironment,
                        useAppDataSpecialFolder: false,
                        appVersion: appVersion ?? new(major: 0, minor: 0, build: 0)
                    );
                });

        public static IServiceCollection RegisterHttpContextAccessor(this IServiceCollection self)
            => self
                .AddHttpContextAccessor();

        public static IServiceCollection RegisterCors(this IServiceCollection self)
            => self
                .AddCors();

        public static IServiceCollection RegisterRouting(this IServiceCollection self)
            => self
                .AddRouting();

        public static IServiceCollection RegisterJwtAuth(this IServiceCollection self, IConfiguration config) {
            var sectionName = nameof(JwtOptions)
                .RemoveTail(CoreRoot.Defaults.OptionsSettingsTails);
            var options = config
               .GetSection(sectionName)
               .Get<JwtOptions>() ?? JwtOptions.Default;

            self
                .AddAuthorization()
                .AddAuthentication(configure => {
                    configure.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    configure.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    configure.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(configure => {
                    configure.RequireHttpsMetadata = options.RequireHttpsMetadata;
                    configure.SaveToken = options.SaveTokens;
                    configure.TokenValidationParameters = options.GetTokenValidationParameters();
                    configure.Events = new JwtBearerEvents {
                        OnAuthenticationFailed = ctx => {
                            if (ctx.Exception is SecurityTokenExpiredException) {
                                ctx.Response.Headers[Root.HttpResponseHeaders.X_JWT_EXPIRED] = bool.TrueString;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            return self;
        }

        public static IServiceCollection RegisterHealthChecks(this IServiceCollection self) {
            self
                .AddHealthChecks();

            return self;
        }

        public static IServiceCollection RegisterSwagger(this IServiceCollection self) {
            self
                .AddEndpointsApiExplorer()
                .AddSwaggerGen(configure => {
                    var openApiSecurityScheme = new OpenApiSecurityScheme {
                        In = ParameterLocation.Header,
                        Description = "Enter JSON Web Token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower()
                    };
                    configure.AddSecurityDefinition(
                        name: JwtBearerDefaults.AuthenticationScheme,
                        securityScheme: openApiSecurityScheme
                    );

                    var openApiSecurityRequirement = new OpenApiSecurityRequirement();
                    openApiSecurityRequirement.Add(
                        key: new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        value: []
                    );
                    configure.AddSecurityRequirement(openApiSecurityRequirement);
                });

            self.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerGenConfigureOptions>();

            return self;
        }

        public static IServiceCollection RegisterVersioning(this IServiceCollection self) {
            self
                .AddApiVersioning(configure => {
                    // Add the headers "api-supported-versions" and "api-deprecated-versions"
                    // This is better for discoverability
                    configure.ReportApiVersions = true;

                    // AssumeDefaultVersionWhenUnspecified should only be enabled when supporting legacy services that did not previously
                    // support API versioning. Forcing existing clients to specify an explicit API version for an
                    // existing service introduces a breaking change. Conceptually, clients in this situation are
                    // bound to some API version of a service, but they don't know what it is and never explicit request it.
                    configure.AssumeDefaultVersionWhenUnspecified = true;
                    configure.DefaultApiVersion = new ApiVersion(1);

                    // Defines how an API version is read from the current HTTP request
                    configure.ApiVersionReader = ApiVersionReader.Combine(
                        new HeaderApiVersionReader("api-version"),
                        new UrlSegmentApiVersionReader()
                    );
                })
                .AddApiExplorer(opts => {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    opts.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    opts.SubstituteApiVersionInUrl = true;
                });

            return self;
        }

        public static IServiceCollection RegisterProblemDetails(this IServiceCollection self)
            => self.AddProblemDetails();

        #endregion
    }
}
