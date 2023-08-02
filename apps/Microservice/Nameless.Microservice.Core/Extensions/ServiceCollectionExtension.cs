﻿using System.Reflection;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nameless.AspNetCore;
using Nameless.AspNetCore.Options;
using Nameless.AspNetCore.Versioning;
using Nameless.Microservice.Infrastructure;
using Nameless.Microservice.Options;
using Nameless.Microservice.Services;
using Nameless.Microservice.Services.Impl;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nameless.Microservice.Extensions {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection RegisterAuth(this IServiceCollection self, IConfiguration config)
            => RegisterAuth(self, config, setup => { });

        public static IServiceCollection RegisterAuth(this IServiceCollection self, IConfiguration config, Action<AuthorizationOptions> setupAuthorization) {
            var sectionName = nameof(JwtOptions).RemoveTail(Internals.ClassTokens.OPTIONS);
            var options = config
               .GetSection(sectionName)
               .Get<JwtOptions>() ?? JwtOptions.Default;
            
            self
                .AddAuthorization(setupAuthorization)
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

            self
                .AddSingleton<IJwtService, JwtService>();

            return self;
        }

        // CORS defines a way in which a browser and server can interact to determine
        // whether or not it is safe to allow the cross-origin request.
        public static IServiceCollection RegisterCors(this IServiceCollection self)
            => RegisterCors(self, setup => { });

        public static IServiceCollection RegisterCors(this IServiceCollection self, Action<CorsOptions> setup)
            => self.AddCors(setup);

        public static IServiceCollection RegisterEndpoints(this IServiceCollection self, params Assembly[] assemblies) {
            Prevent.Against.Null(assemblies, nameof(assemblies));

            var implementations = assemblies
                .SelectMany(_ => _.ExportedTypes)
                .Where(_ => typeof(IEndpoint).IsAssignableFrom(_))
                .ToArray();

            foreach (var implementation in implementations) {
                self.AddScoped(typeof(IEndpoint), implementation);
            }

            return self;
        }

        public static IServiceCollection RegisterHealthChecks(this IServiceCollection self) {
            self.AddHealthChecks();

            return self;
        }

        public static IServiceCollection RegisterOptions(this IServiceCollection self, IConfiguration config) {
            self.AddOptions();

            self
                .PushOptions<SwaggerPageOptions>(config)
                .PushOptions<JwtOptions>(config);

            return self;
        }

        public static IServiceCollection RegisterRouting(this IServiceCollection self)
           => RegisterRouting(self, setup => { });

        public static IServiceCollection RegisterRouting(this IServiceCollection self, Action<RouteOptions> setup)
           => self.AddRouting(setup);

        public static IServiceCollection RegisterSwagger(this IServiceCollection self) {
            self.AddEndpointsApiExplorer();
            self.AddSwaggerGen(configure => {
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

            self.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();

            return self;
        }

        #endregion
    }
}
