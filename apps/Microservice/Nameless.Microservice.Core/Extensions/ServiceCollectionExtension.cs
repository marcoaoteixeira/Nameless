using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nameless.AspNetCore;
using Nameless.AspNetCore.Options;
using Nameless.AspNetCore.Versioning;
using Nameless.Microservice.Infrastructure;
using Nameless.Microservice.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nameless.Microservice.Extensions {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection PrepareAuth(this IServiceCollection services, IConfiguration config) {
            var sectionName = nameof(JwtOptions).RemoveTail(Internals.ClassTokens.OPTIONS);
            var options = config
               .GetSection(sectionName)
               .Get<JwtOptions>() ?? JwtOptions.Default;

            services
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
                                ctx.Response.Headers.Add(Internals.HttpResponseHeaders.JSON_WEB_TOKEN_EXPIRED, bool.TrueString);
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }

        // CORS defines a way in which a browser and server can interact to determine
        // whether or not it is safe to allow the cross-origin request.
        public static IServiceCollection PrepareCors(this IServiceCollection services)
            => services.AddCors();

        public static IServiceCollection PrepareEndpoints(this IServiceCollection services, params Assembly[] assemblies) {
            Prevent.Against.Null(assemblies, nameof(assemblies));

            var implementations = assemblies
                .SelectMany(_ => _.ExportedTypes)
                .Where(_ => typeof(IEndpoint).IsAssignableFrom(_))
                .ToArray();

            foreach (var implementation in implementations) {
                services.AddScoped(typeof(IEndpoint), implementation);
            }

            return services;
        }

        public static IServiceCollection PrepareHealthCheck(this IServiceCollection services) {
            services.AddHealthChecks();

            return services;
        }

        public static IServiceCollection PrepareOptions(this IServiceCollection services, IConfiguration config) {
            services.AddOptions();

            services
                .PushOptions<SwaggerPageOptions>(config)
                .PushOptions<JwtOptions>(config);

            return services;
        }

        public static IServiceCollection PrepareRouting(this IServiceCollection services)
           => services.AddRouting();

        public static IServiceCollection PrepareSwagger(this IServiceCollection services) {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(configure => {
                configure.OperationFilter<SwaggerDefaultValuesOperationFilter>();
                configure.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new() {
                    In = ParameterLocation.Header,
                    Description = "Enter JSON Web Token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower()
                });
                configure.AddSecurityRequirement(new() {
                    {
                        new() {
                            Reference = new() {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        public static IServiceCollection PrepareVersioning(this IServiceCollection services) {
            services.AddApiVersioning(configure => {
                configure.ReportApiVersions = true;
                configure.AssumeDefaultVersionWhenUnspecified = true;
                configure.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddVersionedApiExplorer(opts => {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                opts.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                opts.SubstituteApiVersionInUrl = true;
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerGenConfigureOptions>();

            return services;
        }

        #endregion
    }
}
