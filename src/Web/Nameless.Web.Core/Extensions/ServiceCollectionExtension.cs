using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nameless.Web.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using RootFromCore = Nameless.Root;

namespace Nameless.Web {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection RegisterCors(this IServiceCollection self)
            => self
                .AddCors();

        public static IServiceCollection RegisterHealthChecks(this IServiceCollection self) {
            self
                .AddHealthChecks();

            return self;
        }

        public static IServiceCollection RegisterHttpContextAccessor(this IServiceCollection self)
            => self
                .AddHttpContextAccessor();

        public static IServiceCollection RegisterJwtAuth(this IServiceCollection self, IConfiguration config)
            => RegisterJwtAuth(self, config, _ => { });

        public static IServiceCollection RegisterJwtAuth(this IServiceCollection self, IConfiguration config, Action<AuthorizationOptions> configureAuthorization) {
            var sectionName = nameof(JwtOptions)
                .RemoveTail(RootFromCore.Defaults.OptionsSettingsTails);
            var options = config
               .GetSection(sectionName)
               .Get<JwtOptions>() ?? JwtOptions.Default;

            self
                .AddAuthorization(configureAuthorization)
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

        public static IServiceCollection RegisterProblemDetails(this IServiceCollection self, Action<ProblemDetailsOptions>? opts = null)
            => self.AddProblemDetails(opts);

        public static IServiceCollection RegisterRouting(this IServiceCollection self)
            => self
                .AddRouting();

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

        #endregion
    }
}
