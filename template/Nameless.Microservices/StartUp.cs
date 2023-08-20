﻿using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nameless.Microservices.Repositories;
using Nameless.Microservices.Services;
using Nameless.Services.Impl;
using Nameless.Web;
using Nameless.Web.Infrastructure;
using Nameless.Web.Middlewares;
using Nameless.Web.Options;
using Nameless.Web.Services;
using Nameless.Web.Services.Impl;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebRoot = Nameless.Web.Root;

namespace Nameless.Microservices {
    public sealed class StartUp {
        #region Private Static Properties

        private static Assembly[] SupportAssemblies { get; } = new[] {
            typeof(StartUp).Assembly
        };

        #endregion

        #region Public Properties

        public IConfiguration Configuration { get; }

        #endregion

        #region Public Constructors

        public StartUp(IConfiguration configuration) {
            Configuration = Guard.Against.Null(configuration, nameof(configuration));
        }

        #endregion

        #region Public Methods

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // Configure options
            services
                .AddOptions()
                .PushOptions<SwaggerPageOptions>(Configuration)
                .PushOptions<JwtOptions>(Configuration);

            // AutoMapper
            services
                .AddAutoMapper(typeof(StartUp).Assembly);

            // FluentValidation
            services
                .AddValidatorsFromAssembly(typeof(StartUp).Assembly);

            // Cors
            services
                .AddCors();

            // Routing
            services
                .AddRouting();

            // Auth
            var sectionName = nameof(JwtOptions)
                .RemoveTail(Root.Defaults.OptionsSettingsTail);
            var options = Configuration
               .GetSection(sectionName)
               .Get<JwtOptions>() ?? JwtOptions.Default;

            services
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
                                ctx.Response.Headers.Add(WebRoot.HttpResponseHeaders.X_JWT_EXPIRED, bool.TrueString);
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddSingleton<IJwtService, JwtService>();

            // Healthchecks
            services
                .AddHealthChecks();

            // Minimal Endpoints
            var implementations = SupportAssemblies
                .SelectMany(_ => _.ExportedTypes)
                .Where(_ => typeof(IMinimalEndpoint).IsAssignableFrom(_))
                .ToArray();

            foreach (var implementation in implementations) {
                services.AddScoped(typeof(IMinimalEndpoint), implementation);
            }

            // Swagger
            services
                .AddEndpointsApiExplorer();
            services
                .AddSwaggerGen(configure => {
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

            // Versioning
            services
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

            services
                .AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerGenConfigureOptions>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, IHostApplicationLifetime lifetime, IApiVersionDescriptionProvider versioning) {
            // Cors
            app
                .UseCors(configure
                    => configure
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());

            // Routing
            app
                .UseRouting();

            // Auth
            app
               .UseAuthorization()
               .UseAuthentication()
               .UseMiddleware<JwtAuthorizationMiddleware>();

            // HealthChecks
            app
                .UseHealthChecks("/healthz");

            // Minimal Endpoints
            app
                .UseEndpoints(setup => {
                    var endpoints = app.ApplicationServices.GetServices<IMinimalEndpoint>();
                    foreach (var endpoint in endpoints) {
                        endpoint
                            .Map(setup)

                            .WithOpenApi()

                            .WithName(endpoint.Name)
                            .WithSummary(endpoint.Summary)
                            .WithDescription(endpoint.Description)

                            .WithApiVersionSet(setup.NewApiVersionSet(endpoint.ApiSet).Build())

                            .HasApiVersion(endpoint.ApiVersion);
                    }
                });

            // Swagger
            app
                .UseSwagger()
                .UseSwaggerUI(setup => {
                    foreach (var description in versioning.ApiVersionDescriptions) {
                        setup.SwaggerEndpoint(
                            url: $"/swagger/{description.GroupName}/swagger.json",
                            name: description.GroupName.ToUpperInvariant()
                        );
                    }
                });

            // HttpSecurity
            if (!env.IsDevelopment()) {
                // The default HSTS value is 30 days. You may want to change
                // this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            // ErrorHandling
            if (env.IsDevelopment()) {
                app.UseExceptionHandler("/error");
            }

            // Tear down the composition root and free all resources.
            var container = app.ApplicationServices.GetAutofacRoot();
            lifetime.ApplicationStopped.Register(container.Dispose);
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder) {
            builder
                .RegisterInstance(ClockService.Instance);

            // Example
            var path = typeof(StartUp).Assembly.GetDirectoryPath("data/database.json");
            builder
                .RegisterInstance(new TodoRepository(path))
                .SingleInstance();
            builder
                .Register(ctx => new TodoService(ctx.Resolve<TodoRepository>()))
                .SingleInstance();
        }

        #endregion
    }
}