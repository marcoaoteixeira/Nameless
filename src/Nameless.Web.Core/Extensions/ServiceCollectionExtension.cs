using System.Reflection;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nameless.Web.Auth;
using Nameless.Web.Auth.Impl;
using Nameless.Web.Endpoints;
using Nameless.Web.Infrastructure;
using Nameless.Web.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nameless.Web;

public static class ServiceCollectionExtension {
    public static IServiceCollection AddJwtAuth(this IServiceCollection self,
                                                IConfiguration config,
                                                Action<AuthorizationOptions>? configureAuthorization = null,
                                                Action<JwtOptions>? configureJwt = null) {
        Prevent.Argument.Null(self);
        Prevent.Argument.Null(config);

        var jwtOptions = config.GetSection(nameof(JwtOptions))
                               .Get<JwtOptions>() ?? JwtOptions.Default;

        if (configureJwt is not null) {
            self.Configure(configureJwt);
        }

        self.AddAuthorization(configureAuthorization ?? (_ => { }))
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
                            ctx.Response.Headers[Root.HttpResponseHeaders.X_JWT_EXPIRED] = bool.TrueString;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        self.AddSingleton<IJwtService, JwtService>();

        return self;
    }

    /// <summary>
    /// Adds Swashbuckle Swagger API discovery services. Also add endpoints API explorer.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/> instance.</param>
    /// <param name="enableJwt">Whether it should enable JWT security.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/> instance so other actions can be chained.
    /// </returns>
    public static IServiceCollection AddSwagger(this IServiceCollection self, bool enableJwt = false) {
        Prevent.Argument.Null(self);

        self.AddEndpointsApiExplorer()
            .AddSwaggerGen(opts => {
                if (enableJwt) { ConfigureSwaggerJwt(opts); }

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

    /// <summary>
    /// Adds API versioning services.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/> instance.</param>
    /// <remarks>
    /// API version can be set using HTTP header, through key "api-version". Or URL segment
    /// like "api/v[NUMBER]/something"
    /// </remarks>
    /// <returns>
    /// The current <see cref="IServiceCollection"/> instance so other actions can be chained.
    /// </returns>
    public static IServiceCollection AddApiVersioningDefault(this IServiceCollection self) {
        Prevent.Argument.Null(self);

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

    /// <summary>
    /// Registers all implementations of <see cref="IEndpoint"/>.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/> instance.</param>
    /// <param name="assemblies">The assemblies that will be mapped.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/> instance so other actions can be chained.
    /// </returns>
    public static IServiceCollection AddMinimalEndpoints(this IServiceCollection self, Assembly[] assemblies) {
        Prevent.Argument.Null(self);
        Prevent.Argument.Null(assemblies);

        var endpointType = typeof(IEndpoint);
        var endpointImplementations = assemblies.SelectMany(assembly => assembly.SearchForImplementations<IEndpoint>());
        foreach (var endpointImplementation in endpointImplementations) {
            self.AddTransient(serviceType: endpointType,
                              implementationType: endpointImplementation);
        }
        return self;
    }
}