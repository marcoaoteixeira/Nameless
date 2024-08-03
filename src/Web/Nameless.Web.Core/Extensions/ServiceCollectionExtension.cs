using System.Reflection;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nameless.Services;
using Nameless.Services.Impl;
using Nameless.Web.Infrastructure;
using Nameless.Web.Options;
using Nameless.Web.Services;
using Nameless.Web.Services.Impl;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nameless.Web {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection RegisterCors(this IServiceCollection self, Action<CorsOptions>? configure = null)
            => self.AddCors(configure ?? (_ => { }));

        public static IHealthChecksBuilder RegisterHealthChecks(this IServiceCollection self)
            => self.AddHealthChecks();

        public static IServiceCollection RegisterHttpContextAccessor(this IServiceCollection self)
            => self.AddHttpContextAccessor();

        public static IServiceCollection RegisterJwtAuth(this IServiceCollection self, IConfiguration config, Action<AuthorizationOptions>? configure = null) {
            var sectionName = nameof(JwtOptions).RemoveTail(Nameless.Root.Defaults.OptionsSettingsTails);
            var jwtOptions = config.GetSection(sectionName)
                                   .Get<JwtOptions>() ?? JwtOptions.Default;

            self.AddAuthorization(configure ?? (_ => { }))
                .AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => {
                    options.RequireHttpsMetadata = options.RequireHttpsMetadata;
                    options.SaveToken = jwtOptions.SaveTokens;
                    options.TokenValidationParameters = jwtOptions.GetTokenValidationParameters();
                    options.Events = new JwtBearerEvents {
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

        public static IServiceCollection RegisterJwtService(this IServiceCollection self, Action<JwtOptions>? configure = null)
            => self.AddSingleton<IJwtService>(provider => {
                var options = provider.GetOptions<JwtOptions>();

                configure?.Invoke(options.Value);

                return new JwtService(options: options.Value,
                                      systemClock: provider.GetService<ISystemClock>() ?? SystemClock.Instance,
                                      logger: provider.GetLogger<JwtService>());
            });

        public static IServiceCollection RegisterMinimalEndpoints(this IServiceCollection self, IEnumerable<Assembly> supportAssemblies) {
            var types = MinimalEndpointHelper.ScanAssemblies(supportAssemblies);
            // In the future, check if it'll be necessary to "keyed"
            // this services.
            foreach (var type in types) {
                self.AddTransient(serviceType: typeof(IMinimalEndpoint),
                                  implementationType: type);
            }
            return self;
        }

        public static IServiceCollection RegisterProblemDetails(this IServiceCollection self, Action<ProblemDetailsOptions>? configure = null)
            => self.AddProblemDetails(configure);

        public static IServiceCollection RegisterRouting(this IServiceCollection self, Action<RouteOptions>? configure = null)
            => self.AddRouting(configure ?? (_ => { }));

        public static IServiceCollection RegisterSwagger(this IServiceCollection self, bool enableSecurity = false) {
            self.AddEndpointsApiExplorer()
                .AddSwaggerGen(enableSecurity ? ConfigureSecurity : _ => { });

            self.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerGenConfigureOptions>();

            return self;

            static void ConfigureSecurity(SwaggerGenOptions options) {
                var openApiSecurityScheme = new OpenApiSecurityScheme {
                    In = ParameterLocation.Header,
                    Description = "Enter JSON Web Token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower()
                };
                options.AddSecurityDefinition(
                    name: JwtBearerDefaults.AuthenticationScheme,
                    securityScheme: openApiSecurityScheme
                );

                var openApiSecurityRequirement = new OpenApiSecurityRequirement { {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        }, []
                    }
                };
                options.AddSecurityRequirement(openApiSecurityRequirement);
            }
        }

        public static IServiceCollection RegisterVersioning(this IServiceCollection self) {
            self.AddApiVersioning(options => {
                    // Add the headers "api-supported-versions" and "api-deprecated-versions"
                    // This is better for discoverability
                    options.ReportApiVersions = true;

                    // AssumeDefaultVersionWhenUnspecified should only be enabled when supporting legacy services that did not previously
                    // support API versioning. Forcing existing clients to specify an explicit API version for an
                    // existing service introduces a breaking change. Conceptually, clients in this situation are
                    // bound to some API version of a service, but they don't know what it is and never explicit request it.
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1);

                    // Defines how an API version is read from the current HTTP request
                    options.ApiVersionReader = ApiVersionReader.Combine(
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

        #endregion
    }
}
