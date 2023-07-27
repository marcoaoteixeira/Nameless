using Asp.Versioning.ApiExplorer;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Nameless.AutoMapper;
using Nameless.FluentValidation;
using Nameless.Microservice.Extensions;
using Nameless.Microservice.Services;
using Nameless.Microservice.Services.Impl;
using Nameless.Services.Impl;

namespace Nameless.Microservice.Web {
    public class StartUp {
        #region Public Properties

        public IConfiguration Config { get; }
        public IHostEnvironment Env { get; set; }

        #endregion

        #region Public Constructors

        public StartUp(IConfiguration config, IHostEnvironment env) {
            Config = Prevent.Against.Null(config, nameof(config));
            Env = Prevent.Against.Null(env, nameof(env));
        }

        #endregion

        #region Public Methods

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services
                .RegisterOptions(Config)
                .RegisterAutoMapper(typeof(StartUp).Assembly)
                .RegisterFluentValidation(typeof(StartUp).Assembly)
                .RegisterCors()
                .RegisterRouting()
                .RegisterAuth(Config)
                .RegisterHealthChecks()
                .RegisterEndpoints(typeof(StartUp).Assembly)
                .RegisterSwagger()
                .RegisterVersioning();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime, IApiVersionDescriptionProvider versioning) {
            app
                .ResolveCors()
                .ResolveRouting()
                .ResolveAuth()
                .ResolveHealthChecks()
                .ResolveEndpoints()
                .ResolveSwagger(Env, versioning)
                .ResolveHttpSecurity(Env)
                .ResolveErrorHandling(Env);

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

            builder
                .RegisterType<JwtService>()
                .As<IJwtService>()
                .SingleInstance();
        }

        #endregion
    }
}
