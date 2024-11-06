using System.Reflection;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nameless.Web.Endpoints;
using Nameless.Web.Infrastructure;
using Nameless.Web.Options;
using Nameless.Web.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nameless.Web.Extensions;

public static class ServiceCollectionExtension {
    private const string X_JWT_EXPIRED_KEY = "X-JWT-Expired";

    /// <summary>
    ///     Adds JSON Web Token authentication services.
    /// </summary>
    /// <param name="self">The <see cref="IServiceCollection" /> current instance.</param>
    /// <param name="configureAuthorization">The configuration action for authorization.</param>
    /// <param name="configureJwt">The configuration action for JSON Web Token.</param>
    /// <returns>
    ///     The <see cref="IServiceCollection" /> current instance so other actions can be chained.
    /// </returns>
    public static IServiceCollection AddJwtAuth(this IServiceCollection self,
                                                Action<AuthorizationOptions> configureAuthorization,
                                                Action<JwtOptions> configureJwt) {
        Prevent.Argument.Null(self);

        var jwtOptions = new JwtOptions();

        configureJwt(jwtOptions);

        return self.Configure(configureAuthorization)
                   .Configure(configureJwt)
                   .RegisterJwtAuthServices(jwtOptions);
    }

    /// <summary>
    ///     Adds JSON Web Token authentication services.
    /// </summary>
    /// <param name="self">The <see cref="IServiceCollection" /> current instance.</param>
    /// <param name="configureAuthorization">The configuration action for authorization.</param>
    /// <param name="jwtConfigSection">The configuration section for JSON Web Token.</param>
    /// <returns>
    ///     The <see cref="IServiceCollection" /> current instance so other actions can be chained.
    /// </returns>
    public static IServiceCollection AddJwtAuth(this IServiceCollection self,
                                                Action<AuthorizationOptions> configureAuthorization,
                                                IConfigurationSection jwtConfigSection) {
        Prevent.Argument.Null(self);

        var jwtOptions = jwtConfigSection.Get<JwtOptions>() ?? new JwtOptions();

        return self.Configure(configureAuthorization)
                   .Configure<JwtOptions>(jwtConfigSection)
                   .RegisterJwtAuthServices(jwtOptions);
    }

    /// <summary>
    ///     Adds Swashbuckle Swagger API discovery services. Also adds endpoints API explorer.
    ///     This call must be preceded by a call to <see cref="AddApiVersioningDefault" />.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection" /> instance.</param>
    /// <param name="enableJwt">Whether it should enable JWT security.</param>
    /// <returns>
    ///     The current <see cref="IServiceCollection" /> instance so other actions can be chained.
    /// </returns>
    public static IServiceCollection AddSwaggerWithVersioning(this IServiceCollection self, bool enableJwt = false) {
        Prevent.Argument.Null(self);

        self.AddEndpointsApiExplorer()
            .AddSwaggerGen(opts => {
                if (enableJwt) {
                    ConfigureSwaggerJwt(opts);
                }

                opts.OperationFilter<SwaggerDefaultValuesOperationFilter>();
            });

        self.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerGenConfigureOptions>();

        return self;

        static void ConfigureSwaggerJwt(SwaggerGenOptions options) {
            var openApiSecurityScheme = new OpenApiSecurityScheme {
                In = ParameterLocation.Header,
                Description = "Enter JSON Web Token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower()
            };
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                                          openApiSecurityScheme);

            var openApiSecurityRequirement = new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    []
                }
            };
            options.AddSecurityRequirement(openApiSecurityRequirement);
        }
    }

    /// <summary>
    ///     Registers all implementations of <see cref="MinimalEndpointBase" />.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection" /> instance.</param>
    /// <param name="assemblies">The assemblies that will be mapped.</param>
    /// <returns>
    ///     The current <see cref="IServiceCollection" /> instance so other actions can be chained.
    /// </returns>
    public static IServiceCollection AddMinimalEndpoints(this IServiceCollection self, Assembly[] assemblies) {
        Prevent.Argument.Null(self);
        Prevent.Argument.Null(assemblies);

        var endpointType = typeof(MinimalEndpointBase);
        var endpointImplementations =
            assemblies.SelectMany(assembly => assembly.SearchForImplementations<MinimalEndpointBase>());

        foreach (var endpointImplementation in endpointImplementations) {
            self.AddTransient(endpointType,
                              endpointImplementation);
        }

        return self;
    }

    /// <summary>
    ///     Adds API versioning services.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection" /> instance.</param>
    /// <returns>
    ///     The current <see cref="IServiceCollection" /> instance so other actions can be chained.
    /// </returns>
    public static IServiceCollection AddApiVersioningDefault(this IServiceCollection self) {
        self
            .AddApiVersioning(options => {
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
                options.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("api-version"),
                                                                    new UrlSegmentApiVersionReader());
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

    private static IServiceCollection RegisterJwtAuthServices(this IServiceCollection self, JwtOptions jwtOptions) {
        self.AddAuthorization()
            .AddAuthentication(authenticationOptions => {
                authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                authenticationOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtBearerOptions => {
                jwtBearerOptions.RequireHttpsMetadata = jwtBearerOptions.RequireHttpsMetadata;
                jwtBearerOptions.SaveToken = jwtOptions.SaveTokens;
                jwtBearerOptions.TokenValidationParameters = jwtOptions.GetTokenValidationParameters();
                jwtBearerOptions.Events = new JwtBearerEvents {
                    OnAuthenticationFailed = ctx => {
                        if (ctx.Exception is SecurityTokenExpiredException) {
                            ctx.Response.Headers[X_JWT_EXPIRED_KEY] = bool.TrueString;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        return self.AddSingleton<IJwtService, JwtService>();
    }
}