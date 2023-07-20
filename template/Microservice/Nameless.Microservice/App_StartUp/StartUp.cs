using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Nameless.Microservice {
    public partial class StartUp {
        #region Public Properties

        public IConfiguration Configuration { get; }

        #endregion

        #region Public Constructors

        public StartUp(IConfiguration configuration) {
            Configuration = configuration;
        }

        #endregion

        #region Public Methods

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            ConfigureOptions(services, Configuration);

            ConfigureAutoMapper(services);

            ConfigureFluentValidation(services);

            ConfigureCors(services);

            ConfigureAuth(services, Configuration);

            ConfigureRouting(services);

            ConfigureEndpoints(services);

            ConfigureSwagger(services);

            ConfigureVersioning(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, IApiVersionDescriptionProvider versioning) {
            UseSwagger(app, env, versioning);

            UseCors(app);

            UseRouting(app);

            UseAuth(app);

            UseEndpoints(app);

            UseHttpSecurity(app, env);

            UseErrorHandling(app, env);

            // Tear down the composition root and free all resources.
            var container = app.ApplicationServices.GetAutofacRoot();
            lifetime.ApplicationStopped.Register(container.Dispose);
        }

        #endregion
    }
}